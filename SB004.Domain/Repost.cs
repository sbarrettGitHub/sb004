using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB004.Domain
{
    public class Repost: IRepost
    {
        /// <summary>
        /// Repost id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// ID of the meme being reposted
        /// </summary>
        public string MemeId { get; set; }

        /// <summary>
        /// ID of the user who reposted
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Date of repost
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
