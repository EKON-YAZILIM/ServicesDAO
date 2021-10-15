using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Helpers.Models.DtoModels.VoteDbDto
{
    [Serializable]
    public partial class VoteDto
    {
        public int VoteId { get; set; }
        public int VoteJobID { get; set; }
        public double Reputation { get; set; }
        public string Side { get; set; }
        public DateTime Date { get; set; }
    }
}
