using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Extensions
{
    public static class StringExtensions
    {

        public static string GetMD5(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) 
                throw new ArgumentNullException(nameof(str));

            var hash=MD5.Create().ComputeHash(Encoding.Default.GetBytes(str));

            return Convert.ToBase64String(hash);


        }
    }
}
