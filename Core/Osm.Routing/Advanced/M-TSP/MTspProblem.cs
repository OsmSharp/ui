//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Geo;
//using Osm.Core;
//using Osm.Routing.Raw.Route;
//using Tools.Math.VRP.MultiSalesman.Solver;
//using Tools.Math.Units.Time;

//namespace Osm.Routing.Raw.Advanced.M_TSP
//{
//    /// <summary>
//    /// Class describing the multiple travelling salesman problem.
//    /// </summary>
//    internal class MTspProblem : Problem
//    {
//        /// <summary>
//        /// The list of nodes
//        /// </summary>
//        private List<List<float>> _weights;

//        /// <summary>
//        /// Creates a new Tsp problem.
//        /// </summary>
//        /// <param name="router"></param>
//        /// <param name="first"></param>
//        /// <param name="nodes"></param>
//        /// <param name="last"></param>
//        private MTspProblem(int vehicles, int cities, Second minimum, Second maximum,
//            List<List<float>> weights)
//            : base(vehicles, cities, minimum, maximum)
//        {
//            _weights = weights;
//        }

//        /// <summary>
//        /// Creates a tsp problem from the given parameters.
//        /// </summary>
//        /// <param name="router"></param>
//        /// <param name="first"></param>
//        /// <param name="along"></param>
//        /// <param name="last"></param>
//        /// <returns></returns>
//        public static MTspProblem CreateFrom(int vehicles, int cities, Second minimum, Second maximum,
//            List<List<OsmSharpRoute>> routes)
//        {
//            List<List<float>> weights = new List<List<float>>();
//            for (int a = 0; a < routes.Count; a++)
//            {
//                weights.Add(new List<float>());
//                for (int b = 0; b < routes[a].Count; b++)
//                {
//                    weights[a].Add((float)routes[a][b].TotalTime);
//                }
//            }

//            return new MTspProblem(vehicles,cities,minimum, maximum, weights);
//        }

//        /// <summary>
//        /// Calculates the weight between two given points.
//        /// </summary>
//        /// <param name="city1"></param>
//        /// <param name="city2"></param>
//        /// <returns></returns>
//        public override float Weight(int city1, int city2)
//        {
//            // TODO: update this to use more advanced metrics!
//            return _weights[city1][city2];
//        }
//    }
//}
