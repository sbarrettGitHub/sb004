namespace SB004.Controllers
{
  using System.Net.Http.Headers;

  using SB004.Utilities;
  using SB004.Models;
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Web.Http;
  using SB004.Data;
  using SB004.Domain;

  /// <summary>
  /// A seed is an image from which an Meme is created. 
  /// </summary>
  public class SeedController : ApiController
  {
    readonly IRepository repository;
    readonly IImageManager imageManager;
    public SeedController(IRepository repository, IImageManager imageManager)
    {
      this.repository = repository;
      this.imageManager = imageManager;
    }

    // GET: api/Seed
    public IHttpActionResult Get()
    {
      return this.NotFound();
    }

    // GET: api/Seed/5
    public HttpResponseMessage Get(string id)
    {
      HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
      ISeed seed = repository.GetSeed(id);
      if (seed == null)
      {
        return this.Request.CreateResponse(HttpStatusCode.NotFound, "Invalid ID");
      }
      result.Content = new ByteArrayContent(seed.ImageData);
      result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
      return result;
    }

    /// <summary>
    /// POST: api/Seed
    /// Save the seed image and generate seed id . Resuse exisiting seed if already added
    /// <param name="seedModel">Seed to add</param>
    /// </summary>
    public HttpResponseMessage Post([FromBody]SeedModel seedModel)
    {
      ISeed seed = new Seed
      {
        SourceImageUrl = seedModel.image.IndexOf("http", StringComparison.Ordinal) >= 0 ? seedModel.image : "",
        Height = seedModel.height,
        Width = seedModel.width
      };

      // Get the bytes of the image (Download from URL or load from base 64 data)
      seed.ImageData = imageManager.GetImageData(seedModel.image);

      // Check if seed already exists by generating a hash and checking against the repository
      seed.ImageHash = imageManager.ImageHash(seed.ImageData, seedModel.width, seedModel.height);

      // Check this seed image already exists
      var existingSeed = repository.GetSeedByHash(seed.ImageHash);

      // Add the seed if it does not already exist, otherwose continue with the existing seed image
      if (existingSeed == null)
      {
        // Prime the image
        seed = imageManager.PrimeSeed(seed);

        // Save the image
        seed = repository.AddSeed(seed);
      }
      else
      {
        seed = existingSeed;
      }

      var response = Request.CreateResponse(HttpStatusCode.Created, new SeedModel
      {
        id = seed.Id,
        image = "data:image/jpg;base64," + Convert.ToBase64String(seed.ImageData,
          0,
          seed.ImageData.Length)
      });
      response.Headers.Location = new Uri(Request.RequestUri, "/api/seed/" + seed.Id);
      return response;
    }
    // PUT: api/Seed/5
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE: api/Seed/5
    public void Delete(int id)
    {
    }
  }
}
