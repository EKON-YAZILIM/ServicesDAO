using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static Helpers.Constants.Enums;

namespace Helpers.Models.DtoModels.VoteDbDto
{
    [Serializable]
    public partial class VotingDto
    {
        public int VotingID { get; set; }
        public int JobID { get; set; }
        public bool IsFormal { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public VoteStatusTypes Status { get; set; }
        public int? QuorumCount { get; set; }
        public VoteTypes Type { get; set; }
        public int VoteCount { get; set; }
        public double? StakedFor { get; set; }
        public double? StakedAgainst { get; set; }

        //How much of the new minted reputation will be distributed to job doer.
        public double ReputationDistributionRatio { get; set; }

    }
}
