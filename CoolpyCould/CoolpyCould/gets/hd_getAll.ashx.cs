using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CoolpyCould.gets
{
    /// <summary>
    /// Summary description for hd_getAll
    /// </summary>
    public class hd_getAll : AbstractAsyncHandler
    {
        protected override Task ProcessRequestAsync(HttpContext context)
        {
            var checker = Throttle.check("alsGet", 10, context);
            if (!checker.CheckResult)
            {
                return context.Response.Output.WriteAsync("{Error:406}");
            }

            context.Response.ContentType = "application/json";

            var appKey = context.Request.Headers["U-ApiKey"];

            if (appKey != "appkey")
            {
                return context.Response.Output.WriteAsync("{Error:412}");
            }

            var meth = context.Request.HttpMethod;
            //查看数据结点信息
            if (meth == "GET")
            {
                var routeValues = context.Request.RequestContext.RouteData.Values;
                string dvid = routeValues["dvid"].ToString();
                string ssid = routeValues["ssid"].ToString();
                var start = context.Request.QueryString["start"];
                var end = context.Request.QueryString["end"];
                var interval = context.Request.QueryString["interval"];
                var page = context.Request.QueryString["page"];

            }
            return context.Response.Output.WriteAsync("{value:39.4}");
        }
    }
}