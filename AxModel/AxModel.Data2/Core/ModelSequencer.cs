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

namespace AxModel.Data2.Core
{
    public class ModelSequencer
    {
        public List<ModelDependency> Dependencies { get; private set; }

        public ModelSequencer(List<ModelDependency> dependencies)
        {
            this.Dependencies = dependencies;
        }

        public List<ModelSequenceData> GetModels()
        {
            var sequence = new List<ModelSequenceData>();

            var sequencer = this.Dependencies
                                            .Where(d => d.ElementCount > 0)
                                            .OrderBy(d => d.LayerId)
                                            .ThenBy(d => d.BaseLayerId)
                                            .ThenBy(d => d.ModelId)
                                            .ThenBy(d => d.BaseModelId)
                                            .ToList();

            sequencer.ForEach(s =>
            {
                if (sequence.Count(ss => ss.ModelId == s.ModelId) <= 0)
                {
                    //if (s.BaseModelId > 0)
                        sequence.Add(new ModelSequenceData(s.ModelId, s.ModelName, s.LayerId, s.LayerName));
                }
            });

            return sequence;
        }
    }

    public class ModelSequenceData
    {
        public int ModelId { get; private set; }
        public string ModelName { get; private set; }

        public int LayerId { get; private set; }
        public string LayerName { get; private set; }

        public ModelSequenceData(int modelId, string modelName,
                                    int layerId, string layerName)
        {
            this.ModelId = modelId;
            this.ModelName = modelName;

            this.LayerId = layerId;
            this.LayerName = layerName;
        }

        public static List<string> ToCSV(List<ModelSequenceData> sequence)
        {
            var lines = new List<string>();
            lines.Add("Layer Id,Layer,Model Id,Model");

            sequence.ForEach(s =>
                lines.Add(String.Format("{0},{1},{2},{3}",
                s.LayerId, s.LayerName,
                s.ModelId, s.ModelName)));

            return lines;
        }
    }
}
