using MongoDB.Bson;
using SB004.Domain;

namespace SB004.Data.Entities
{
    public class MemeEntity
    {
        public MemeEntity(IMeme meme)
        {
            this.SeedId = meme.SeedId;
            this.ImageData = meme.ImageData;
            this.DateCreated = meme.DateCreated;
            this.CreatedBy = meme.CreatedBy;
            this.Comments = (Comment[])meme.Comments;
        }
        public ObjectId Id { get; set; }
        public string SeedId { get; set; }
        public byte[] ImageData { get; set; }
        public BsonDateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public Comment[] Comments { get; set; }       
    }
}
