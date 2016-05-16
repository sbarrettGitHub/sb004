using System;

namespace SB004.Domain
{
	public interface ITrendFactors
	{
		DateTime DateCreated { get; set; }
		long Likes { get; set; }
		long Dislikes { get; set; }
		long Favourites { get; set; }
		long Shares { get; set; }
		long Views { get; set; }
		long Reposts { get; set; }
		long ReportCount { get; set; }
		long UserCommentCount { get; set; }
		long ReplyCount { get; set; }
	}
}
