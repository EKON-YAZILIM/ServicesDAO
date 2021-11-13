using System;
using System.Collections.Generic;
using System.Text;
using static Helpers.Constants.Enums;

namespace Helpers.Models.DtoModels.ReputationDbDto
{
    public class UserReputationStakeDto
    {
        public int UserReputationStakeID { get; set; }
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int? ReferenceID { get; set; } //Id of the bid or the vote (can be anything else), ReputationStakeTypes indicates the type of this stake
        public int? ReferenceProcessID { get; set; } //Id of the auction or the voting (can be anything else), indicates the process which this stake belongs to
        public double Amount { get; set; }
        public StakeType Type { get; set; }
        public ReputationStakeStatus Status { get; set; }

    }
}
