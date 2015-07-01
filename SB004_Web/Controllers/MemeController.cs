using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SB004.Controllers
{
    using System;
    using System.Linq;

    using SB004.Data;
    using SB004.Domain;
    using SB004.Models;
    using SB004.User;
    using SB004.Utilities;
    using System.Net.Http.Formatting;
    using Newtonsoft.Json.Serialization;
    using System.Collections.Generic;

  [RoutePrefix("api/meme")]
  public class MemeController : ApiController
    {
        readonly IRepository repository;
        readonly IImageManager imageManager;
        public MemeController(IRepository repository, IImageManager imageManager)
        {
            this.repository = repository;
            this.imageManager = imageManager;
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

            return Ok(new  
            { 
                meme.Id,
                meme.CreatedBy,
                meme.CreatedByUserId,
                DateCreated = meme.DateCreated.ToLocalTime(),
                meme.Title,
                meme.Comments,
                meme.ResponseToId,
                meme.ReplyIds,
                meme.Likes,
                meme.Dislikes,
                meme.Favourites,
                meme.Shares,
                meme.Views,
                seedImage = new
                {
                 seed.Id,
                 seed.Width,
                 seed.Height,
                 image = "data:image/jpeg;base64," + imageManager.GetImageData(seed.ImageData)
                }
            });
        }
        /// <summary>
        /// Get: api/meme/lite/id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("lite/{id}")]
        public IHttpActionResult GetLiteMeme(string id)
        {
            IMeme meme = repository.GetMeme(id);
            if (meme == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                meme.Id,
                meme.CreatedBy,
                meme.CreatedByUserId,
                DateCreated = meme.DateCreated.ToLocalTime(),
                meme.ResponseToId,
                meme.ReplyIds,
                replyCount = meme.ReplyIds.Count,
                userCommentCount = repository.GetUserCommentCount(id),
                meme.Likes,
                meme.Dislikes,
                meme.Favourites,
                meme.Shares,
                meme.Views,
                
            });
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
          IEnumerable<IMeme> searchResults = repository.SearchMeme(0, 10);
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
          string userId = User.Identity.UserId();
//          string userName = User.Identity.Name;

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
                ReplyIds = new List<string>()
            };

            // Fall back on adding the text manually to the seed if mem image not supplied by client
            if (meme.ImageData == null || meme.ImageData.Length == 0) { 
                // Get the seed image
                byte[] seedData = repository.GetSeed(meme.SeedId).ImageData;

                // Add the meme comments to the seed image to make the meme
                meme.ImageData = imageManager.GenerateMemeImage(meme, seedData);
            }
            
            //save the meme 
            meme = repository.SaveMeme(meme);



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
        if (meme.ReplyIds == null)
        {
          meme.ReplyIds = new List<string>();
        }

        // Add the repy id to the top of the list of replies
        meme.ReplyIds.Insert(0, replyMemeId);
        
        // Update the meme
        repository.SaveMeme(meme);

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
          return MemeInteration(id, 1, 0, 0, 0, 0);
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
          return MemeInteration(id, 0, 1, 0, 0, 0);
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
          return MemeInteration(id, 0, 0, 1, 0, 0);
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
          return MemeInteration(id, 0, 0, 0, 1, 0);
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
          userProfile = repository.SaveUser(userProfile);

          // Update meme increment favourites count only
          return MemeInteration(id, 0, 0, 0, 0, 1);

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
      private HttpResponseMessage MemeInteration(string id, int likesIncrement, int dislikesIncrement, int viewsIncrement, int sharesIncrement, int favouritesIncrement)
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

          meme = repository.SaveMeme(meme);

          var response = Request.CreateResponse(HttpStatusCode.OK, meme);
          response.Headers.Location = new Uri(Request.RequestUri, "/api/meme/" + meme.Id);
          return response;
      }
 
        #endregion
    }
}
