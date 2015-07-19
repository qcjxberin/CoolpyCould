using System.IO;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Linq;

namespace CoolpyCould.device
{
    /// <summary>
    /// Summary description for hd_dvGetPutDel
    /// </summary>
    public class hd_dvGetPutDel : AbstractAsyncHandler
    {
        protected override Task ProcessRequestAsync(HttpContext context)
        {
            var checker = Throttle.check("dvsPutDel", Global.throttle, context);
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

            //判断是否有模拟put,delete请求
            var req = context.Request.QueryString["method"];
            if (!string.IsNullOrEmpty(req))
            {
                var mt = req.ToUpper();
                if (Enum.GetNames(typeof(em_HttpMeth)).Contains(mt))
                {
                    meth = mt;
                }else{
                    Errors.GetError().p417(context);
                    return context.Response.Output.WriteAsync(string.Empty);
                }
            }

            //查看设备信息
            if (meth == "GET")
            {
                int dv;
                if (int.TryParse(dvid, out dv))
                {
                   return context.Response.Output.WriteAsync(
                       Controller.DevicesController.GetDevices().GetOne(appKey, dv));
                }
                else
                {
                    return context.Response.Output.WriteAsync(Errors.e7002);
                }
            }

            //修改设备信息
            if (meth == "PUT")
            {
                if (context.Request.InputStream.Length == 0)
                {
                    Errors.GetError().p204(context);
                    return context.Response.Output.WriteAsync(string.Empty);
                }
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    int dv;
                    if (int.TryParse(dvid, out dv))
                    {
                        if (!DataDV.GetDV().CheckConnected())
                        {
                            return context.Response.Output.WriteAsync(Errors.e7006);
                        }
                        var obj = Controller.DevicesController.GetDevices().GetOne(appKey, dv);
                        if (obj != null)
                        {
                            DeviceModel model;
                            try
                            {
                                model = BsonSerializer.Deserialize<DeviceModel>(reader.ReadToEnd());
                                if (string.IsNullOrEmpty(model.title))
                                {
                                    return context.Response.Output.WriteAsync(Errors.e7001);
                                }
                                return context.Response.Output.WriteAsync(
                                    Controller.DevicesController.GetDevices().Edit(appKey, dv, model.ToBsonDocument()));
                            }
                            catch 
                            {
                                return context.Response.Output.WriteAsync(Errors.e7005); 
                            }
                        }
                        else
                        {
                            return context.Response.Output.WriteAsync(Errors.e7004);
                        }
                    }
                    else
                    {
                        return context.Response.Output.WriteAsync(Errors.e7002);
                    }
                }
            }

            //删除设备
            if (meth == "DELETE")
            {
                int dv;
                if (int.TryParse(dvid, out dv))
                {
                    if (!DataDV.GetDV().CheckConnected())
                    {
                        return context.Response.Output.WriteAsync(Errors.e7006);
                    }
                    var obj = Controller.DevicesController.GetDevices().GetOne(appKey, dv);
                    if (obj != null)
                    {
                        return context.Response.Output.WriteAsync(
                            Controller.DevicesController.GetDevices().Delete(appKey, dv));
                    }
                    else
                    {
                        return context.Response.Output.WriteAsync(Errors.e7004);
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