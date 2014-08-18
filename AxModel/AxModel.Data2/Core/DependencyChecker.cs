/*
 * AX 2012 – Model dependencies and Install Order
 * http://shashidotnet.wordpress.com
 * 
 * Author: Shashi Sadasivan
 * 
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxModel.Data2.Core
{
    public class DependencyChecker
    {
        //private static AX_2012_R2Entities db;
        private int _minLayerId;

        public ConcurrentBag<ModelDependency> Dependencies { get; private set; }

        public DependencyChecker()
        {
            //db = new AX_2012_R2Entities();
            //var connectionString = db.Database.Connection.ConnectionString;
            //Console.WriteLine(connectionString);
            //Console.WriteLine("Using connectiong string from config file....!");
        }

        public void Start(int minLayer = -1)
        {

            this.Dependencies = new ConcurrentBag<ModelDependency>();

            //this._minLayerId = 8;
            //TODO: Set this via code if specified
            if (minLayer <= 0)
                this._minLayerId = Properties.Settings.Default.MinApplicationLayer;
            else
                this._minLayerId = minLayer;

            var allModels = Data.AxModelDataProvider.GetModels();

            var models = Data.AxModelDataProvider.GetModels().Where(m => m.LayerId >= this._minLayerId).ToList();

            Console.WriteLine("List of Models:");
            models.ForEach(m => {
                var layer = Data.AxModelDataProvider.GetLayer(m.LayerId);
                Console.WriteLine(m.Id.ToString() + " : " + layer.Name + " " + m.LayerId);
            });



            var mfs = new List<ModelManifest>();
            models.ForEach(m =>
            {
                var manifest = Data.AxModelDataProvider.GetManifest(m.Id);
                mfs.Add(manifest);
            });
            //mfs = mfs.Where(m => m.Model.LayerId >= this._minLayerId).OrderBy(m => m.ModelId).ToList();
            mfs = mfs.OrderBy(m => m.ModelId).ToList();
            Console.WriteLine("List of Models (ordered by modelid):");
            mfs.ForEach(m =>
                Console.WriteLine(m.ModelId.ToString() + " - " + m.Name + " - "));

            List<Task> taskList = new List<Task>();
            mfs.ForEach(mf => taskList.Add(this.CheckModel(mf)));
            Task.WaitAll(taskList.ToArray());

            //foreach (var manifest in mfs)
            //{
            //    await this.CheckModel(manifest);
            //}
        }

        private async Task CheckModel(ModelManifest modelManifest)
        {
            bool dependencyAdded = false;
            //var elementDatas = db.ModelElementDatas.Where(d => d.ModelId == modelManifest.ModelId).ToList();
            //var elementDatas = this.GetModelElementData(modelManifest.ModelId);
            var elementDatas = Data.AxModelDataProvider.GetModelElementData(modelManifest.ModelId);

            foreach (var elementData in elementDatas)
            {
                //Check if element handle exists in any other model in layer below it, but above the min layerId
                //var otherDatas = db.ModelElementDatas
                //                   .Where(d => (d.ElementHandle == elementData.ElementHandle
                //                       //|| (d.ParentHandle == elementData.ParentHandle && elementData.ParentHandle != 0)
                //                                    || (d.ElementHandle == elementData.ParentHandle && elementData.ParentHandle == 0)
                //                                    )
                //                                    && d.ModelId != elementData.ModelId
                //                                    && d.LayerId <= elementData.LayerId
                //                                    && d.LayerId >= this._minLayerId);
                var otherDatas = Data.AxModelDataProvider.GetModelElementDataForElementData(elementData, this._minLayerId);

                //int i = 100;
                if (otherDatas != null && otherDatas.Count() > 0)
                {
                    List<ModelElement> elementTree = new List<ModelElement>();
                    //var element = db.ModelElements.Where(e => e.ElementHandle == elementData.ElementHandle).FirstOrDefault();
                    var element = Data.AxModelDataProvider.GetModelElement(elementData.ElementHandle);
                    elementTree.Add(element);
                    //Find all the models this element is dependent on
                    var parentHandle = elementData.ParentHandle;
                    var parentElementHandle = elementData.ElementHandle;
                    while (parentHandle > 0)
                    {
                        //var parentElement = db.ModelElements.Where(m => m.ElementHandle == parentHandle).FirstOrDefault();
                        var parentElement = Data.AxModelDataProvider.GetModelElement(parentHandle);
                        elementTree.Add(parentElement);
                        parentHandle = parentElement.ParentHandle;
                        parentElementHandle = parentElement.ElementHandle;
                    }

                    elementTree.Reverse();
                    var elementPath = this.GetElementPath(elementTree);

                    bool consolePrint = false;

                    await Task.Run(() =>
                    {
                        foreach (var otherData in otherDatas)
                        {
                            if (ModelDependency.Exists(this.Dependencies, elementData.ModelId, otherData.ModelId) == false)
                            {
                                //var elementLayer = this.GetLayer(elementData.LayerId);
                                var elementLayer = Data.AxModelDataProvider.GetLayer(elementData.LayerId);
                                //var baseLayer = this.GetLayer(otherData.LayerId);
                                var baseLayer = Data.AxModelDataProvider.GetLayer(otherData.LayerId);
                                //var baseModelManifest = this.GetManifest(otherData.ModelId);
                                var baseModelManifest = Data.AxModelDataProvider.GetManifest(otherData.ModelId);

                                //var manifest = this.db.ModelManifests.FirstOrDefault(m => m.ModelId == otherData.ModelId);
                                var manifest = Data.AxModelDataProvider.GetManifest(otherData.ModelId);

                                if (consolePrint == false)
                                {
                                    consolePrint = true;
                                    //Console.WriteLine("Element in Model " + modelManifest.Name + ", Layer: " + this.GetLayerName(elementData.LayerId) + ": ");
                                    Console.WriteLine("Element in Model " + modelManifest.Name + ", Layer: " + elementLayer.Name + ": ");
                                    Console.WriteLine(elementPath);
                                    Console.WriteLine("is dependent on element in the");
                                }
                                //Console.WriteLine("Layer: " + this.GetLayerName(otherData.LayerId) + " ,Model: " + manifest.Name);
                                Console.WriteLine("Layer: " + baseLayer.Name + " ,Model: " + manifest.Name);

                                //var dependency = new ModelDependency(ref db, elementData.ModelId, otherData.ModelId);
                                

                                var dependency = new ModelDependency(elementData.ModelId, otherData.ModelId,
                                        elementData.LayerId, otherData.LayerId,
                                        modelManifest.Name, baseModelManifest.Name,
                                        elementLayer.Name, baseLayer.Name);
                                this.Dependencies.Add(dependency);
                                dependencyAdded = true;
                            }
                        }
                    });
                }

                
            }

            if (dependencyAdded == false)
            {
                //Add a blank one
                this.Dependencies.Add(new ModelDependency(modelManifest.ModelId, 0));
            }

            return;
        }

        //private static object _getManifestLockObject = new object();
        //private ModelManifest GetManifest(int modelId)
        //{
        //    ModelManifest manifest = null;
        //    lock (_getManifestLockObject)
        //    {
        //        //manifest = db.ModelManifests.FirstOrDefault(m => m.ModelId == modelId);
        //    }
        //    return manifest;    
        //}

        //private static object _getModelElementDataLockObject = new object();
        //private List<ModelElementData> GetModelElementData(int modelId)
        //{
        //    List<ModelElementData> elementData = null;
        //    lock (_getModelElementDataLockObject)
        //    {
        //        elementData = db.ModelElementDatas.Where(d => d.ModelId == modelId).ToList();
        //    }
        //    return elementData;
        //}

        //private static object _getLayerLockObject = new object();
        //private Layer GetLayer(int layerId)
        //{
        //    Layer layer = null;
        //    lock (_getLayerLockObject)
        //    {
        //        layer = db.Layers.FirstOrDefault(d => d.Id == layerId);
        //    }
        //    return layer;
        //}

        private string GetElementPath(List<ModelElement> elementTree)
        {
            StringBuilder elementPath = new StringBuilder();
            foreach (var element in elementTree)
            {
                //Is this the Parent? then get the element type
                if (element.ParentHandle == 0)
                {
                    //var elementType = db.ElementTypes.Where(e => e.ElementType1 == element.ElementType).First();
                    var elementType = Data.AxModelDataProvider.GetElementType(element.ElementType);
                    elementPath.Append(elementType.TreeNodeName);
                }

                elementPath.Append(@"\");
                elementPath.Append(element.Name);
            }
            return elementPath.ToString();
        }

        private string GetLayerName(int layerId)
        {
            //var layer = db.Layers.First(l => l.Id == layerId);
            var layer = Data.AxModelDataProvider.GetLayer(layerId);
            return layer.Name;
        }
    }
}
