﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class JobPostCommentModel
    {
        public int UserID { get; set; }
        public int JobPostCommentID { get; set; }
        public string ProfileImage { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public int SubCommentID { get; set; }
        public int UpVote { get; set; }
        public int DownVote { get; set; }
        public int Points { get; set; }
        public bool? IsUpVote { get; set; }
        public bool IsUsersComment { get; set; }
        public bool? IsPinned { get; set; }
        public bool? IsFlagged { get; set; }
        public double? UserReputation { get; set; }
    }
}
