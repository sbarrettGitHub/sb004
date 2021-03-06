﻿using System;
using System.Collections.Generic;
using System.Net;
using SB004.Domain;

namespace SB004.Models
{
	public class TimelineModel
	{
		public IUser User { get; set; }
		public List<TimelineEntryModel> TimelineEntries { get; set; } 
	}
	public class TimelineGroup
	{
		public MemeLiteModel Meme { get; set; }
		public DateTime TimeStamp { get; set; }
		public List<TimelineEntryModel> TimelineEntries { get; set; }
		public bool HasMore { get; set; }
	}

	public class TimelineGroupModel
	{
		public IUser User { get; set; }
		public List<TimelineGroup> TimelineGroups { get; set; } 
	}

	public class TimeLineUserCommentModel : UserComment
	{
		public TimeLineUserCommentModel(IUserComment userComment, IUser user)
		{
			this.Id = userComment.Id;
			this.MemeId = userComment.MemeId;
			this.UserId = userComment.UserId;
			this.UserName = userComment.UserName;
			this.DateCreated = userComment.DateCreated;
			this.Likes = userComment.Likes;
			this.Dislikes = userComment.Dislikes;
			this.Comment = WebUtility.HtmlEncode(userComment.Comment);
			this.User = user;
		}
		public IUser User { get; set; }
	}

	public class TimelineEntryModel
	{
		public TimelineEntryModel()
		{
		}
		public TimelineEntryModel(ITimeLine timeLineEntry)
		{
			this.TimelineId = timeLineEntry.Id;
			this.DateOfEntry = timeLineEntry.DateOfEntry;
			this.UserId = timeLineEntry.UserId;
			this.EntryType = timeLineEntry.EntryType;
			this.TimeLineRefId = timeLineEntry.TimeLineRefId;
			this.TimeLineRefAlternateId = timeLineEntry.TimeLineRefAlternateId;
			this.Message = timeLineEntry.Message;
		}
		public string TimelineId { get; set; }
		public string UserId { get; set; }
		public string TimeLineRefId { get; set; }
		public string TimeLineRefAlternateId { get; set; }
		public string Message { get; set; }
		public TimeLineEntry EntryType { get; set; }

		public DateTime DateOfEntry { get; set; }
		public MemeLiteModel Meme { get; set; }
		public MemeLiteModel AlternateMeme { get; set; }
		public IUser User { get; set; }
		public TimeLineUserCommentModel UserComment { get; set; } 

	}
}