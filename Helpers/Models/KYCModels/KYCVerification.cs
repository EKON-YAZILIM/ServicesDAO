using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.KYCModels
{
    public class KYCVerification
    {
        public string applicant_id { get; set; }
        public string form_id { get; set; }
        public List<string> types { get; set; }

    }
}
