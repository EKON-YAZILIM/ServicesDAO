using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string NameSurname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Newsletter { get; set; }
        public string UserType { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public bool KYCStatus { get; set; }
        public int? FailedLoginCount { get; set; }
        public string ProfileImage { get; set; }
        public string UserName { get; set; }
        public string WalletAddress { get; set; }
        public string IBAN { get; set; }

    }
}
