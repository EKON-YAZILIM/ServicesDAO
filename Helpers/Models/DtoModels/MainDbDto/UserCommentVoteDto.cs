using System;
using System.Collections.Generic;


namespace Helpers.Models.DtoModels.MainDbDto
{
    [Serializable]
    public partial class UserCommentVoteDto
    {
        public int UserCommentVoteID { get; set; }
        public int UserId { get; set; }
        public int JobPostCommentID { get; set; }
        public bool IsUpVote { get; set; }
    }
}
