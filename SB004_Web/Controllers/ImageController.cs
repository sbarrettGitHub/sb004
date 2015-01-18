using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace SB004.Controllers
{
    public class ImageController : ApiController
    {
        // GET: api/Image
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Image/5
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(getImageBytesFromCache());
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
        private byte[] getImageBytesFromCache()
        {
            byte[] imageBytes = (byte[])HttpContext.Current.Cache["test"];
           
            return imageBytes;
        }
        private byte[] getImageBytes()
        {
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData("http://www.google.com/images/logos/ps_logo2.png");
            return imageBytes;
        }
    }
}
