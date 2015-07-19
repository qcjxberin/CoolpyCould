using CoolpyCould.Controller;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CoolpyCould.datapoint
{
    /// <summary>
    /// Summary description for hd_dpAddSee
    /// </summary>
    public class hd_dpAddSee : AbstractAsyncHandler
    {
        protected override Task ProcessRequestAsync(HttpContext context)
        {
            var checker = Throttle.check("dpsPost", Global.throttle1, context);
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

            //添加设备
            if (meth == "POST")
            {
                if (context.Request.InputStream.Length == 0)
                {
                    Errors.GetError().p204(context);
                    return context.Response.Output.WriteAsync(string.Empty);
                }
                int dv;
                int ss;
                if (int.TryParse(dvid, out dv) && int.TryParse(ssid, out ss))
                {
                    string netjson;
                    using (var reader = new StreamReader(context.Request.InputStream))
                    {
                        netjson = reader.ReadToEnd();
                    }
                    var result = SensorsController.GetSensors().GetSensorType(dv, ss);
                    if (!result.hasError)
                    {
                        var type = result.type;
                        if (type == em_SensorType.gen)
                        {
                            return context.Response.Output.WriteAsync(
                                DataPointController.GetDataPoints().CreateGen(dv, ss, type, netjson));
                        }
                        else if (type == em_SensorType.gps)
                        {
                            return context.Response.Output.WriteAsync(
                                DataPointController.GetDataPoints().CreateGps(dv, ss, type, netjson));
                        }
                        else if (type == em_SensorType.value)
                        {
                            if (netjson.StartsWith("{"))
                            {
                                return context.Response.Output.WriteAsync(
                                DataPointController.GetDataPoints().CreateValOne(dv, ss, type, netjson));
                            }
                            else if (netjson.StartsWith("["))
                            {
                                return context.Response.Output.WriteAsync(
                                DataPointController.GetDataPoints().CreateValMultible(dv, ss, type, netjson));
                            }
                        }
                        else if (type == em_SensorType.photo)
                        {
                            //有另外专属接口处理图片管理
                            //return context.Response.Output.WriteAsync(
                            //    DataPointController.GetDataPoints().CreateImg(dv, ss, type, netjson));
                            return context.Response.Output.WriteAsync(Errors.e7009);
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