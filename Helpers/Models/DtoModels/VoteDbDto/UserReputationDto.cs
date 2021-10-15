using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.VoteDbDto
{
    [Serializable]
    public partial class UserReputationDto
    {
        public int UserReputationID { get; set; }
        public int UserId { get; set; }
        public double Reputation { get; set; }
    }
}
