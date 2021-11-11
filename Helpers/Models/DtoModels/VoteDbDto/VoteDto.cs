using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Helpers.Models.DtoModels.VoteDbDto
{
    [Serializable]
    public partial class VoteDto
    {
        public int VoteID { get; set; }
        public int VotingID { get; set; }
        public int UserID { get; set; }
        public double Reputation { get; set; }
        public string Side { get; set; }
        public DateTime Date { get; set; }
    }
}
