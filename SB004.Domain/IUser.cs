using System.Collections.Generic;
namespace SB004.Domain
{
    public interface IUser
    {
        string UserName { get; set; }

        string Id { get; set; }

        string AuthenticationUserId { get; set; }

        string AuthenticationProvider { get; set; }

        string Email { get; set; }

        List<string> FollowingIds { get; set; }

        bool Active { get; set; }

        string Thumbnail { get; set; }

        List<string> FavouriteMemeIds { get; set; }
    }
}
