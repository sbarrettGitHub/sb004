using System;

namespace SB004.Domain
{
    public class TimeLine:ITimeLine
    {
	    public TimeLine()
	    {
	    }

	    public TimeLine(string userId, TimeLineEntry entryType, string timeLineRefId,
			string timeLineRefAlternateId, string message)
	    {
		    this.UserId = userId;
		    this.DateOfEntry = DateTime.Now.ToUniversalTime();
		    this.EntryType = entryType;
		    this.TimeLineRefId = timeLineRefId;
		    this.TimeLineRefAlternateId = timeLineRefAlternateId;
			this.Message = message;
	    }
		public string Id { get; set; }
	    public string UserId { get; set; }
        public DateTime DateOfEntry { get; set; }
        public TimeLineEntry EntryType { get; set; }
        public string TimeLineRefId { get; set; }
		public string TimeLineRefAlternateId { get; set; }
		public string Message { get; set; }
    }
}
