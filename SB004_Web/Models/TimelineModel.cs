using System;
using SB004.Domain;

namespace SB004.Models
{
	public class TimelineModel : ITimeLine
	{
		public string UserId { get; set; }
		
		public string TimeLineRefId { get; set; }
		public string TimeLineRefAlternateId { get; set; }
		public string Message { get; set; }

		public TimeLineEntry EntryType { get; set; }
		public DateTime DateOfEntry { get; set; }
		public MemeLiteModel Meme { get; set; }
		public MemeLiteModel AlternateMeme { get; set; }
		public IUser User { get; set; } 
		public IUserComment UserComment{ get; set; } 

	}
}