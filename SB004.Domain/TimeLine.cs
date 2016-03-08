using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB004.Domain
{
    public class TimeLine:ITimeLine
    {
        public string UserId { get; set; }
        public DateTime DateOfEntry { get; set; }
        public TimeLineEntry EntryType { get; set; }
        public string TimeLineRefId { get; set; }
    }
}
