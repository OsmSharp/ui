using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Osm.Routing.Interpreter;
using OsmSharp.Osm.Routing.Data;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Route;
using OsmSharp.Osm.Routing.Core.TSP.Genetic;
using OsmSharp.Routing.Core.Graph.Router.Dykstra;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Osm.Data.Core.Processor.Progress;

namespace OsmSharpService.Core.Routing
{
    /// <summary>
    /// Processes routing service requests.
    /// </summary>
    public class RoutingOperationProcessor : IProcessor
    {
        /// <summary>
        /// Holds routing data.
        /// </summary>
        private MemoryRouterDataSource<OsmEdgeData> _data;

        /// <summary>
        /// Holds the routing interpreter.
        /// </summary>
        private OsmRoutingInterpreter _interpreter;

        /// <summary>
        /// Processes a request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object ProcessRoutingResource(RoutingOperation request)
        {
            // create the response object.
            RoutingResponse response = new RoutingResponse();

            if (!this.IsReady())
            { // return that the service is not ready.
                response.Status = RoutingResponseStatus.Failed;
                response.StatusMessage = "Service is not ready!";
                return response;
            }

            if (request.Hooks == null || request.Hooks.Length == 0)
            { // there are no hooks!
                response.Status = RoutingResponseStatus.Failed;
                response.StatusMessage = "No hooks found!";
                return response;
            }

            try
            {
                // resolve the points and do the routing.
                Router<OsmEdgeData> router = new Router<OsmEdgeData>(
                    _data, _interpreter, new DykstraRoutingBinairyHeap<OsmEdgeData>(
                        _data.TagsIndex));

                // create the coordinates list.
                GeoCoordinate[] coordinates = new GeoCoordinate[request.Hooks.Length];
                for (int idx = 0; idx < request.Hooks.Length; idx++)
                {
                    coordinates[idx] = new GeoCoordinate(
                        request.Hooks[idx].Latitude, request.Hooks[idx].Longitude);
                }

                // resolve all.
                RouterPoint[] router_points = router.Resolve(coordinates);

                // add a tag for each point.
                List<RoutingHook> unroutable_hooks = new List<RoutingHook>();
                for (int idx = 0; idx < request.Hooks.Length; idx++)
                {
                    if (router_points[idx] != null)
                    {
                        router_points[idx].Tags.Add(new KeyValuePair<string, string>(
                            "id", request.Hooks[idx].Id.ToString()));
                    }
                    else
                    { // the point is not connected, add to the unroutable hooks.
                        unroutable_hooks.Add(request.Hooks[idx]);
                    }
                }

                // check connectivity.
                List<RouterPoint> routable_points = new List<RouterPoint>();
                for (int idx = 0; idx < request.Hooks.Length; idx++)
                {
                    if (router_points[idx] != null &&
                        router.CheckConnectivity(router_points[idx], 200))
                    { // the point is connected.
                        routable_points.Add(router_points[idx]);
                    }
                    else
                    { // the point is not connected, add to the unroutable hooks.
                        unroutable_hooks.Add(request.Hooks[idx]);
                    }
                }

                // do the routing.
                switch (request.Type)
                {
                    case RoutingOperationType.ManyToMany:
                        response.Weights = router.CalculateManyToManyWeight(routable_points.ToArray(),
                            routable_points.ToArray());
                        break;
                    case RoutingOperationType.Regular:
                        OsmSharpRoute route = null;
                        for (int idx = 0; idx < routable_points.Count - 1; idx++)
                        {
                            OsmSharpRoute current = router.Calculate(
                                routable_points[idx], routable_points[idx + 1]);

                            if (route == null)
                            {
                                route = current;
                            }
                            else
                            {
                                route = OsmSharpRoute.Concatenate(route, current);
                            }
                        }
                        response.Route = route;
                        break;
                    case RoutingOperationType.TSP:
                        RouterTSPAEXGenetic<RouterPoint> tsp_solver = new RouterTSPAEXGenetic<RouterPoint>(
                            router, 300, 300);
                        response.Route = tsp_solver.CalculateTSP(routable_points.ToArray());
                        break;
                }

                // set the unroutable hooks.
                response.UnroutableHooks = unroutable_hooks.ToArray();

                // report succes!
                response.Status = RoutingResponseStatus.Success;
                response.StatusMessage = string.Empty;
            }
            catch (Exception ex)
            { // any exception was thrown.
                response.Status = RoutingResponseStatus.Failed;
                response.StatusMessage = ex.Message;
            }
            return response;
        }

        #region Singleton

        /// <summary>
        /// Holds the instance.
        /// </summary>
        private static RoutingOperationProcessor _instance;

        /// <summary>
        /// Returns an instance of this processor.
        /// </summary>
        /// <returns></returns>
        public static RoutingOperationProcessor GetInstance()
        {
            if (_instance == null)
            { // create the instance.
                _instance = new RoutingOperationProcessor();
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
            return _data != null;
        }

        /// <summary>
        /// Stops this processor.
        /// </summary>
        public void Stop()
        {
            _data = null;
        }

        #region Preparation

        /// <summary>
        /// Prepares the router.
        /// </summary>
        private void PrepareRouter()
        {
            // initialize the interpreters.
            _interpreter =
                new  OsmRoutingInterpreter();

            string file = System.Configuration.ConfigurationManager.AppSettings["pbf_file"];

            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<OsmEdgeData> data =
                new MemoryRouterDataSource<OsmEdgeData>(tags_index);
            OsmEdgeDataGraphProcessingTarget target_data = new OsmEdgeDataGraphProcessingTarget(
                data, _interpreter, data.TagsIndex);
            PBFDataProcessorSource data_processor_source = new PBFDataProcessorSource((new FileInfo(
                file)).OpenRead());
            ProgressDataProcessorSource progress_source = new ProgressDataProcessorSource(data_processor_source);
            target_data.RegisterSource(progress_source);
            target_data.Pull();

            _data = data; // only set the data property here now after pre-processing!
        }

        #endregion
    }
}
