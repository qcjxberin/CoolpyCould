using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CoolpyCould.Controller
{
    public class DevicesController
    {
        private static DevicesController ctrler;
        public static DevicesController GetDevices()
        {
            if (ctrler == null)
            {
                ctrler = new DevicesController();
            }
            return ctrler;
        }

        MongoCollection<BsonDocument> dvs = DataDV.GetDV().getCol().GetCollection("dvs");
        public string Create(string appKey, BsonDocument data)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var last = dvs.FindAll().LastOrDefault();
            data.Add("ukey", appKey);
            data.Add("dvid", last == null ? 1 : last["dvid"].AsInt32 + 1);
            dvs.Insert(data);
            return "{device_id:" + data["dvid"].ToString() + "}";
        }

        public string GetAll(string appKey)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var query = Query.EQ("ukey", appKey);
            var dball = dvs.Find(query).AsParallel();
            var display = new List<BsonDocument>();
            Parallel.ForEach(dball, item =>
            {
                item.Add("id", item["dvid"].ToString());
                item.Remove("_id");
                item.Remove("ukey");
                item.Remove("dvid");
                display.Add(item);
            });
            return display.ToJson();
        }

        public string GetOne(string appKey, int dvid)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var query1 = Query.EQ("ukey", appKey);
            var query2 = Query.EQ("dvid", dvid);
            var query = Query.And(query1, query2);
            var obj = dvs.FindOne(query);
            if (obj != null)
            {
                obj.Add("id", obj["dvid"].ToString());
                obj.Remove("_id");
                obj.Remove("ukey");
                obj.Remove("dvid");
                return obj.ToString();
            }
            else
            {
                return Errors.e7004;
            }
        }

        public string Edit(string appKey, int dvid, BsonDocument data)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var query1 = Query.EQ("ukey", appKey);
            var query2 = Query.EQ("dvid", dvid);
            var query = Query.And(query1, query2);
            UpdateDocument update = new UpdateDocument("$set", data);
            dvs.Update(query, update, UpdateFlags.Upsert);
            return string.Empty;
        }

        public string Delete(string appKey, int dvid)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var sensors = SensorsController.GetSensors().GetAllForDel(dvid);
            Parallel.ForEach(sensors, item =>
            {
                SensorsController.GetSensors().Delete(item["dvid"].AsInt32, item["ssid"].AsInt32);
            });
            var query1 = Query.EQ("ukey", appKey);
            var query2 = Query.EQ("dvid", dvid);
            var query = Query.And(query1, query2);
            dvs.Remove(query);
            return string.Empty;
        }

    }
}