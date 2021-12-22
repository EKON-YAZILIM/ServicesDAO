using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Models
{
    public class JobPostComment
    {
        [Key]
        public int JobPostCommentID { get; set; }
        public int JobID { get; set; }
        public int UserID { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public int SubCommentID { get; set; }
        public int UpVote { get; set; }
        public int DownVote { get; set; }
        public bool? IsPinned { get; set; }
        public bool? IsFlagged { get; set; }
    }
}
