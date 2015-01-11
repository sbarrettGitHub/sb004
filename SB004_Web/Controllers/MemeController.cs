using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SB004.Controllers
{
  using System.Net.Http.Headers;

  public class MemeController : ApiController
    {
      public HttpResponseMessage Get(string id)
      {
        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        result.Content = new ByteArrayContent(getImageBytes());
        result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        return result;
      }
      private byte[] getImageBytes()
      {
        var webClient = new WebClient();
        byte[] imageBytes = webClient.DownloadData("http://www.google.com/images/logos/ps_logo2.png");
        return imageBytes;
      }
    }
}
