using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Helpers.Models.DtoModels.VoteDbDto
{
    [DataContract]
    [Serializable]
    public partial class VoteDto
    {
        [DataMember]
        public int VoteId { get; set; }

        [DataMember]
        public int VoteJobID { get; set; }

        [DataMember]
        public double Reputation { get; set; }

        [DataMember]
        public string Side { get; set; }

        [DataMember]
        public DateTime Date { get; set; }
    }
}
