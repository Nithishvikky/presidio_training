using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day13_App.Misc
{
    public static class ExtensionFunctions
    {
        // Extension function which has this as a parameter
        public static bool StringValidationCheck(this string str,int len)
        {
            if (str.Substring(0, 1).ToLower() == "s" && str.Length == len)
                return true;
            return false;
        }
    }
}