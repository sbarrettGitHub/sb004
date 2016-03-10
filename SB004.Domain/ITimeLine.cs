
using System;
namespace SB004.Domain
{
    public enum TimeLineEntry { Post, Repost, Like, Dislike, Reply, Comment, StatusMessage}
    public interface ITimeLine
    {
        string UserId { get; set; }
        DateTime DateOfEntry { get; set; }
        TimeLineEntry EntryType { get; set; }
        string TimeLineRefId { get; set; }
		string TimeLineRefAlternateId { get; set; }
		string Message { get; set; }
    }
}
