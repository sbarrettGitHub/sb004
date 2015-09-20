using System;
namespace SB004.Domain
{
    public interface IRepost
    {
        /// <summary>
        /// Repost id
        /// </summary>
        string Id { get; set; }
        
        /// <summary>
        /// ID of the meme being reposted
        /// </summary>
        string MemeId { get; set; }
        
        /// <summary>
        /// ID of the user who reposted
        /// </summary>
        string UserId { get; set; }
        
        /// <summary>
        /// Date of repost
        /// </summary>
        DateTime DateCreated { get; set; }
    }
}
