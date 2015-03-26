using System.Collections.Generic;
namespace SB004.Domain
{
    public class User: IUser
    {
        public string UserName { get; set; }

        public string Id { get; set; }

        public string AuthenticationUserId { get; set; }

        public string AuthenticationProvider { get; set; }

        public string Email { get; set; }

        public List<string> FollowingIds { get; set; }

        public bool Active { get; set; }

        public string Thumbnail { get; set; }

    }
}
