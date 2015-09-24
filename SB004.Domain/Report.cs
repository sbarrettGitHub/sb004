using System;

namespace SB004.Domain
{
    public class Report : IReport
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

        /// <summary>
        /// Reason for objection
        /// </summary>
        public string Objection { get; set; }

        /// <summary>
        /// ID of the user who moderated
        /// </summary>
        public string ModeratedByUserId { get; set; }

        /// <summary>
        /// Moderators judgement
        /// </summary>
        public string ModeratorJudgement { get; set; }

        /// <summary>
        /// Date of moderation
        /// </summary>
        public DateTime DateModerated { get; set; }
    }
}
