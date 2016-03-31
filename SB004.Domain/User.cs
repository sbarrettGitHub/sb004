using System.Collections.Generic;
namespace SB004.Domain
{
    public class UserLite: IUserLite
    {
        public string UserName { get; set; }

        public string Id { get; set; }
    }
    public class User: IUser
    {
        private List<IUser> following;
        private List<IUser> followedBy;
        public User() 
        {
            FollowingIds = new List<IUserLite>();
            FollowedByIds = new List<IUserLite>();
            FavouriteMemeIds = new List<string>();
        }
        public string UserName { get; set; }

        public string Id { get; set; }

        public string AuthenticationUserId { get; set; }

        public string AuthenticationProvider { get; set; }

        public string Email { get; set; }
        
		public string StatusMessage { get; set; }

        public List<IUserLite> FollowingIds { get; set; }

        public List<IUser> Following
        {
            get
            {
                return this.following;
            }
        }

		public List<IUserLite> FollowedByIds { get; set; }
		public List<IUser> FollowedBy
		{
			get
			{
				return this.followedBy;
			}
		}
        public bool Active { get; set; }

        public string Thumbnail { get; set; }

        public List<string> FavouriteMemeIds { get; set; }
		
		public int Posts { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Favourites { get; set; }
        public int Shares { get; set; }
        public int Views { get; set; }

		public int Replies { get; set; }
        public int Reposts { get; set; }
		public int Comments { get; set; }
		public int Reposted { get; set; }
        public IUser SetFollowing(List<IUser> f)
        {
            this.following = f;
            return this;
        }
		public IUser SetFollowedBy(List<IUser> f)
		{
			this.followedBy = f;
			return this;
		}
    }
    public class Credentials : ICredentials
    {
        public string Id { get; set; }

        public string Email{ get; set; }

        public string Password { get; set; }
        public string Salt { get; set; }

    }
}
