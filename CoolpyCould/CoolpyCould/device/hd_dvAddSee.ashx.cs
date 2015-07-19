using System.IO;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace CoolpyCould.device
{
    /// <summary>
    /// Summary description for hd_dvAddSee
    /// </summary>
    public class hd_dvAddSee : AbstractAsyncHandler
    {
        protected override Task ProcessRequestAsync(HttpContext context)
        {
            var checker = Throttle.check("dvsPostGet", Global.throttle, context);
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

            //添加设备
            if (meth == "POST")
            {
                if (context.Request.InputStream.Length == 0)
                {
                    Errors.GetError().p204(context);
                    return context.Response.Output.WriteAsync(string.Empty);
                }
                using (var reader = new StreamReader(context.Request.InputStream))
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
                            Controller.DevicesController.GetDevices().Create(
                            appKey, model.ToBsonDocument()));
                    }
                    catch
                    {
                        return context.Response.Output.WriteAsync(Errors.e7003);
                    }
                }
            }

            //罗列设备
            if (meth == "GET")
            {
                return context.Response.Output.WriteAsync(
                    Controller.DevicesController.GetDevices().GetAll(appKey));
            }

            Errors.GetError().p404(context);
            return context.Response.Output.WriteAsync(string.Empty);
        }
    }
}