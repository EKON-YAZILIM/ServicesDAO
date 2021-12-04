using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;


namespace Helpers.Models.KYCModels
{
    public class KYCFileUpload
    {
        public string Type { set; get; }
        public string Name { set; get; }
        public string Surname { set; get; }
        public string DoB { set; get; }
        public string Email { set; get; }
        public string Country { set; get; }
        public string DocumentNUmber { set; get; }
        public string IssueDate { set; get; }
        public string ExpiryDate { set; get; }
        public int UserID { set; get; }


        public IFormFile UploadedFile1 { set; get; }
        public IFormFile UploadedFile2 { set; get; }

    }
}
