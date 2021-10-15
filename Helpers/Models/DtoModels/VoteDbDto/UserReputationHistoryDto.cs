using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.VoteDbDto
{
    [Serializable]
    public partial class UserReputationHistoryDto
    {
        public int UserReputationHistoryID { get; set; }
        public DateTime Date { get; set; }
        public double Input { get; set; }
        public double Output { get; set; }
        public double LastTotal { get; set; }
        public string Explanation { get; set; }
        public int UserReputationID { get; set; }
    }
}
