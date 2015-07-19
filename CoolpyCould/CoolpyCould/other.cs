using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoolpyCould
{
    public class other
    {
        public static bool IsJson(string input)
        {
            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        } 
    }
}