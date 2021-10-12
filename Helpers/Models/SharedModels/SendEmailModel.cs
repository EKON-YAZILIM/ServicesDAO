using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.SharedModels
{
    /// <summary>
    ///  Email notification model 
    /// </summary>
    public class SendEmailModel
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<string> To { get; set; } = new List<string>();
        public List<string> Cc { get; set; } = new List<string>();
        public List<string> Bcc { get; set; } = new List<string>();
    }
}
