using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.IdentityModels
{
    public class ResetCompleteModel
    {
        public string newPass { get; set; }
        public string passwordChangeToken { get; set; }
    }
}
