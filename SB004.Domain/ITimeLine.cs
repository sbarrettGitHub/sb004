
using System;
namespace SB004.Domain
{
    public enum TimeLineEntry {All, Post, Repost, Like, Dislike, Reply, Comment, StatusMessage, Follow, Unfollow}
    public interface ITimeLine
    {
		string Id { get; set; }
        string UserId { get; set; }
		string AlternateUserId { get; set; }
        DateTime DateOfEntry { get; set; }
        TimeLineEntry EntryType { get; set; }
        string TimeLineRefId { get; set; }
		string TimeLineRefAlternateId { get; set; }
		string Message { get; set; }
    }
}
