using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.VoteDbDto
{
    [DataContract]
    [Serializable]
    public partial class UserReputationDto
    {
        [DataMember]
        public int UserReputationID { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public double Reputation { get; set; }
    }
}
