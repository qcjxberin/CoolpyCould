using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace CoolpyCould
{
    public class Global : System.Web.HttpApplication
    {
        public static int throttle = 2;
        public static int throttle1 = 10;
        protected void Application_Start(object sender, EventArgs e)
        {
            string from = WebConfigurationManager.AppSettings["throttle"];
            int fmint =0;
            if(int.TryParse(from,out fmint))
            {
                throttle = fmint;
            }
            from = WebConfigurationManager.AppSettings["throttle1"];
            fmint = 0;
            if (int.TryParse(from, out fmint))
            {
                throttle1 = fmint;
            }
            BsonSerializer.UseNullIdChecker = BsonSerializer.UseZeroIdChecker = false;
            RegisterRoutes(RouteTable.Routes);
        }

        void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpHandler<device.hd_dvAddSee>("v1.0/devices");
            routes.MapHttpHandler<device.hd_dvGetPutDel>("v1.0/device/{dvid}");
            routes.MapHttpHandler<sensor.hd_ssAddSee>("v1.0/device/{dvid}/sensors");
            routes.MapHttpHandler<sensor.hd_ssGetPutDel>("v1.0/device/{dvid}/sensor/{ssid}");
            routes.MapHttpHandler<datapoint.hd_dpGetLast>("v1.0/device/{dvid}/sensor/{ssid}/datapoint");
            routes.MapHttpHandler<datapoint.hd_dpAddSee>("v1.0/device/{dvid}/sensor/{ssid}/datapoints");
            routes.MapHttpHandler<datapoint.hd_dpGetPutDel>("v1.0/device/{dvid}/sensor/{ssid}/datapoint/{key}");

            routes.MapHttpHandler<gets.hd_getAll>("v1.0/device/{dvid}/sensor/{ssid}/json");
        }
    }

    public static class HttpHandlerExtensions
    {
        public static void MapHttpHandler<THandler>(this RouteCollection routes, string url) where THandler : IHttpHandler, new()
        {
            routes.MapHttpHandler<THandler>(null, url, null, null);
        }

        public static void MapHttpHandler<THandler>(this RouteCollection routes, string name, string url, object defaults, object constraints)
            where THandler : IHttpHandler, new()
        {
            var route = new Route(url, new HttpHandlerRouteHandler<THandler>());
            route.Defaults = new RouteValueDictionary(defaults);
            route.Constraints = new RouteValueDictionary(constraints);
            routes.Add(name, route);
        }
    }

    public class HttpHandlerRouteHandler<THandler> : IRouteHandler where THandler : IHttpHandler, new()
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new THandler();
        }
    }
}