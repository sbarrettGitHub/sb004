using System;
namespace SB004.Domain
{
    public interface IReport
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

        /// <summary>
        /// Rason for objection
        /// </summary>
        string Objection { get; set; }
        /// <summary>
        /// ID of the user who moderated
        /// </summary>
        string ModeratedByUserId { get; set; }

        /// <summary>
        /// Moderators judgement
        /// </summary>
        string ModeratorJudgement { get; set; }

        /// <summary>
        /// Date of moderation
        /// </summary>
        DateTime DateModerated { get; set; }
    }
}
