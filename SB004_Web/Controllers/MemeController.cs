﻿using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SB004.Controllers
{
  using System;
  using System.Linq;

  using SB004.Business;
  using SB004.Data;
  using SB004.Domain;
  using SB004.Models;
  using SB004.User;
  using SB004.Utilities;

  using System.Collections.Generic;

  [RoutePrefix("api/meme")]
  public class MemeController : ApiController
  {
    readonly IRepository repository;
    readonly IImageManager imageManager;
    readonly IMemeBusiness memeBusiness;

    public MemeController(IRepository repository, IImageManager imageManager, IMemeBusiness memeBusiness)
    {
      this.repository = repository;
      this.imageManager = imageManager;
      this.memeBusiness = memeBusiness;
    }
    /// <summary>
    /// Get: api/meme/id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Route("{id}")]
    public IHttpActionResult Get(string id)
    {
      IMeme meme = repository.GetMeme(id);
      if (meme == null)
      {
        return NotFound();
      }
      ISeed seed = repository.GetSeed(meme.SeedId);

      return
        Ok(
          new
          {
            meme.Id,
            meme.CreatedBy,
            meme.CreatedByUserId,
            DateCreated = meme.DateCreated.ToLocalTime(),
            meme.Title,
            meme.Comments,
            meme.ResponseToId,
            replyCount = meme.ReplyIds.Count,
            userCommentCount = repository.GetUserCommentCount(id),
            meme.Likes,
            meme.Dislikes,
            meme.Favourites,
            meme.Shares,
            meme.Views,
            seedImage =
              new
              {
                seed.Id,
                seed.Width,
                seed.Height,
                image = "data:image/jpeg;base64," + imageManager.GetImageData(seed.ImageData)
              }
          });
    }

    /// <summary>
    /// Retrieves a paginated list of meme replies ordered by reply trend and date created
    /// Get: api/meme/id/replies/
    /// </summary>
    /// <param name="id"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <returns></returns>
    [Route("{id}/replies")]
    public IHttpActionResult GetMemeReplies(string id, int skip, int take)
    {
      IMeme meme = repository.GetMeme(id);
      int fullReplyCount = 0;
      if (meme == null)
      {
        return NotFound();
      }

      List<MemeLiteModel> replies = new List<MemeLiteModel>();

      // No replies no list
      if (meme.ReplyIds != null)
      {
        // Gwet the full count of all replies (even if not all are being returned)
        fullReplyCount = meme.ReplyIds.Count;
        
        // Get the meme ids of the relevent replies
        IEnumerable<IReply> memeReplies = meme.ReplyIds
                                             .OrderByDescending(x => x.TrendScore)
                                             .ThenByDescending(y => y.DateCreated)
                                             .Skip(skip)
                                             .Take(take);
        // Create the list of reply memes
        foreach (IReply reply in memeReplies)
        {
          MemeLiteModel replyMeme = this.GetMemeLite(reply.Id);
          if (replyMeme != null)
          {
            replies.Add(replyMeme);
          }
        }
      }

      return Ok(new { fullReplyCount, replies });
    }
    /// <summary>
    /// Returns a lite meme model
    /// Get: api/meme/lite/
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Route("lite/{id}")]
    public IHttpActionResult GetLiteMeme(string id)
    {
      MemeLiteModel meme = this.GetMemeLite(id);
      if (meme == null)
      {
        return NotFound();
      }

      return Ok(meme);
    }

    /// <summary>
    /// Get: api/meme/
    /// Return default search of memes for the given user if authenticated or general search if not
    /// </summary>
    /// <returns></returns>
    [Route("")]
    public IHttpActionResult Get()
    {
      //string userId = User.Identity.UserId();
      IEnumerable<IMeme> searchResults = repository.SearchTrendingMemes(0, 10);
      return Ok(searchResults);
    }
    /// <summary>
    /// POST: api/Meme
    /// Save the meme and generate seed id . 
    /// <param name="memeModel">Meme to add</param> 
    /// </summary>
    [Authorize]
    [HttpPost]
    [Route("")]
    public HttpResponseMessage Post([FromBody]MemeModel memeModel)
    {

      IMeme meme = new Meme
      {
        CreatedByUserId = User.Identity.UserId(),
        CreatedBy = User.Identity.Name,
        DateCreated = DateTime.Now.ToUniversalTime(),
        SeedId = memeModel.SeedId,
        Comments = memeModel.Comments.Select(x => (IComment)new Comment
        {
          Id = x.Id,
          BackgroundColor = x.BackgroundColor,
          Color = x.Color,
          FontFamily = x.FontFamily,
          FontSize = x.FontSize,
          FontStyle = x.FontStyle,
          FontWeight = x.FontWeight,
          Position = new PositionRef
          {
            Align = x.Position.Align,
            X = x.Position.X,
            Y = x.Position.Y,
            Width = x.Position.Width,
            Height = x.Position.Height
          },
          Text = x.Text,
          TextAlign = x.TextAlign,
          TextDecoration = x.TextDecoration,
          TextShadow = x.TextShadow,
        }).ToList(),
        ImageData = Convert.FromBase64String(memeModel.ImageData),
        ResponseToId = memeModel.ResponseToId,
        ReplyIds = new List<IReply>(),

      };

      //save the meme 
      meme = memeBusiness.SaveMeme(meme);

      HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, meme);
      response.Headers.Location = new Uri(Request.RequestUri, "/api/meme/" + meme.Id);
      return response;
    }

    /// <summary>
    ///
    /// </summary>
    [HttpPatch]
    [Route("{id}/reply/{replyMemeId}")]
    public HttpResponseMessage AddReply(string id, string replyMemeId)
    {
      IMeme meme = repository.GetMeme(id);

      if (meme == null)
      {
        return Request.CreateResponse(HttpStatusCode.NotFound);
      }

      // Add the repy id to the meme, calculate a reply trend
      memeBusiness.AddReplyToMeme(meme, replyMemeId);

      // Update the meme
      memeBusiness.SaveMeme(meme);

      HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, meme);
      response.Headers.Location = new Uri(Request.RequestUri, "/api/meme/" + meme.Id);
      return response;
    }
    /// <summary>
    /// Retrieve the meme. Increment the like count
    /// </summary>
    /// <param name="id">Id of the meme</param>
    /// <returns></returns>
    [HttpPatch]
    [Route("{id}/like/")]
    public HttpResponseMessage LikeMeme(string id)
    {
      // Update meme increment like only
      return MemeInteraction(id, 1, 0, 0, 0, 0);
    }
    /// <summary>
    /// Retrieve the meme. Increment the meme dislike count
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch]
    [Route("{id}/dislike/")]
    public HttpResponseMessage DislikeMeme(string id)
    {
      // Update meme increment dislike only
      return MemeInteraction(id, 0, 1, 0, 0, 0);
    }
    /// <summary>
    /// Record thar this meme was opened for viewing
    /// Retrieve the meme. Increment the meme view count
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch]
    [Route("{id}/viewed/")]
    public HttpResponseMessage ViewMeme(string id)
    {
      // Update meme increment views only
      return MemeInteraction(id, 0, 0, 1, 0, 0);
    }
    /// <summary>
    /// Record that this meme was shared
    /// Retrieve the meme. Increment the meme view count
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch]
    [Route("{id}/shared/")]
    public HttpResponseMessage ShareMeme(string id)
    {
      // Update meme increment shares only
      return MemeInteraction(id, 0, 0, 0, 1, 0);
    }
    /// <summary>
    /// Requires authentication. Retrieve the authenticated user. Add the meme id to the users list of favourites.
    /// Increment the number of favourites this meme is listed as
    /// </summary>
    /// <param name="id">meme id of favoutite meme</param>
    /// <returns></returns>
    [Authorize]
    [HttpPatch]
    [Route("{id}/favourite/")]
    public HttpResponseMessage AddAsFavouriteMeme(string id)
    {
      string userId = User.Identity.UserId();

      // Retrieve the authenticated user
      IUser userProfile = repository.GetUser(userId);
      if (userProfile == null)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
      }

      // Is meme already a favourite of this user?
      if (userProfile.FavouriteMemeIds.Any(x => x == id))
      {
        var response = Request.CreateResponse(HttpStatusCode.PreconditionFailed);
        return response;
      }

      // Add as user favourite
      userProfile.FavouriteMemeIds.Add(id);

      // Save the user
      this.repository.Save(userProfile);

      // Update meme increment favourites count only
      return MemeInteraction(id, 0, 0, 0, 0, 1);

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPatch]
    [Route("{id}/repost/")]
    public HttpResponseMessage RepostMeme(string id)
    {
        string userId = User.Identity.UserId();
        // Retrieve the authenticated user
        IUser userProfile = repository.GetUser(userId);
        if (userProfile == null)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
        IMeme meme = repository.GetMeme(id);
        if (meme == null)
        {
            var responseNotFound = Request.CreateResponse(HttpStatusCode.NotFound);
            responseNotFound.Headers.Location = new Uri(Request.RequestUri, "/api/meme/" + id);
            return responseNotFound;
        }

        // Repost the meme under the users name
        meme = memeBusiness.RepostMeme(meme, userProfile);

        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, meme);
        response.Headers.Location = new Uri(Request.RequestUri, "/api/meme/" + meme.Id);
        return response;
    }
    /// <summary>
    /// Requires authentication. Retrieve the authenticated user. Remove the meme id from the users list of favourites.
    /// Decrement the number of favourites this meme is listed as
    /// </summary>
    /// <param name="id">meme id of favoutite meme</param>
    /// <returns></returns>
    [Authorize]
    [HttpDelete]
    [Route("{id}/favourite/")]
    public HttpResponseMessage DropAsFavouriteMeme(string id)
    {
      string userId = User.Identity.UserId();

      // Retrieve the authenticated user
      IUser userProfile = repository.GetUser(userId);
      if (userProfile == null)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
      }

      // Is meme actually a favourite of this user?
      if (userProfile.FavouriteMemeIds.All(x => x != id))
      {
        var response = Request.CreateResponse(HttpStatusCode.PreconditionFailed);
        return response;
      }

      // Remove as user favourite
      userProfile.FavouriteMemeIds.Remove(id);

      // Save the user
      this.repository.Save(userProfile);

      // Update meme decrement favourites count only
      return MemeInteraction(id, 0, 0, 0, 0, -1);

    }
    #region Private Methods
    /// <summary>
    /// Record interation with the meme. 
    /// Increment count of likes, dislikes, views, shares 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="likesIncrement"></param>
    /// <param name="dislikesIncrement"></param>
    /// <param name="viewsIncrement"></param>
    /// <param name="sharesIncrement"></param>
    /// <param name="favouritesIncrement"></param>
    /// <returns></returns>
    // ReSharper disable once UnusedParameter.Local
    private HttpResponseMessage MemeInteraction(string id, int likesIncrement, int dislikesIncrement, int viewsIncrement, int sharesIncrement, int favouritesIncrement)
    {

      IMeme meme = repository.GetMeme(id);
      if (meme == null)
      {
        var responseNotFound = Request.CreateResponse(HttpStatusCode.NotFound);
        responseNotFound.Headers.Location = new Uri(Request.RequestUri, "/api/meme/" + id);
        return responseNotFound;
      }

      meme.Likes += likesIncrement;
      meme.Dislikes += dislikesIncrement;
      meme.Views += viewsIncrement;
      meme.Shares += sharesIncrement;
      meme.Favourites += favouritesIncrement;

      // Update the meme
      meme = memeBusiness.SaveMeme(meme);

      var response = Request.CreateResponse(HttpStatusCode.OK, meme);
      response.Headers.Location = new Uri(Request.RequestUri, "/api/meme/" + meme.Id);
      return response;
    }
    /// <summary>
    /// Retrieved a lite meme model
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private MemeLiteModel GetMemeLite(string id)
    {
      IMeme meme = repository.GetMeme(id);
      if (meme == null)
      {
        return null;
      }

      return new MemeLiteModel
      {
        Id = meme.Id,
        CreatedBy = meme.CreatedBy,
        CreatedByUserId = meme.CreatedByUserId,
        DateCreated = meme.DateCreated.ToLocalTime(),
        ResponseToId = meme.ResponseToId,
        replyCount = meme.ReplyIds.Count,
        userCommentCount = repository.GetUserCommentCount(id),
        Likes = meme.Likes,
        Dislikes = meme.Dislikes,
        Favourites = meme.Favourites,
        Shares = meme.Shares,
        Views = meme.Views
      };
    }
    #endregion
  }
}
