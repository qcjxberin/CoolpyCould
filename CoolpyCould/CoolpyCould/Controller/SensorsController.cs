using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CoolpyCould.Controller
{
    public class SensorsController
    {
        private static SensorsController ctrler;
        public static SensorsController GetSensors()
        {
            if (ctrler == null)
            {
                ctrler = new SensorsController();
            }
            return ctrler;
        }

        MongoCollection<BsonDocument> sss = DataDV.GetDV().getCol().GetCollection("sss");

        public string Create(int dvid, BsonDocument data)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var last = sss.FindAll().LastOrDefault();       
            data.Add("dvid", dvid);
            data.Add("ssid", last == null ? 1 : last["ssid"].AsInt32 + 1);
            sss.Insert(data);
            return "{sensor_id:" + data["ssid"].ToString() + "}";
        }

        public string GetAll(int dvid)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var query = Query.EQ("dvid", dvid);
            var dball = sss.Find(query).AsParallel();
            var display = new List<BsonDocument>();
            Parallel.ForEach(dball, item =>
            {
                item.Add("id", item["ssid"].ToInt32());
                item.Remove("_id");
                item.Remove("dvid");
                item.Remove("ssid");
                display.Add(item);
            });
            return display.ToJson();
        }

        public string GetOne(int dvid, int ssid)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var query1 = Query.EQ("dvid", dvid);
            var query2 = Query.EQ("ssid", ssid);
            var query = Query.And(query1, query2);
            var obj = sss.FindOne(query);
            if (obj != null)
            {
                obj.Add("id", obj["ssid"].ToInt32());
                obj.Remove("_id");
                obj.Remove("dvid");
                obj.Remove("ssid");
                return obj.ToJson();
            }
            else
            {
                return Errors.e7004;
            }
        }

        public TypeResult GetSensorType(int dvid, int ssid)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return new TypeResult() { hasError = true, error = Errors.e7006 };
            }
            var query1 = Query.EQ("dvid", dvid);
            var query2 = Query.EQ("ssid", ssid);
            var query = Query.And(query1, query2);
            var obj = sss.FindOne(query);
            if (obj != null)
            {
                return new TypeResult() { hasError=false, type = (em_SensorType)obj["type"].AsInt32 };
            }
            else
            {
                return new TypeResult() { hasError = true, error = Errors.e7004 };
            }
        }

        public string Edit(int dvid, int ssid, BsonDocument data)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var query1 = Query.EQ("dvid", dvid);
            var query2 = Query.EQ("ssid", ssid);
            var query = Query.And(query1, query2);
            UpdateDocument update = new UpdateDocument("$set", data);
            sss.Update(query, update, UpdateFlags.Upsert);
            return string.Empty;
        }

        public string Delete(int dvid, int ssid)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            //继续删除数据结点表
            var result = GetSensorType(dvid, ssid);
            if (!result.hasError)
            {
                var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + result.type.ToString());
                coll.Drop();
            }
            //删除传感器
            var query1 = Query.EQ("dvid", dvid);
            var query2 = Query.EQ("ssid", ssid);
            var query = Query.And(query1, query2);
            sss.Remove(query);
            return string.Empty;
        }

        public List<BsonDocument> GetAllForDel(int dvid)
        {
            var query = Query.EQ("dvid", dvid);
            return sss.Find(query).ToList();
        }

    }

    public class TypeResult
    {
        public TypeResult()
        {
            hasError = false;
        }
        public bool hasError { get; set; }
        public string error { get; set; }
        public em_SensorType type { get; set; }
    }
}