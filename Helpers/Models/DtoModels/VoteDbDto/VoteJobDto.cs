using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Helpers.Models.DtoModels.VoteDbDto
{
    [DataContract]
    [Serializable]
    public partial class VoteJobDto
    {
        [DataMember]
        public int VoteJobID { get; set; }

        [DataMember]
        public int JobID { get; set; }

        [DataMember]
        public bool IsFormal { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }
    }
}
