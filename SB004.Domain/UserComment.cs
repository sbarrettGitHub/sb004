using System;
namespace SB004.Domain
{
    public class UserComment:IUserComment
    {
        public string Id { get; set; }
        public string MemeId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime DateCreated { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string Comment { get; set; }
    }
}