using CoolpyCould.Controller;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CoolpyCould.datapoint
{
    /// <summary>
    /// Summary description for hd_dpGetPutDel
    /// </summary>
    public class hd_dpGetPutDel : AbstractAsyncHandler
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
            string skey = routeValues["key"].ToString();

            //判断是否有模拟put,delete请求
            var req = context.Request.QueryString["method"];
            if (!string.IsNullOrEmpty(req))
            {
                var mt = req.ToUpper();
                if (Enum.GetNames(typeof(em_HttpMeth)).Contains(mt))
                {
                    meth = mt;
                }
                else
                {
                    Errors.GetError().p417(context);
                    return context.Response.Output.WriteAsync(string.Empty);
                }
            }

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
                        var type = result.type;
                        if (type == em_SensorType.gen)
                        {
                            return context.Response.Output.WriteAsync(
                                DataPointController.GetDataPoints().GetGen(dv, ss, type, skey));
                        }
                        else if (type == em_SensorType.gps || type == em_SensorType.value || type == em_SensorType.photo)
                        {
                            DateTime dt;
                            try
                            {
                                dt = DateTime.ParseExact(skey, "yyyy-MM-ddTHH:mm:ss",
                                    new CultureInfo("zh-CN"), DateTimeStyles.AssumeUniversal);
                            }
                            catch
                            {
                                return context.Response.Output.WriteAsync(Errors.e7002);
                            }
                            return context.Response.Output.WriteAsync(
                                DataPointController.GetDataPoints().GetGpsValImg(dv, ss, type, dt));
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

            //修改设备信息
            if (meth == "PUT")
            {
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
                                DataPointController.GetDataPoints().EditGen(dv, ss, type, skey, netjson));
                        }
                        else if (type == em_SensorType.gps)
                        {
                            DateTime dt;
                            try
                            {
                                dt = DateTime.ParseExact(skey, "yyyy-MM-ddTHH:mm:ss",
                                        new CultureInfo("zh-CN"), DateTimeStyles.AssumeUniversal);
                            }
                            catch
                            {
                                return context.Response.Output.WriteAsync(Errors.e7002);
                            }
                            return context.Response.Output.WriteAsync(
                                DataPointController.GetDataPoints().EditGps(dv, ss, type, dt, netjson));
                        }
                        else if (type == em_SensorType.value)
                        {
                            DateTime dt;
                            try
                            {
                                dt = DateTime.ParseExact(skey, "yyyy-MM-ddTHH:mm:ss",
                                        new CultureInfo("zh-CN"), DateTimeStyles.AssumeUniversal);
                            }
                            catch
                            {
                                return context.Response.Output.WriteAsync(Errors.e7002);
                            }
                            return context.Response.Output.WriteAsync(
                                DataPointController.GetDataPoints().EditVal(dv, ss, type, dt, netjson));
                        }
                        else if (type == em_SensorType.photo)
                        {
                            DateTime dt;
                            try
                            {
                                dt = DateTime.ParseExact(skey, "yyyy-MM-ddTHH:mm:ss",
                                        new CultureInfo("zh-CN"), DateTimeStyles.AssumeUniversal);
                            }
                            catch
                            {
                                return context.Response.Output.WriteAsync(Errors.e7002);
                            }
                            return context.Response.Output.WriteAsync(
                                DataPointController.GetDataPoints().EditImg(dv, ss, type, dt, netjson));
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

            //删除设备
            if (meth == "DELETE")
            {
                int dv;
                int ss;
                if (int.TryParse(dvid, out dv) && int.TryParse(ssid, out ss))
                {
                    var result = SensorsController.GetSensors().GetSensorType(dv, ss);
                    if (!result.hasError)
                    {
                        var type = result.type;
                        if (type == em_SensorType.gen)
                        {
                            return context.Response.Output.WriteAsync(
                                DataPointController.GetDataPoints().DelGen(dv, ss, type, skey));
                        }
                        else if (type == em_SensorType.gps || type == em_SensorType.value || type == em_SensorType.photo)
                        {
                            DateTime dt;
                            try
                            {
                                dt = DateTime.ParseExact(skey, "yyyy-MM-ddTHH:mm:ss",
                                        new CultureInfo("zh-CN"), DateTimeStyles.AssumeUniversal);
                            }
                            catch
                            {
                                return context.Response.Output.WriteAsync(Errors.e7002);
                            }
                            return context.Response.Output.WriteAsync(
                                DataPointController.GetDataPoints().DelGpsValImg(dv, ss, type, dt));
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