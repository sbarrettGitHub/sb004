using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace SB004.Controllers
{
  using SB004.Data;
    using SB004.Domain;

  public class ImageController : ApiController
  {
    readonly IRepository repository;

    public ImageController(IRepository repository)
    {
      this.repository = repository;
    }

    // GET: api/Image
    public IEnumerable<string> Get()
    {
      return new[] { "value1", "value2" };
    }

    // GET: api/Image/5
    public HttpResponseMessage Get(string id)
    {
      HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
      IMeme meme = repository.GetMeme(id);
      if (meme == null)
      {
        return this.Request.CreateResponse(HttpStatusCode.NotFound, "Invalid ID");
      }
      result.Content = new ByteArrayContent(meme.ImageData);
      result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
      return result;
    }

    // POST: api/Image
    public void Post([FromBody]string value)
    {
    }

    // PUT: api/Image/5
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE: api/Image/5
    public void Delete(int id)
    {
    }
  }
}
