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
using OsmSharpService.Core.Routing.Primitives;
using OsmSharp.Routing.Core.Graph.DynamicGraph.SimpleWeighed;

namespace OsmSharpService.Core.Routing
{
    /// <summary>
    /// Processes routing service requests.
    /// </summary>
    public class OperationProcessor : IProcessor
    {
        /// <summary>
        /// Holds routing data.
        /// </summary>
        private MemoryRouterDataSource<SimpleWeighedEdge> _data;

        /// <summary>
        /// Holds the routing interpreter.
        /// </summary>
        private OsmRoutingInterpreter _interpreter;

        /// <summary>
        /// Processes a routing operation.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public object ProcessRoutingOperation(RoutingOperation operation)
        {
            // create the response object.
            RoutingResponse response = new RoutingResponse();

            if (!this.IsReady())
            { // return that the service is not ready.
                response.Status = OsmSharpServiceResponseStatusEnum.Failed;
                response.StatusMessage = "Service is not ready!";
                return response;
            }

            if (operation.Hooks == null || operation.Hooks.Length == 0)
            { // there are no hooks!
                response.Status = OsmSharpServiceResponseStatusEnum.Failed;
                response.StatusMessage = "No hooks found!";
                return response;
            }

            try
            {
                // resolve the points and do the routing.
                Router<SimpleWeighedEdge> router = new Router<SimpleWeighedEdge>(
                    _data, _interpreter, new DykstraRoutingLive(
                        _data.TagsIndex));

                // create the coordinates list.
                GeoCoordinate[] coordinates = new GeoCoordinate[operation.Hooks.Length];
                for (int idx = 0; idx < operation.Hooks.Length; idx++)
                {
                    coordinates[idx] = new GeoCoordinate(
                        operation.Hooks[idx].Latitude, operation.Hooks[idx].Longitude);
                }

                // resolve all.
                RouterPoint[] router_points = router.Resolve(operation.Vehicle, coordinates);

                // add a tag for each point.
                List<RoutingHook> unroutable_hooks = new List<RoutingHook>();
                for (int idx = 0; idx < operation.Hooks.Length; idx++)
                {
                    if (router_points[idx] != null)
                    {
                        router_points[idx].Tags.Add(new KeyValuePair<string, string>(
                            "id", operation.Hooks[idx].Id.ToString()));
                    }
                    else
                    { // the point is not connected, add to the unroutable hooks.
                        unroutable_hooks.Add(operation.Hooks[idx]);
                    }
                }

                // check connectivity.
                List<RouterPoint> routable_points = new List<RouterPoint>();
                for (int idx = 0; idx < operation.Hooks.Length; idx++)
                {
                    if (router_points[idx] != null &&
                        router.CheckConnectivity(operation.Vehicle, router_points[idx], 200))
                    { // the point is connected.
                        routable_points.Add(router_points[idx]);
                    }
                    else
                    { // the point is not connected, add to the unroutable hooks.
                        unroutable_hooks.Add(operation.Hooks[idx]);
                    }
                }

                // do the routing.
                switch (operation.Type)
                {
                    case RoutingOperationType.ManyToMany:
                        response.Weights = router.CalculateManyToManyWeight(operation.Vehicle, routable_points.ToArray(),
                            routable_points.ToArray());
                        break;
                    case RoutingOperationType.Regular:
                        OsmSharpRoute route = null;
                        for (int idx = 0; idx < routable_points.Count - 1; idx++)
                        {
                            OsmSharpRoute current = router.Calculate(operation.Vehicle, 
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
                        response.Route = tsp_solver.CalculateTSP(operation.Vehicle, routable_points.ToArray());
                        break;
                }

                // set the unroutable hooks.
                response.UnroutableHooks = unroutable_hooks.ToArray();

                // report succes!
                response.Status = OsmSharpServiceResponseStatusEnum.Success;
                response.StatusMessage = string.Empty;
            }
            catch (Exception ex)
            { // any exception was thrown.
                response.Status = OsmSharpServiceResponseStatusEnum.Failed;
                response.StatusMessage = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Processes a resolve operation.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public object ProcessResolveResource(ResolveOperation operation)
        {
            // create the response object.
            ResolveResponse response = new ResolveResponse();

            if (!this.IsReady())
            { // return that the service is not ready.
                response.Status = OsmSharpServiceResponseStatusEnum.Failed;
                response.StatusMessage = "Service is not ready!";
                return response;
            }

            if (operation.Hooks == null || operation.Hooks.Length == 0)
            { // there are no hooks!
                response.Status = OsmSharpServiceResponseStatusEnum.Failed;
                response.StatusMessage = "No hooks found!";
                return response;
            }

            try
            {
                // resolve the points and do the routing.
                Router<SimpleWeighedEdge> router = new Router<SimpleWeighedEdge>(
                    _data, _interpreter, new DykstraRoutingLive(
                        _data.TagsIndex));

                // create the coordinates list.
                response.ResolvedHooks = new RoutingHookResolved[operation.Hooks.Length];
                for (int idx = 0; idx < operation.Hooks.Length; idx++)
                {
                    RouterPoint resolved = router.Resolve(operation.Vehicle, new GeoCoordinate(
                        operation.Hooks[idx].Latitude, operation.Hooks[idx].Longitude));

                    if (resolved != null)
                    { // the point was resolved successfully.
                        response.ResolvedHooks[idx] = new RoutingHookResolved()
                        {
                            Id = operation.Hooks[idx].Id,
                            Latitude = (float)resolved.Location.Latitude,
                            Longitude = (float)resolved.Location.Longitude,
                            Succes = true
                        };
                    }
                    else
                    { // the hook was unsuccessfully resolved.
                        response.ResolvedHooks[idx] = new RoutingHookResolved()
                        {
                            Id = operation.Hooks[idx].Id,
                            Succes = false
                        };
                    }
                }

                // report succes!
                response.Status = OsmSharpServiceResponseStatusEnum.Success;
                response.StatusMessage = string.Empty;
            }
            catch (Exception ex)
            { // any exception was thrown.
                response.Status = OsmSharpServiceResponseStatusEnum.Failed;
                response.StatusMessage = ex.Message;
            }
            return response;
        }

        #region Singleton

        /// <summary>
        /// Holds the instance.
        /// </summary>
        private static OperationProcessor _instance;

        /// <summary>
        /// Returns an instance of this processor.
        /// </summary>
        /// <returns></returns>
        public static OperationProcessor GetInstance()
        {
            if (_instance == null)
            { // create the instance.
                _instance = new OperationProcessor();
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
            MemoryRouterDataSource<SimpleWeighedEdge> data =
                new MemoryRouterDataSource<SimpleWeighedEdge>(tags_index);
            SimpleWeighedDataGraphProcessingTarget target_data = new SimpleWeighedDataGraphProcessingTarget(
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
