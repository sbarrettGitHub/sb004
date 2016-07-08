using System;
using System.Collections.Generic;

namespace SB004.Domain
{
    public class Mail:IMail
    {
        public Mail()
        {
            this.To = new List<string>();
        }

        public Mail(string address, string subject, string body)
        {
            this.To = new List<string>();
            this.To.Add(address);
            this.Subject = subject;
            this.Body = body;
        }

        public string Id { get; set; }
        public DateTime? DateSent { get; set; }
        public DateTime DateAdded { get; set; }
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int SendAttempts { get; set; }
        public string SendError { get; set; }
    }
}