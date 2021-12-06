using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.KYCModels
{
    public class KYCCountries
    {
        public string country_code { get; set; }
        public List<Name> labels { get; set; }
    }

    public class Name
    {
        public string language_code { get; set; }
        public string label { get; set; }
    }
}
