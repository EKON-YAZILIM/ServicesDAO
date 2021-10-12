using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Helpers.Models.DtoModels.MainDbDto
{
    [DataContract]
    [Serializable]
    public partial class JobPostDto
    {
        [DataMember]
        public int JobID { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public int UserID { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string JobDescription { get; set; }

        [DataMember]
        public double Amount { get; set; }

        [DataMember]
        public double DosPaid { get; set; }

        [DataMember]
        public string TimeFrame { get; set; }
    }
}
