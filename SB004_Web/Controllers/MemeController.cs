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
                meme.Title,
                meme.Comments,
                meme.ResponseToId,
                replyCount = meme.ReplyIds.Count
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
//          string userId = User.Identity.UserId();
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

            var jsonMediaTypeFormatter = new JsonMediaTypeFormatter
            {
                SerializerSettings =
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, meme, jsonMediaTypeFormatter);
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

        var jsonMediaTypeFormatter = new JsonMediaTypeFormatter
        {
          SerializerSettings =
          {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
          }
        };
        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, meme, jsonMediaTypeFormatter);
        response.Headers.Location = new Uri(Request.RequestUri, "/api/meme/" + meme.Id);
        return response;
      }
    }
}
