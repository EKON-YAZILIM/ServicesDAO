using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Models
{
    public class UserKYC
    {
        [Key]
        public int UserKYCID { get; set; }
        public int UserID { get; set; }
        public string UserType { get; set; }
        public string FileId1 { get; set; }
        public string FileId2 { get; set; }
        public string ApplicantId { get; set; }
        public string VerificationId { get; set; }
        public string DocumentId { get; set; }
        public string KYCStatus { get; set; }
        public string Comment { get; set; }
        public bool Verified { get; set; }

    }
}
