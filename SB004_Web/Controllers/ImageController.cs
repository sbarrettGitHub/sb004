using SB004.Data;
using SB004.Domain;
using SB004.Utilities;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
namespace SB004.Controllers
{
   

  [RoutePrefix("api/image")]
  public class ImageController : ApiController
  {
    readonly IRepository repository;
    readonly IImageManager imageManager;
    public ImageController(IRepository repository, IImageManager imageManager)
    {
      this.repository = repository;
      this.imageManager = imageManager;
    }
    /// <summary>
    /// Returns an image representing a meme, only
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Route("{id}")]
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
    /// <summary>
    /// Get a users image
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Route("user/{id}")]
    public HttpResponseMessage GetUserImage(string id)
    {
        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        IImage userImage = repository.GetImage(id);
        if (userImage == null)
        {
            userImage = new Image();
            userImage.ImageData = imageManager.GetImageData(Url.Content("~/Content/Images/avatar.GIF"));             
        }
        result.Content = new ByteArrayContent(userImage.ImageData);
        result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        return result;
    }
    
  }
}
