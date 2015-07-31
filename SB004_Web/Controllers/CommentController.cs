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

  [RoutePrefix("api/comment")]
  public class CommentController : ApiController
  {
    readonly IRepository repository;
    public CommentController(IRepository repository)
    {
      this.repository = repository;
    }
    /// <summary>
    /// Retrieve the user added comments for a specified meme id
    /// </summary>
    /// <param name="id">Meme id that the user added comments have been added to</param>
    /// <param name="skip">for pagination</param>
    /// <param name="take">for pagination</param>
    /// <returns></returns>
    [Route("{id}")]
    public IHttpActionResult Get(string id, int skip, int take)
    {
      long fullCommentCount = repository.GetUserCommentCount(id);
      var userComments = repository.GetUserComments(id, skip, take);

      return this.Ok(new { fullCommentCount, userComments });
    }

    #region POST
    /// <summary>
    /// Post a comment 
    /// </summary>
    /// <param name="comment"></param>
    /// <returns></returns>
    // [Authorize]
    [Route("")]
    public HttpResponseMessage PostComment([FromBody]UserCommentModel userCommentModel)
    {
      string userId = User.Identity.UserId();
      string userName = User.Identity.Name;
      IUserComment userComment = new UserComment
      {
        MemeId = userCommentModel.MemeId,
        DateCreated = DateTime.Now.ToUniversalTime(),
        UserId = userId,
        UserName = userName,
        Likes = 0,
        Dislikes = 0,
        Comment = userCommentModel.Comment != null
                  ? userCommentModel.Comment.Length > 1000
                        ? userCommentModel.Comment.Substring(0, 1000)
                        : userCommentModel.Comment
                  : ""
      };
      userComment = repository.Save(userComment);

      var response = Request.CreateResponse(HttpStatusCode.Created, userComment);
      response.Headers.Location = new Uri(Request.RequestUri, "/api/comments/" + userComment.Id);
      return response;
    }
    #endregion
    #region PATCH

    /// <summary>
    /// Like a comment
    /// </summary>
    /// <param name="id">Comment id</param>
    /// <returns></returns>
    //[Authorize]
    [HttpPatch]
    [Route("{id}/like/")]
    public HttpResponseMessage LikeComment(string id)
    {
      IUserComment userComment = repository.GetUserComment(id);
      if (userComment == null)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
      }
      userComment.Likes++;
      userComment = repository.Save(userComment);

      var response = Request.CreateResponse(HttpStatusCode.Created, userComment);
      response.Headers.Location = new Uri(Request.RequestUri, "/api/comments/" + userComment.Id);
      return response;
    }

    /// <summary>
    /// Dislike a comment
    /// </summary>
    /// <param name="id">Comment id</param>
    /// <returns></returns>
    // [Authorize]
    [HttpPatch]
    [Route("{id}/dislike/")]
    public HttpResponseMessage DislikeComment(string id)
    {
      IUserComment userComment = repository.GetUserComment(id);
      if (userComment == null)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
      }
      userComment.Dislikes++;
      userComment = repository.Save(userComment);

      var response = Request.CreateResponse(HttpStatusCode.Created, userComment);
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