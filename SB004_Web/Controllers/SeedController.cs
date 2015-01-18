using SB004.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SB004.Data;
using SB004.Domain;
using SB004.Business;
using System.Web;

namespace SB004.Controllers
{
    public class SeedController : ApiController
    {
        IRepository repository;
        IImageManager imageManager;
        public SeedController(IRepository repository, IImageManager imageManager)
        {
            this.repository = repository;
            this.imageManager = imageManager;
        }
        // GET: api/Seed
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Seed/5
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// POST: api/Seed
        /// Save the seed image and generate seed id 
        /// </summary>
        /// <param name="seed">Seed id</param>
        public HttpResponseMessage Post([FromBody]SeedModel seedModel)
        {
            ISeed seed = new Seed 
            { 
                imageUrl = seedModel.image, 
                sourceImageUrl = seedModel.image, 
                height = seedModel.height, 
                width = seedModel.width 
            };

            // Prime the image
            seed = imageManager.PrimeSeed(seed);

            HttpContext.Current.Cache.Insert("test", seed.imageData);

            // Save the image
            seed = repository.AddSeed(seed);

            seedModel.id = seed.id;
            seedModel.image = seed.imageUrl;

            var response = Request.CreateResponse<SeedModel>(HttpStatusCode.Created, seedModel);
            response.Headers.Location = new Uri(Request.RequestUri, "/api/seed/" + seed.id);
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
