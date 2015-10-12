using System.Collections.Generic;
namespace SB004.Domain
{
    public class User: IUser
    {
        public User() 
        {
            FollowingIds = new List<string>();
            FavouriteMemeIds = new List<string>();
        }
        public string UserName { get; set; }

        public string Id { get; set; }

        public string AuthenticationUserId { get; set; }

        public string AuthenticationProvider { get; set; }

        public string Email { get; set; }

        public List<string> FollowingIds { get; set; }

        public bool Active { get; set; }

        public string Thumbnail { get; set; }

        public List<string> FavouriteMemeIds { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Favourites { get; set; }
        public int Shares { get; set; }
        public int Views { get; set; }
        public int Reposted { get; set; }
    }
}
