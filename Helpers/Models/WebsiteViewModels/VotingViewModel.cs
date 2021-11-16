using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.DtoModels.VoteDbDto;
using System;
using System.Collections.Generic;
using System.Text;
using static Helpers.Constants.Enums;

namespace Helpers.Models.WebsiteViewModels
{
    public class VotingViewModel
    {
        public int JobID { get; set; }
        public bool IsFormal { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public VoteStatusTypes Status { get; set; }
        public int VotingID { get; set; }
        public double? StakedFor { get; set; }
        public double? StakedAgainst { get; set; }
        public int VoteCount { get; set; }
        public int? QuorumCount { get; set; }

    }
}
