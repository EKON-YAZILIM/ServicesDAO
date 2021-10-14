using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Models
{
    public class UserCommentVote 
    {
        [Key]
        public int UserCommentVoteID { get; set; }
        public int UserId { get; set; }
        public int JobPostCommentID { get; set; }
        public bool IsUpVote { get; set; }

    }
}
