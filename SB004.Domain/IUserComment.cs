using System;
namespace SB004.Domain
{
    public interface IUserComment
    {
        string Id { get; set; }
        string MemeId { get; set; }
        string UserId { get; set; }
        string UserName { get; set; }
        DateTime DateCreated { get; set; }
        int Likes { get; set; }
        int Dislikes { get; set; }
        string Comment { get; set; }
    }
}
