using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace CoolpyCould
{
    public class DataDV
    {
        static DataDV dv;
        public static DataDV GetDV()
        {
            if (dv == null)
            {
                dv = new DataDV();
            }
            return dv;
        }

        public MongoDatabase getCol()
        {
            var connectionString = WebConfigurationManager.AppSettings["connet"];
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            //server.DropDatabase("Cooldb");
            return server.GetDatabase("Cooldb");
        }

        bool isConneted = false;
        public bool CheckConnected()
        {
            if (!isConneted)
            {
                try
                {
                    getCol().Server.Ping();
                    isConneted = true;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else { 
            return true;
            }
        }

    }
}