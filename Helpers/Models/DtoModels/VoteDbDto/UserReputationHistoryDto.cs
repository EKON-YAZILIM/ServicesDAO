using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.VoteDbDto
{
    [DataContract]
    [Serializable]
    public partial class UserReputationHistoryDto
    {
        [DataMember]
        public int UserReputationHistoryID { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public double Input { get; set; }

        [DataMember]
        public double Output { get; set; }

        [DataMember]
        public double LastTotal { get; set; }

        [DataMember]
        public string Explanation { get; set; }

        [DataMember]
        public int UserReputationID { get; set; }
    }
}
