using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SB004.Controllers
{
  using System;
  using System.Linq;
  using System.Net.Http.Headers;

  using SB004.Data;
  using SB004.Domain;
  using SB004.Models;
  using SB004.Utilities;

  public class MemeController : ApiController
  {
    readonly IRepository repository;
    public MemeController(IRepository repository)
    {
      this.repository = repository;
    }

    public HttpResponseMessage Get(string id)
    {
      HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
      result.Content = new ByteArrayContent(getImageBytes());
      result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
      return result;
    }
    /// <summary>
    /// POST: api/Meme
    /// Save the meme and generate seed id . 
    /// <param name="memeModel">Meme to add</param> 
    /// </summary>
    public HttpResponseMessage Post([FromBody]MemeModel memeModel)
    {
      IMeme meme = new Meme
      {
        SeedId = memeModel.SeedId,
        Comments = memeModel.Comments.Select(x=> (IComment) new Comment
        {
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
            Y = x.Position.Y
          },
          Text = x.Text,
          TextAlign = x.TextAlign,
          TextDecoration = x.TextDecoration,
          TextShadow = x.TextShadow,
        }).ToList(),
        ImageData = Convert.FromBase64String(memeModel.ImageData)
      };

      // Save the meme with the new image
      meme = repository.AddMeme(meme);
      
      var response = Request.CreateResponse(HttpStatusCode.Created, meme);
      response.Headers.Location = new Uri(Request.RequestUri, "/api/meme/" + meme.Id);
      return response;
    }
    private byte[] getImageBytes()
    {
      var webClient = new WebClient();
      byte[] imageBytes = webClient.DownloadData("http://www.google.com/images/logos/ps_logo2.png");
      return imageBytes;
    }
  }
}
