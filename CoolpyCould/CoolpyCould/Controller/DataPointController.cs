using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace CoolpyCould.Controller
{
    public class DataPointController
    {
        private static DataPointController ctrler;
        public static DataPointController GetDataPoints()
        {
            if (ctrler == null)
            {
                ctrler = new DataPointController();
            }
            return ctrler;
        }

        #region 查看指定数据
        public string GetGen(int dvid, int ssid, em_SensorType type, string key)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            var query = Query.EQ("key", key);
            var obj = coll.FindOne(query);
            if (obj != null)
            {
                obj.Remove("_id");
                obj.Remove("key");
                return obj.ToJson();
            }
            else
            {
                return Errors.e7004;
            }
        }

        public string GetGpsValImg(int dvid, int ssid, em_SensorType type, DateTime key)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            var query = Query.EQ("timestamp", key);
            var obj = coll.FindOne(query);
            if (obj != null)
            {
                obj.Remove("_id");
                obj.Remove("timestamp");
                return obj.ToJson();
            }
            else
            {
                return Errors.e7004;
            }
        }
        #endregion

        #region 查看最新数据
        public string GetGenLast(int dvid, int ssid, em_SensorType type)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            var obj = coll.FindAll().LastOrDefault();
            if (obj != null)
            {
                obj.Remove("_id");
                obj["key"] = obj["key"].ToString();
                return obj.ToJson();
            }
            else
            {
                return Errors.e7004;
            }
        }

        public string GetGpsValImgLast(int dvid, int ssid, em_SensorType type)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            var obj = coll.FindAll().LastOrDefault();
            if (obj != null)
            {
                obj.Remove("_id");
                obj["timestamp"] = obj["timestamp"].ToString().Replace("Z", "");
                return obj.ToJson();
            }
            else
            {
                return Errors.e7004;
            }
        }
        #endregion

        #region 编辑
        public string EditGen(int dvid, int ssid, em_SensorType type, string key, string netjson)
        {
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            DataPointGenModel gm;
            try
            {
                gm = BsonSerializer.Deserialize<DataPointGenModel>(netjson);
                if (!DataDV.GetDV().CheckConnected())
                {
                    return Errors.e7006;
                }
                var query = Query.EQ("key", key);
                gm.key = key;
                UpdateDocument update = new UpdateDocument("$set", gm.ToBsonDocument());
                coll.Update(query, update, UpdateFlags.Upsert);
                return string.Empty;
            }
            catch
            {
                return Errors.e7003;
            }
        }

        public string EditGps(int dvid, int ssid, em_SensorType type, DateTime key, string netjson)
        {
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            DataPointGpsModel gm;
            try
            {
                gm = BsonSerializer.Deserialize<DataPointGpsModel>(netjson);
                if (!DataDV.GetDV().CheckConnected())
                {
                    return Errors.e7006;
                }
                var query = Query.EQ("timestamp", key);
                gm.timestamp = key;
                UpdateDocument update = new UpdateDocument("$set", gm.ToBsonDocument());
                coll.Update(query, update, UpdateFlags.Upsert);
                return string.Empty;
            }
            catch
            {
                return Errors.e7003;
            }
        }

        public string EditVal(int dvid, int ssid, em_SensorType type, DateTime key, string netjson)
        {
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            DataPointValModel gm;
            try
            {
                gm = BsonSerializer.Deserialize<DataPointValModel>(netjson);
                if (!DataDV.GetDV().CheckConnected())
                {
                    return Errors.e7006;
                }
                var query = Query.EQ("timestamp", key);
                gm.timestamp = key;
                UpdateDocument update = new UpdateDocument("$set", gm.ToBsonDocument());
                coll.Update(query, update, UpdateFlags.Upsert);
                return string.Empty;
            }
            catch
            {
                return Errors.e7003;
            }
        }

        public string EditImg(int dvid, int ssid, em_SensorType type, DateTime key, string netjson)
        {
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            DataPointImgModel gm;
            try
            {
                gm = BsonSerializer.Deserialize<DataPointImgModel>(netjson);
                if (!DataDV.GetDV().CheckConnected())
                {
                    return Errors.e7006;
                }
                var query = Query.EQ("timestamp", key);
                gm.timestamp = key;
                UpdateDocument update = new UpdateDocument("$set", gm.ToBsonDocument());
                coll.Update(query, update, UpdateFlags.Upsert);
                return string.Empty;
            }
            catch
            {
                return Errors.e7003;
            }
        }

        #endregion

        #region 删除
        public string DelGen(int dvid, int ssid, em_SensorType type, string key)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            var query = Query.EQ("key", key);
            var obj = coll.FindOne(query);
            if (obj != null)
            {
                coll.Remove(query);
                return string.Empty;
            }
            else
            {
                return Errors.e7004;
            }
        }

        public string DelGpsValImg(int dvid, int ssid, em_SensorType type, DateTime key)
        {
            if (!DataDV.GetDV().CheckConnected())
            {
                return Errors.e7006;
            }
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            var query = Query.EQ("timestamp", key);
            var obj = coll.FindOne(query);
            if (obj != null)
            {
                coll.Remove(query);
                return string.Empty;
            }
            else
            {
                return Errors.e7004;
            }
        }

        #endregion

        #region 创建
        public string CreateGen(int dvid, int ssid, em_SensorType type, string netjson)
        {
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            DataPointGenModel gm;
            try
            {
                gm = BsonSerializer.Deserialize<DataPointGenModel>(netjson);
                if (string.IsNullOrEmpty(gm.key) || gm.key.Length > 128 || gm.value.LongCount() > 1024)  
                {
                    return Errors.e7001;
                }
                if (!DataDV.GetDV().CheckConnected())
                {
                    return Errors.e7006;
                }
                var query = Query.EQ("key", gm.key);
                var obj = coll.FindOne(query);
                if (obj == null)
                {
                    coll.Insert(gm.ToBsonDocument());
                    return string.Empty;
                }
                else
                {
                    return Errors.e7008;
                }
            }
            catch
            {
                return Errors.e7003;
            }
        }

        public string CreateGps(int dvid, int ssid, em_SensorType type, string netjson)
        {
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            DataPointGpsModel gm;
            try
            {
                gm = BsonSerializer.Deserialize<DataPointGpsModel>(netjson);
                if (!DataDV.GetDV().CheckConnected())
                {
                    return Errors.e7006;
                }
                var query = Query.EQ("timestamp", gm.timestamp);
                var obj = coll.FindOne(query);
                if (obj == null)
                {
                    coll.Insert(gm.ToBsonDocument());
                    return string.Empty;
                }
                else
                {
                    return Errors.e7008;
                }
            }
            catch
            {
                return Errors.e7003;
            }
        }

        public string CreateValOne(int dvid, int ssid, em_SensorType type, string netjson)
        {
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            DataPointValModel gm;
            try
            {
                gm = BsonSerializer.Deserialize<DataPointValModel>(netjson);
                if (!DataDV.GetDV().CheckConnected())
                {
                    return Errors.e7006;
                }
                var query = Query.EQ("timestamp", gm.timestamp);
                var obj = coll.FindOne(query);
                if (obj == null)
                {
                    coll.Insert(gm.ToBsonDocument());
                    return string.Empty;
                }
                else
                {
                    return Errors.e7008;
                }
            }
            catch
            {
                return Errors.e7003;
            }
        }

        public string CreateValMultible(int dvid, int ssid, em_SensorType type, string netjson)
        {
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            ///数组形数值结点数据
            List<DataPointValModel> gm;
            try
            {
                gm = BsonSerializer.Deserialize<List<DataPointValModel>>(netjson);
                if (!DataDV.GetDV().CheckConnected())
                {
                    return Errors.e7006;
                }
                List<IMongoQuery> CheckQuery = new List<IMongoQuery>();
                foreach (var item in gm)
                {
                    var query = Query.EQ("timestamp", item.timestamp);
                    CheckQuery.Add(query);
                }
                var obj = coll.FindOne(Query.Or(CheckQuery));
                if (obj == null)
                {
                    coll.InsertBatch(gm);
                    return string.Empty;
                }
                else
                {
                    return Errors.e7008;
                }
            }
            catch
            {
                return Errors.e7003;
            }
        }

        public string CreateImg(int dvid, int ssid, em_SensorType type, string netjson)
        {
            var coll = DataDV.GetDV().getCol().GetCollection(dvid.ToString() + ssid.ToString() + type.ToString());
            DataPointImgModel gm;
            try
            {
                gm = BsonSerializer.Deserialize<DataPointImgModel>(netjson);
                if (!DataDV.GetDV().CheckConnected())
                {
                    return Errors.e7006;
                }
                var query = Query.EQ("timestamp", gm.timestamp);
                var obj = coll.FindOne(query);
                if (obj == null)
                {
                    coll.Insert(gm.ToBsonDocument());
                    return string.Empty;
                }
                else
                {
                    return Errors.e7008;
                }
            }
            catch
            {
                return Errors.e7003;
            }
        }
        #endregion

    }
}