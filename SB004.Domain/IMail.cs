using System;
using System.Collections.Generic;

namespace SB004.Domain
{
    public interface IMail
    {
        string Id { get; set; }
        DateTime? DateSent { get; set; }
        DateTime DateAdded { get; set; }
        List<string> To { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        int SendAttempts{ get; set; }
        string SendError { get; set; }
    }
}
