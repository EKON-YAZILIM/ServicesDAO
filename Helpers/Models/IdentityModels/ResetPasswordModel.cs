using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.IdentityModels
{
    public class ResetPasswordModel
    {
        public string email { get; set; }
        public string forgotPassEmailTitle { get; set; }
        public string forgotPassEmailContent { get; set; }
    }
}
