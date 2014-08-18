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
using System.Threading.Tasks;

namespace AxModel.Data2.Data
{
    public class AxModelDataProvider
    {
        public static AxModelDataSettings DbSettings { get; set; }

        public AxModelDataProvider() 
        {
            
        }
        public AxModelDataProvider(AxModelDataSettings settings)
        {
            DbSettings = settings;
        }

        private static void PopulateDb(AX_2012_R2Entities db)
        {
            if (DbSettings != null)
            {
                if (string.IsNullOrEmpty(DbSettings.DbServer) == true
                || string.IsNullOrEmpty(DbSettings.DbName) == true)
                    throw new Exception("Database server and database name should be specified");

                string connStringBase = "data source={0};initial catalog={1};integrated security=True;multipleactiveresultsets=True;App=EntityFramework";
                var connectionString = String.Format(connStringBase, DbSettings.DbServer, DbSettings.DbName);

                db.Database.Connection.ConnectionString = connectionString;
            }
        }

        public static ElementType GetElementType(int elementType)
        {
            using (var db = new AX_2012_R2Entities())
            {
                PopulateDb(db);
                return db.ElementTypes.FirstOrDefault(m => m.ElementType1 == elementType);
            }
        }

        public static Model GetModel(int modelId)
        {
            using (var db = new AX_2012_R2Entities())
            {
                PopulateDb(db);
                return db.Models.FirstOrDefault(m => m.Id == modelId);
            }
        }
        public static List<Model> GetModels()
        {
            using (var db = new AX_2012_R2Entities())
            {
                PopulateDb(db);
                return db.Models.ToList();
            }
        }

        public static Layer GetLayer(int layerId)
        {
            using (var db = new AX_2012_R2Entities())
            {
                PopulateDb(db);
                return db.Layers.FirstOrDefault(l => l.Id == layerId);
            }

        }

        public static List<ModelManifest> GetManifests()
        {
            using (var db = new AX_2012_R2Entities())
            {
                PopulateDb(db);
                return db.ModelManifests.ToList();
            }
        }
        public static ModelManifest GetManifest(int modelId)
        {
            using (var db = new AX_2012_R2Entities())
            {
                PopulateDb(db);
                return db.ModelManifests.FirstOrDefault(m => m.ModelId == modelId);
            }
        }

        public static ModelElement GetModelElement(int elementHandle)
        {
            using (var db = new AX_2012_R2Entities())
            {
                PopulateDb(db);
                return db.ModelElements.FirstOrDefault(m => m.ElementHandle == elementHandle);
            }
        }

        public static List<ModelElementData> GetModelElementData(int modelId)
        {
            using (var db = new AX_2012_R2Entities())
            {
                PopulateDb(db);
                return db.ModelElementDatas.Where(m => m.ModelId == modelId).ToList();
            }
        }

        public static List<ModelElementData> GetModelElementDataForElementData(ModelElementData elementData, int minLayerId)
        {
            using (var db = new AX_2012_R2Entities())
            {
               
                PopulateDb(db);
                var otherDatas = db.ModelElementDatas
                                   .Where(d => (d.ElementHandle == elementData.ElementHandle
                                       //|| (d.ParentHandle == elementData.ParentHandle && elementData.ParentHandle != 0)
                                                    || (d.ElementHandle == elementData.ParentHandle && elementData.ParentHandle == 0)
                                                    )
                                                    && d.ModelId != elementData.ModelId
                                                    && d.LayerId <= elementData.LayerId
                                                    && d.LayerId >= minLayerId);
                return otherDatas.ToList();
            }
        }

        public static int GetModelElementCount(int modelId)
        {
            using (var db = new AX_2012_R2Entities())
            {
                PopulateDb(db);
                return db.ModelElementDatas.Where(m => m.ModelId == modelId).Count();
            }
        }
    }
}
