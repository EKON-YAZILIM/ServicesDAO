using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.IdentityModels
{
    public class RegisterModel
    {
        public string email { get; set; }     
        public string namesurname { get; set; }
        public string gsm { get; set; }
        public string password { get; set; }
        public string registerEmailTitle { get; set; }
        public string registerEmailContent { get; set; }
    }
}
