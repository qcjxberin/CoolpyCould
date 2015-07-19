using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace CoolpyCould
{
    public class Throttle
    {
        public static ThroottleResult check(string Name, int Seconds, HttpContext context)
        {
            var key = string.Concat(Name, "-", context.Request.UserHostAddress);
            var allowExecute = false;

            if (HttpRuntime.Cache[key] == null)
            {
                HttpRuntime.Cache.Add(key,
                    true,
                    null,
                    DateTime.Now.AddSeconds(Seconds),
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null);

                allowExecute = true;
            }

            if (!allowExecute)
            {
                return new ThroottleResult() { CheckResult = false, Message = "error 406" };
            }
            return new ThroottleResult() { CheckResult = true };
        }

        private string GetIP(HttpContext context)
        {
            if (context.Request.ServerVariables["HTTP_VIA"] != null) // using proxy
            {
                return context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();  // Return real client IP.
            }
            else// not using proxy or can't get the Client IP
            {
                return context.Request.ServerVariables["REMOTE_ADDR"].ToString(); //While it can't get the Client IP, it will return proxy IP.
            }
        }
    }

    public class ThroottleResult
    {
        public bool CheckResult { get; set; }
        public string Message { get; set; }
    }
}