using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AxModel.Data
{
    public class DbProvider
    {
        public static AX_2012_R2_modelEntities GetDb(string dbServer, string dbName)
        {
            if (string.IsNullOrEmpty(dbServer) == true
                || string.IsNullOrEmpty(dbName) == true)
                throw new Exception("Database server and database name should be specified");

            string connStringBase = "data source={0};initial catalog={1};integrated security=True;multipleactiveresultsets=True;App=EntityFramework";
            var connectionString = String.Format(connStringBase, dbServer, dbName);

            var db = new AX_2012_R2_modelEntities();
            db.Database.Connection.ConnectionString = connectionString;

            return db;
        }
    }
}
