using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Helpers.Models.DtoModels.MainDbDto
{
    [DataContract]
    [Serializable]
    public partial class JobPostCommentDto
    {
        [DataMember]
        public int JobPostCommentID { get; set; }

        [DataMember]
        public int JobID { get; set; }

        [DataMember]
        public int UserID { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public int SubCommentID { get; set; }
    }
}
