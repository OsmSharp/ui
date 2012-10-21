// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Core.Route;
using Osm.Core;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core.VRP
{
    public abstract class RouterVRP<ResolvedType>
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Holds the basic router.
        /// </summary>
        private IRouter<ResolvedType> _router;

        /// <summary>
        /// Creates a new VRP router.
        /// </summary>
        /// <param name="router"></param>
        public RouterVRP(IRouter<ResolvedType> router)
        {
            _router = router;
        }

        /// <summary>
        /// Returns the router.
        /// </summary>
        protected IRouter<ResolvedType> Router
        {
            get
            {
                return _router;
            }
        }

        /// <summary>
        /// Calculates a weight matrix for the given array of points.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected float[][] CalculateManyToManyWeigth(ResolvedType[] points)
        {
            return _router.CalculateManyToManyWeight(points, points);
        }

        /// <summary>
        /// Calculates an actual route between two points.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected OsmSharpRoute Calculate(ResolvedType from, ResolvedType to)
        {
            return _router.Calculate(from, to);
        }

        /// <summary>
        /// Constructs the actual routes after a solution was found.
        /// </summary>
        /// <param name="vrp_solution"></param>
        /// <returns></returns>
        protected OsmSharpRoute[] ConstructSolution(int[][] vrp_solution,
            ResolvedType[] depots, ResolvedType[] clients)
        {
            if (depots != null)
            {
                OsmSharpRoute[] solution = new OsmSharpRoute[vrp_solution.Length];

                for (int route_idx = 0; route_idx < vrp_solution.Length; route_idx++)
                {
                    // concatenate the route(s).
                    OsmSharpRoute tsp = null;
                    OsmSharpRoute route;
                    if (depots != null)
                        tsp = _router.Calculate(depots[route_idx], clients[vrp_solution[route_idx][0] - depots.Length]);

                    for (int idx = 0; idx < vrp_solution[route_idx].Length - 1; idx++)
                    {
                        if (vrp_solution[route_idx][idx] - vrp_solution.Length >= clients.Length ||
                            vrp_solution[route_idx][idx + 1] - vrp_solution.Length >= clients.Length)
                            continue;

                        route = _router.Calculate(clients[vrp_solution[route_idx][idx] - depots.Length],
                            clients[vrp_solution[route_idx][idx + 1] - depots.Length]);

                        if (route.Entries != null && route.Entries.Length > 0)
                        {
                            if (tsp == null)
                            { // first route = start
                                tsp = route;
                            }
                            else
                            { // concatenate.
                                tsp = OsmSharpRoute.Concatenate(tsp, route);
                            }
                        }
                    }

                    // concatenate the route from the last to the first point again.
                    if (depots != null)
                    {
                        route = null;
                        try
                        {
                            route = _router.Calculate(clients[vrp_solution[route_idx][vrp_solution[route_idx].Length - 1] - depots.Length],
                                        depots[route_idx]);
                        }
                        catch { }


                        if (route != null && route.Entries != null && route.Entries.Length > 0)
                        {
                            tsp = OsmSharpRoute.Concatenate(tsp, route);
                        }
                    }

                    solution[route_idx] = tsp;
                }
                return solution;
            }
            else
            {
                OsmSharpRoute[] solution = new OsmSharpRoute[vrp_solution.Length];
                for (int route_idx = 0; route_idx < vrp_solution.Length; route_idx++)
                {
                    // concatenate the route(s).
                    OsmSharpRoute tsp = null;
                    OsmSharpRoute route;
                    for (int idx = 0; idx < vrp_solution[route_idx].Length - 1; idx++)
                    {
                        route = _router.Calculate(clients[vrp_solution[route_idx][idx]],
                            clients[vrp_solution[route_idx][idx + 1]]);
                        if (route.Entries.Length > 0)
                        {
                            if (tsp == null)
                            { // first route = start
                                tsp = route;
                            }
                            else
                            { // concatenate.
                                tsp = OsmSharpRoute.Concatenate(tsp, route);
                            }
                        }
                    }

                    // concatenate the route from the last to the first point again.
                    route = _router.Calculate(clients[vrp_solution[route_idx][vrp_solution[route_idx].Length - 1]],
                                clients[vrp_solution[route_idx][0]]);
                    if (route.Entries.Length > 0)
                    {
                        tsp = OsmSharpRoute.Concatenate(tsp, route);
                    }

                    solution[route_idx] = tsp;
                }
                return solution;
            }
        }

        #region Intermidiate Results

        /// <summary>
        /// Delegate to pass on an array of routes.
        /// </summary>
        /// <param name="result"></param>
        public delegate void OsmSharpRoutesDelegate(OsmSharpRoute[] result, Dictionary<int, List<int>> solution);

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        public event OsmSharpRoutesDelegate IntermidiateResult;

        /// <summary>
        /// Returns true when the event has to be raised.
        /// </summary>
        /// <returns></returns>
        protected bool CanRaiseIntermidiateResult()
        {
            return this.IntermidiateResult != null;
        }

        /// <summary>
        /// Raises the intermidiate results event.
        /// </summary>
        /// <param name="result"></param>
        protected void RaiseIntermidiateResult(OsmSharpRoute[] result, Dictionary<int, List<int>> solution)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result, solution);
            }
        }

        #endregion
    }
}
