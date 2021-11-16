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
            if(text.Length > length)
            {
                return text.Substring(0, length) + "...";
            }
            else
            {
                return text;
            }         
        }
    }
}
