using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxModel.Data.ModelDependency
{
    public class Checker
    {
        private AX_2012_R2_modelEntities db;
        private int _minLayerId;

        public List<ModelDependency> Dependencies { get; private set; }
        //Get all Models above a certain Layer (i.e. skip the SYS, SYP's etc)

        public Checker()
        {
            db = new Data.AX_2012_R2_modelEntities();
            var connectionString = db.Database.Connection.ConnectionString;
            Console.WriteLine(connectionString);
            Console.WriteLine("Using connectiong string from config file....!");
        }
        public Checker(string dbServer, string dbName)
        {
            db = DbProvider.GetDb(dbServer, dbName);
            /*
            string connStringBase = "data source={0};initial catalog={1};integrated security=True;multipleactiveresultsets=True;App=EntityFramework";
            db = new Data.AX_2012_R2_modelEntities();
            if (string.IsNullOrEmpty(dbServer) == false
                && string.IsNullOrEmpty(dbName) == false)
            {
                var connectionString = String.Format("data source={0};initial catalog={1};integrated security=True;multipleactiveresultsets=True;App=EntityFramework", 
                    dbServer, dbName);

                db.Database.Connection.ConnectionString = connectionString;
            }
            else
                throw new Exception("Database server and database name should be specified");
            */
            Console.WriteLine(db.Database.Connection.ConnectionString);
        }
        public void Start()
        {

            this.Dependencies = new List<ModelDependency>();

            //TODO: comment this return to continue
            // return;
            
            
            //var layers = db.Layers.ToList();
            //layers.ForEach(l =>
            //    Console.WriteLine(l.Name + " : " + l.Id.ToString()));

            this._minLayerId = 8;

            var allModels = db.Models.ToList();

            var models = db.Models.OrderBy(m => m.LayerId).Where(m => m.LayerId >= this._minLayerId).ToList();
            models.ForEach(m =>
                Console.WriteLine(m.Id.ToString() + " : " + m.Layer.Name + " " + m.LayerId));



            var mfs = db.ModelManifests.Where(m => m.Model.LayerId >= this._minLayerId).OrderBy(m => m.ModelId).ToList();

            mfs.ForEach(m =>
                Console.WriteLine(m.ModelId.ToString() + " - " + m.Name + " - "));

            foreach (var manifest in mfs)
            {
                this.CheckModel(manifest);
            }
        }

        private void CheckModel(ModelManifest modelManifest)
        {
            bool dependencyAdded = false;
            var elementDatas = db.ModelElementDatas.Where(d => d.ModelId == modelManifest.ModelId).ToList();

            foreach (var elementData in elementDatas)
            {
                //Check if element handle exists in any other model in layer below it, but above the min layerId
                var otherDatas = db.ModelElementDatas
                                   .Where(d => (d.ElementHandle == elementData.ElementHandle 
                                                    //|| (d.ParentHandle == elementData.ParentHandle && elementData.ParentHandle != 0)
                                                    || (d.ElementHandle == elementData.ParentHandle && elementData.ParentHandle == 0)
                                                    ) 
                                                    && d.ModelId != elementData.ModelId
                                                    && d.LayerId <= elementData.LayerId
                                                    && d.LayerId >= this._minLayerId);
                if (otherDatas != null && otherDatas.Count() > 0)
                {
                    List<ModelElement> elementTree = new List<ModelElement>();
                    var element = db.ModelElements.Where(e => e.ElementHandle == elementData.ElementHandle).FirstOrDefault();
                    elementTree.Add(element);
                    //Find all the models this element is dependent on
                    var parentHandle = elementData.ParentHandle;
                    var parentElementHandle = elementData.ElementHandle;
                    while (parentHandle > 0)
                    {
                        var parentElement = db.ModelElements.Where(m => m.ElementHandle == parentHandle).FirstOrDefault();
                        elementTree.Add(parentElement);
                        parentHandle = parentElement.ParentHandle;
                        parentElementHandle = parentElement.ElementHandle;
                    }

                    elementTree.Reverse();
                    var elementPath = this.GetElementPath(elementTree);

                    bool consolePrint = false;
                    
                    foreach (var otherData in otherDatas)
                    {
                        if (ModelDependency.Exists(this.Dependencies, elementData.ModelId, otherData.ModelId) == false)
                        {
                            var manifest = this.db.ModelManifests.FirstOrDefault(m => m.ModelId == otherData.ModelId);

                            if (consolePrint == false)
                            {
                                consolePrint = true;
                                Console.WriteLine("Element in Model " + modelManifest.Name + ", Layer: " + this.GetLayerName(elementData.LayerId) + ": ");
                                Console.WriteLine(elementPath);
                                Console.WriteLine("is dependent on element in the");
                            }
                            Console.WriteLine("Layer: " + this.GetLayerName(otherData.LayerId) + " ,Model: " + manifest.Name);

                            var dependency = new ModelDependency(ref db, elementData.ModelId, otherData.ModelId);
                            this.Dependencies.Add(dependency);
                            dependencyAdded = true;
                        }
                    }
                }

            }

            if (dependencyAdded == false)
            {
                //Add a blank one
                this.Dependencies.Add(new ModelDependency(ref db, modelManifest.ModelId, 0));
            }
        }

        private string GetElementPath(List<ModelElement> elementTree)
        {
            StringBuilder elementPath = new StringBuilder();
            foreach (var element in elementTree)
            {
                //Is this the Parent? then get the element type
                if (element.ParentHandle == 0)
                {
                    var elementType = this.db.ElementTypes.Where(e => e.ElementType1 == element.ElementType).First();
                    elementPath.Append(elementType.TreeNodeName);
                }
                
                elementPath.Append(@"\");
                elementPath.Append(element.Name);
            }
            return elementPath.ToString();
        }

        private string GetLayerName(int layerId)
        {
            var layer = this.db.Layers.First(l => l.Id == layerId);
            return layer.Name;
        }

    }
}
