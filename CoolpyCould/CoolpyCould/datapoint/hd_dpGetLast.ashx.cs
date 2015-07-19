using CoolpyCould.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CoolpyCould.datapoint
{
    /// <summary>
    /// Summary description for hd_dpGetPutDel
    /// </summary>
    public class hd_dpGetLast : AbstractAsyncHandler
    {
        protected override Task ProcessRequestAsync(HttpContext context)
        {
            var checker = Throttle.check("dpsLast", Global.throttle1, context);
            if (!checker.CheckResult)
            {
                Errors.GetError().p406(context);
                return context.Response.Output.WriteAsync(string.Empty);
            }

            context.Response.ContentType = "application/json";

            var appKey = context.Request.Headers["U-ApiKey"];

            if (appKey != "appkey")
            {
                Errors.GetError().p412(context);
                return context.Response.Output.WriteAsync(string.Empty);
            }

            var meth = context.Request.HttpMethod;
            var routeValues = context.Request.RequestContext.RouteData.Values;
            string dvid = routeValues["dvid"].ToString();
            string ssid = routeValues["ssid"].ToString();

            //查看设备信息
            if (meth == "GET")
            {
                int dv;
                int ss;
                if (int.TryParse(dvid, out dv) && int.TryParse(ssid, out ss))
                {
                    var result = SensorsController.GetSensors().GetSensorType(dv, ss);
                    if (!result.hasError)
                    {
                        if (result.type == em_SensorType.gen) {
                            return context.Response.Output.WriteAsync(
                                    DataPointController.GetDataPoints().GetGenLast(dv, ss, result.type));
                        }
                        else if (result.type == em_SensorType.gps || result.type == em_SensorType.photo || result.type == em_SensorType.value) {
                            return context.Response.Output.WriteAsync(
                                   DataPointController.GetDataPoints().GetGpsValImgLast(dv, ss, result.type));
                        }
                    }
                    else
                    {
                        return context.Response.Output.WriteAsync(result.error);
                    }
                }
                else
                {
                    return context.Response.Output.WriteAsync(Errors.e7002);
                }
            }

            Errors.GetError().p404(context);
            return context.Response.Output.WriteAsync(string.Empty);
        }
    }
}