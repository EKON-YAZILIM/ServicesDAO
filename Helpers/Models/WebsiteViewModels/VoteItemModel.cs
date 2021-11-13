using System;
using System.Collections.Generic;
using System.Text;
using static Helpers.Constants.Enums;

namespace Helpers.Models.WebsiteViewModels
{
    public class VoteItemModel
    {
        public int VoteID { get; set; }
        public int VotingID { get; set; }
        public int UserID { get; set; }
        public DateTime Date { get; set; }
        public StakeType Direction { get; set; }
        public double ReputationStake { get; set; }
        public string UserName { get; set; }
    }
}
