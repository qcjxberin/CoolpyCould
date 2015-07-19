using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoolpyCould
{
    public enum em_SensorType
    {
        value, gps, gen, photo
    }

    public enum em_HttpMeth
    {
        PUT, DELETE
    }

    public class DataPointGenModel
    {
        public string key { get; set; }
        public BsonDocument value { get; set; }
    }

    public class DataPointValModel
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime timestamp { get; set; }
        public double value { get; set; }
    }

    public class DataPointGpsModel
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime timestamp { get; set; }
        public gps value { get; set; }
    }

    public class gps
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public double speed { get; set; }
        [BsonIgnoreIfNull]
        public string offset { get; set; }
    }

    public class DataPointImgModel
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime timestamp { get; set; }
        public byte[] value { get; set; }
    }

    public class SensorModel
    {
        [BsonDefaultValue(em_SensorType.value)]
        public em_SensorType type { get; set; }
        [BsonRequired]
        public string title { get; set; }
        [BsonIgnoreIfNull]
        public string about { get; set; }
        [BsonIgnoreIfNull]
        public IList<string> tags { get; set; }
        [BsonIgnoreIfNull]
        public unit unit { get; set; }
    }

    public class unit
    {
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class DeviceModel
    {
        [BsonRequired]
        public string title { get; set; }
        [BsonIgnoreIfNull]
        public string about { get; set; }
        [BsonIgnoreIfNull]
        public IList<string> tags { get; set; }
        [BsonIgnoreIfNull]
        public location location { get; set; }
        [BsonIgnoreIfNull]
        public string ukey { get; set; }
        [BsonIgnoreIfNull]
        public int? dvid { get; set; }
    }

    public class location
    {
        public string local { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
    }
}