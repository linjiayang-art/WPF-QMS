using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Extensions
{
    public class Factory
    {
        public static string ProcessNumberToLetter(int number)
        {
            if (number >= 1 && number <= 26)
            {
                char letter = (char)('A' + number);
                return letter.ToString();
            }
            else
            {
                return "Invalid Input";
            }
        }
    }
}
