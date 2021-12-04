using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Helpers.Models.DtoModels.MainDbDto
{
    [Serializable]
    public partial class UserKYCDto
    {
        public int UserKYCID { get; set; }
        public int UserID { get; set; }
        public string UserType { get; set; }
        public string FileId1 { get; set; }
        public string FileId2 { get; set; }
        public string ApplicantId { get; set; }
        public string VerificationId { get; set; }
        public string KYCStatus { get; set; }
        public string Comment { get; set; }
        public bool Verified { get; set; }

    }
}
