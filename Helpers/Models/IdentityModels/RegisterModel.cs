using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.IdentityModels
{
    public class RegisterModel
    {
        public string email { get; set; }     
        public string username { get; set; }
        public string password { get; set; }
        public string ip { get; set; }
        public string port { get; set; }
    }
}
