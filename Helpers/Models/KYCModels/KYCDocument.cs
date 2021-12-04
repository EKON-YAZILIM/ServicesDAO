using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.KYCModels
{
    public class KYCDocument
    {
        public string applicant_id { get; set; }
        public string type { get; set; }
        public string document_number { get; set; }
        public string issue_date { get; set; }
        public string expiry_date { get; set; }
        public string front_side_id { get; set; }
        public string back_side_id { get; set; }

    }
}
