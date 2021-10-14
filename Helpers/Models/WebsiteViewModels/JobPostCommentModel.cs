using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
   public class JobPostCommentModel
    {
        public string ProfileImage { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public int SubCommentID { get; set; }
        public int UpVote { get; set; }
        public int DownVote { get; set; }
        public int Points { get; set; }
    }
}
