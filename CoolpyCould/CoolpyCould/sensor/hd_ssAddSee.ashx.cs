using System.IO;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace CoolpyCould.sensor
{
    /// <summary>
    /// Summary description for hd_ssAddSee
    /// </summary>
    public class hd_ssAddSee : AbstractAsyncHandler
    {
        protected override Task ProcessRequestAsync(HttpContext context)
        {
            var checker = Throttle.check("sssPostGet", Global.throttle, context);
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
            int dvid;
            if (!int.TryParse(routeValues["dvid"].ToString(), out dvid))
            {
                Errors.GetError().p404(context);
                return context.Response.Output.WriteAsync(string.Empty);
            }
            //string ssid = routeValues["ssid"].ToString();

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
                    SensorModel model;
                    try
                    {
                        model = BsonSerializer.Deserialize<SensorModel>(reader.ReadToEnd());
                        if (string.IsNullOrEmpty(model.title))
                        {
                            return context.Response.Output.WriteAsync(Errors.e7001);
                        }
                        return context.Response.Output.WriteAsync(
                            Controller.SensorsController.GetSensors().Create(
                            dvid, model.ToBsonDocument()));
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
                            Controller.SensorsController.GetSensors().GetAll(dvid));
            }

            Errors.GetError().p404(context);
            return context.Response.Output.WriteAsync(string.Empty);
        }
    }
}