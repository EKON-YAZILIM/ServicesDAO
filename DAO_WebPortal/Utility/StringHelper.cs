using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_WebPortal.Utility
{
    public static class StringHelper
    {
        /// <summary>
        ///  Method for shortening strings in narrow spaces like tables and cards etc.
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="length">Desired length</param>
        /// <returns></returns>
        public static string ShortenString(string text, int length)
        {
            if(string.IsNullOrEmpty(text)) return "";

            if(text.Length > length)
            {
                return text.Substring(0, length) + "...";
            }
            else
            {
                return text;
            }         
        }

        /// <summary>
        ///  Used for hiding user's reputation and converting it to a range to provide anonymity
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="length">Desired length</param>
        /// <returns></returns>
        public static string AnonymizeReputation(double? reputation){

            if(reputation == null || reputation == 0) return "-";
                       
            if(reputation > 0 && reputation < 10) return "0-10";

            if(reputation >= 10 && reputation < 50) return "10-50";

            if(reputation >= 50 && reputation < 100) return "50-100";

            if(reputation >= 100 && reputation < 1000) return reputation.ToString()[0]+"00+";
            
            if(reputation >= 1000 && reputation < 10000)return reputation.ToString()[0]+"000+";

            if(reputation >= 10000)return reputation.ToString()[0]+"0000+";

            return "10.000+";
        }

        public static string FormatPrice(double? price){

           try
           {
                return Convert.ToDouble(price).ToString("N2");
           }
           catch
           {

           }

            if(price == null) 
            {
                return "";
            }
            else
            {
                return price.ToString();
            }

        }
    }
}
