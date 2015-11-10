using System.Collections.Generic;
namespace SB004.Domain
{
    public interface IUserLite 
    {
        string UserName { get; set; }

        string Id { get; set; } 
    }
    public interface ICredentials
    {
        string Id { get; set; }

        string Email { get; set; }

        // Store as hash
        string Password { get; set; }

        string Salt { get; set; }
    }
    public interface IUser
    {
        string UserName { get; set; }

        string Id { get; set; }

        string AuthenticationUserId { get; set; }

        string AuthenticationProvider { get; set; }

        string Email { get; set; }

        List<IUserLite> FollowingIds { get; set; }

        bool Active { get; set; }

        string Thumbnail { get; set; }

        List<string> FavouriteMemeIds { get; set; }

        int Likes { get; set; }
        int Dislikes { get; set; }
        int Favourites { get; set; }
        int Shares { get; set; }
        int Views { get; set; }
        int Reposted { get; set; }
    }
    
}
