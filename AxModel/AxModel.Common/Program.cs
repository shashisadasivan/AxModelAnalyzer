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

namespace AxModel.Common
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            using (var db = new AxModel.Data.AX_2012_R2_modelEntities())
            {
                //var layers = db.Layers.ToList();
                //layers.ForEach(l =>
                //    Console.WriteLine(l.Name + " : " + l.Id.ToString()));
                int minLayerId = 8;
                var models = db.Models.Where(m => m.LayerId >= minLayerId).ToList();
                models.ForEach(m =>
                    Console.WriteLine(m.Id.ToString() + " : " + m.Layer.Name + " " + m.LayerId));



                var mfs = db.ModelManifests.Where(m => m.Model.LayerId >= minLayerId).OrderBy(m => m.ModelId).ToList();

                mfs.ForEach(m =>
                    Console.WriteLine(m.ModelId.ToString() + " - " + m.Name + " - "));
            }
             */

            RunData2();
            return;

            /*
            Data.ModelDependency.Checker checker = null;
            Console.Write("Database server: ");
            var dbServer = Console.ReadLine();
            if (String.IsNullOrEmpty(dbServer) == true)
            {
                checker = new Data.ModelDependency.Checker();
            }
            else
            {
                Console.Write("Database name (model): ");
                var dbName = Console.ReadLine();
                checker = new Data.ModelDependency.Checker(dbServer, dbName);
            }
            DateTime startDateTime = DateTime.Now;
            Console.WriteLine(startDateTime.ToLongTimeString());
            //var checker = new Data.ModelDependency.Checker();
            checker.Start();
            var lines = Data.ModelDependency.ModelDependency.GetDependenciesCSV(checker.Dependencies);
            lines.ForEach(s => Console.WriteLine(s));

            //Get the sequence:
            Data.ModelDependency.ModelSequencer sequencer = new Data.ModelDependency.ModelSequencer(checker.Dependencies.ToList());
            var models = sequencer.GetModels();
            lines = Data.ModelDependency.ModelSequenceData.ToCSV(models);
            Console.WriteLine("Sequence....");
            lines.ForEach(s =>
                Console.WriteLine(s));
            Console.WriteLine(startDateTime.ToLongTimeString());
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            */
        }

        private static void RunData2()
        {
            Data2.Core.DependencyChecker checker = null;
            Console.Write("Database server: ");
            var dbServer = Console.ReadLine();
            if (String.IsNullOrEmpty(dbServer) == true)
            {
                checker = new Data2.Core.DependencyChecker();
                Data2.Data.AxModelDataProvider provider = new Data2.Data.AxModelDataProvider();
            }
            else
            {
                Console.Write("Database name (model): ");
                var dbName = Console.ReadLine();
                checker = new Data2.Core.DependencyChecker();
                Data2.Data.AxModelDataProvider provider = new Data2.Data.AxModelDataProvider(new Data2.Data.AxModelDataSettings() { DbServer = dbServer, DbName = dbName });

            }

            Console.WriteLine(DateTime.Now.ToLongTimeString());

            //var checker = new Data.ModelDependency.Checker();
            
            
            checker.Start();
            Console.WriteLine();
            Console.WriteLine("Dependencies...");
            var lines = Data2.Core.ModelDependency.GetDependenciesCSV(checker.Dependencies.ToList());
            lines.ForEach(s => Console.WriteLine(s));

            //Get the sequence:
            var sequencer = new Data2.Core.ModelSequencer(checker.Dependencies.ToList());
            var models = sequencer.GetModels();
            lines = Data2.Core.ModelSequenceData.ToCSV(models);
            Console.WriteLine();
            Console.WriteLine("Sequence....");
            Console.WriteLine("--------------------");
            lines.ForEach(s =>
                Console.WriteLine(s));
            
            Console.WriteLine("--------------------");
            Console.WriteLine(DateTime.Now.ToLongTimeString());
        }
    }
}
