using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.KYCModels
{
    public class KYCVerification
    {
        public string applicant_id { get; set; }
        public List<string> types { get; set; }
        public string callback_url { get; set; }

    }
}
