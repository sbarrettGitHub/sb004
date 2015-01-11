using SB004.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SB004.Controllers
{
    public class SeedController : ApiController
    {
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

        // POST: api/Seed
        public void Post([FromBody]Seed seed)
        {
            var s = seed;
            
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
