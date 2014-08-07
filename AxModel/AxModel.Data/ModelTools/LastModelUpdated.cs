using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AxModel.Data.ModelTools
{
    public class LastModelUpdated
    {
        private AX_2012_R2_modelEntities db;

        public LastModelUpdated(string dbServer, string dbName)
        {
            db = DbProvider.GetDb(dbServer, dbName);

            //var modelMfs = db.ModelManifests.OrderBy(mf => mf.).ToList();
            var models = db.Models.OrderBy(m => m.LayerId).ToList();

            foreach (var model in models)
            {
                var modelElement = db.ModelElementDatas.OrderByDescending(ed => ed.MODIFIEDDATETIME).First(ed => ed.ModelId == model.Id);

            }
        }
    }


}
