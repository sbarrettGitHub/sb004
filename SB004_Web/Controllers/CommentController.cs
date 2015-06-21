using SB004.Data;
using SB004.Domain;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SB004.User;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;
using SB004.Models;
namespace SB004.Controllers
{


    public class CommentController : ApiController
    {
        readonly IRepository repository;
        public CommentController(IRepository repository)
        {
            this.repository = repository;
        }
        public IHttpActionResult Get(string id, int skip, int take)
        {
            return this.Ok(repository.GetUserComments(id,skip, take));
        }

        #region POST
        /// <summary>
        /// Post a comment 
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
       // [Authorize]
        public HttpResponseMessage PostComment([FromBody]UserCommentModel userCommentModel)
        {
            string userId = User.Identity.UserId();
            string userName = User.Identity.Name;
            IUserComment userComment = new UserComment
            {
                MemeId = userCommentModel.MemeId,
                DateCreated = DateTime.Now.ToUniversalTime(),
                UserId = userId,
                UserName=userName,
                Likes = 0,
                Dislikes = 0,
                Comment = userCommentModel.Comment
            };
            userComment = repository.SaveUserComment(userComment);
            var jsonMediaTypeFormatter = new JsonMediaTypeFormatter
            {
                SerializerSettings =
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };
            var response = Request.CreateResponse(HttpStatusCode.Created, userComment, jsonMediaTypeFormatter);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/comments/" + userComment.Id);
            return response;
        }
        #endregion

        //#region DELETE
        //public Comment DeleteComment(int id)
        //{
        //    Comment comment;
        //    if (!repository.TryGet(id, out comment))
        //        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
        //    repository.Delete(id);
        //    return comment;
        //}
        //#endregion
        //
        //    #region Paging GET
        //    public IEnumerable<Comment> GetComments(int pageIndex, int pageSize)
        //    {
        //      return repository.Get().Skip(pageIndex * pageSize).Take(pageSize);
        //    }
        //    #endregion
    }
}