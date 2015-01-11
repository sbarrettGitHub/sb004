using System.Web.Http;
namespace SB004.Controllers
{


  public class CommentController : ApiController
  {
    public IHttpActionResult Get()
    {
      return this.Ok(new {id=1, text="Some comment"});
    }

//    #region POST
//    public HttpResponseMessage PostComment(Comment comment)
//    {
//      comment = repository.Add(comment);
//      var response = Request.CreateResponse<Comment>(HttpStatusCode.Created, comment);
//      response.Headers.Location = new Uri(Request.RequestUri, "/api/comments/" + comment.ID.ToString());
//      return response;
//    }
//    #endregion
//
//    #region DELETE
//    public Comment DeleteComment(int id)
//    {
//      Comment comment;
//      if (!repository.TryGet(id, out comment))
//        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
//      repository.Delete(id);
//      return comment;
//    }
//    #endregion
//
//    #region Paging GET
//    public IEnumerable<Comment> GetComments(int pageIndex, int pageSize)
//    {
//      return repository.Get().Skip(pageIndex * pageSize).Take(pageSize);
//    }
//    #endregion
  }
}