using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.KYCModels
{
    public class KYCCallBack
    {
        public string request_id { get; set; }
        public string type { get; set; }
        public string verification_id { get; set; }
        public string applicant_id { get; set; }
        public string status { get; set; }
        public bool verified { get; set; }
        public Verifications verifications { get; set; }
    }

    public class Verifications
    {
        public Document document { get; set; }
    }

    public class Document
    {
        public bool verified { get; set; }
        public string comment { get; set; }
        public List<object> decline_reasons { get; set; }
    }
}
