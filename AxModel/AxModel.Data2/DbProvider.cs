/*
 * AX 2012 – Model dependencies and Install Order
 * http://shashidotnet.wordpress.com
 * 
 * Author: Shashi Sadasivan
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AxModel.Data2
{
    public class DbProvider
    {
        public static AX_2012_R2Entities GetDb(string dbServer, string dbName)
        {
            if (string.IsNullOrEmpty(dbServer) == true
                || string.IsNullOrEmpty(dbName) == true)
                throw new Exception("Database server and database name should be specified");

            string connStringBase = "data source={0};initial catalog={1};integrated security=True;multipleactiveresultsets=True;App=EntityFramework";
            var connectionString = String.Format(connStringBase, dbServer, dbName);

            var db = new AX_2012_R2Entities();
            db.Database.Connection.ConnectionString = connectionString;

            return db;
        }
    }
}
