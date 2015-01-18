using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SB004.Business;
using SB004.Domain;
namespace SB004.Data
{
    public class Repository : IRepository
    {
        IdManager idManager { get; set; }

        public Repository(IdManager idManager)
        {
            this.idManager = idManager;
        }

        public ISeed AddSeed(ISeed seed)
        {
            // Generate a seed id
            string id = idManager.CreateGuid();

            // Save the seed

            // Convert the id to base 64
            seed.id = idManager.EncodeGuid(id);
            
            return seed;
        }
    }
}