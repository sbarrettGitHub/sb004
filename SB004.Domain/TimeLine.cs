using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB004.Domain
{
    public class TimeLine:ITimeLine
    {
	    public TimeLine()
	    {
	    }

	    public TimeLine(string userId, TimeLineEntry entryType, string timeLineRefId,
		    string timeLineRefAlternateId)
	    {
		    this.UserId = userId;
		    this.DateOfEntry = DateTime.Now.ToUniversalTime();
		    this.EntryType = entryType;
		    this.TimeLineRefId = timeLineRefId;
		    this.TimeLineRefAlternateId = timeLineRefAlternateId;
	    }

	    public string UserId { get; set; }
        public DateTime DateOfEntry { get; set; }
        public TimeLineEntry EntryType { get; set; }
        public string TimeLineRefId { get; set; }
		public string TimeLineRefAlternateId { get; set; }
    }
}
