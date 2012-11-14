using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routing.Core;
using Osm.Routing.Data;
using Osm.Core;
using Routing.Core.Router.Memory;
using Osm.Routing.Data.Processing;
using Osm.Data.PBF.Raw.Processor;
using Osm.Data.Core.Processor.Progress;
using System.IO;
using Osm.Routing.Interpreter;

namespace OsmSharpService.Core.Routing
{
    /// <summary>
    /// Processes routing service requests.
    /// </summary>
    public class RoutingResourceProcessor : IProcessor
    {
        /// <summary>
        /// Holds routing data.
        /// </summary>
        private Router<OsmEdgeData> _router;

        /// <summary>
        /// Processes a request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object ProcessRoutingResource(RoutingResource request)
        {
            return null;
        }

        #region Singleton
        
        /// <summary>
        /// Holds the instance.
        /// </summary>
        private static IProcessor _instance;

        /// <summary>
        /// Returns an instance of this processor.
        /// </summary>
        /// <returns></returns>
        public static IProcessor GetInstance()
        {
            if (_instance == null)
            { // create the instance.
                _instance = new RoutingResourceProcessor();
            }
            return _instance;
        }

        #endregion

        /// <summary>
        /// Starts this processor.
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            System.Threading.Thread thread = new System.Threading.Thread(
                new System.Threading.ThreadStart(PrepareRouter));
            thread.Start();

            return true;
        }

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        /// <returns></returns>
        public bool IsReady()
        {
            return _router != null;
        }

        /// <summary>
        /// Stops this processor.
        /// </summary>
        public void Stop()
        {
            _router = null;
        }

        #region Preparation

        /// <summary>
        /// Prepares the router.
        /// </summary>
        private void PrepareRouter()
        {
            // initialize the interpreters.
            OsmRoutingInterpreter interpreter =
                new Osm.Routing.Interpreter.OsmRoutingInterpreter();

            string file = System.Configuration.ConfigurationManager.AppSettings["pbf_file"];

            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<OsmEdgeData> data =
                new MemoryRouterDataSource<OsmEdgeData>(tags_index);
            OsmEdgeDataGraphProcessingTarget target_data = new OsmEdgeDataGraphProcessingTarget(
                data, interpreter, data.TagsIndex);
            PBFDataProcessorSource data_processor_source = new PBFDataProcessorSource((new FileInfo(
                file)).OpenRead());
            ProgressDataProcessorSource progress_source = new ProgressDataProcessorSource(data_processor_source);
            //ProgressDataProcessorTarget processor_target = new ProgressDataProcessorTarget(
            //    target_data);
            //DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            //sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(progress_source);
            target_data.Pull();

            // initialize the router.
            _router =
                new Router<OsmEdgeData>(
                    data, interpreter);
        }

        #endregion
    }
}
