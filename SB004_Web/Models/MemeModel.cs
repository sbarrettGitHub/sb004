using SB004.Data;

namespace SB004.Models
{
    using SB004.Domain;
    using System;
    using System.Collections.Generic;

	
	public class PositionRefModel : IPositionRef
    {
        public string Align { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Padding { get; set; }
    }
    public class CommentModel
    {
        public int Id { get; set; }
        public PositionRefModel Position;
        public string Text { get; set; }
        public string Color { get; set; }
        public string BackgroundColor { get; set; }
        public string FontFamily { get; set; }
        public string FontSize { get; set; }
        public string FontWeight { get; set; }
        public string TextDecoration { get; set; }
        public string FontStyle { get; set; }
        public string TextAlign { get; set; }
        public string TextShadow { get; set; }
    }

    public class MemeModel
    {
        public string Id { get; set; }
        public string SeedId { get; set; }
        public CommentModel[] Comments { get; set; }
        public string ImageData { get; set; }
        public string ResponseToId { get; set; }
        public List<string> ReplyIds { get; set; }
        public List<string> HashTags { get; set; }
    }
    public class UserCommentModel
    {
        public string MemeId { get; set; }
        public string Comment { get; set; }
    }
    public class MemeLiteModel
    {
		public MemeLiteModel(IRepository repository, IMeme meme)
		{
			Id = meme.Id;
			CreatedBy = meme.CreatedBy;
			CreatedByUserId = meme.CreatedByUserId;
			Creator = meme.Creator;
			DateCreated = meme.DateCreated.ToLocalTime();
			ResponseToId = meme.ResponseToId;
			replyCount = meme.ReplyIds.Count;
			userCommentCount = repository.GetUserCommentCount(meme.Id);
			Likes = meme.Likes;
			Dislikes = meme.Dislikes;
			Favourites = meme.Favourites;
			Shares = meme.Shares;
			Views = meme.Views;
			Reposts = meme.Reposts;
			RepostOfId = meme.RepostOfId;
			HashTags = meme.HashTags;
		}
	    public string Id { get; set; }
        public string CreatedByUserId { get; set; }
        public IUser Creator { get; set; } 
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public long userCommentCount { get; set; }
        public string ResponseToId { get; set; }
		public long replyCount { get; set; }
		public long Likes { get; set; }
		public long Dislikes { get; set; }
		public long Favourites { get; set; }
		public long Shares { get; set; }
		public long Views { get; set; }
		public long Reposts { get; set; }
		public string RepostOfId { get; set; }
		public List<string> HashTags { get; set; }
		
    }
    public class ReportModel 
    {
        public string Objection { get; set; }
    }
    public class UserMemeModel 
    {
        public IUser User { get; set; }
        public List<MemeLiteModel> Memes { get; set; }
        public long FullMemeCount { get; set; }
    }

}