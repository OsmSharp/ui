//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Routing.Raw.Route;
//using Tools.Math.Units.Time;

//namespace Osm.Routing.Raw.Advanced.M_TSP
//{
//    /// <summary>
//    /// Class handling M-TSP calculations.
//    /// </summary>
//    public static class MTspRouter
//    {
//        /// <summary>
//        /// Calculates a solution to the Tsp Problem.
//        /// </summary>
//        /// <param name="router"></param>
//        /// <param name="points"></param>
//        /// <returns></returns>
//        public static IList<IList<int>> Calculate(Router router, List<ResolvedPoint> points, int vehicles, int cities, Second minimum, Second maximum)
//        {
//            // calculate weight matrix.
//            List<List<OsmSharpRoute>> matrix = router.CalculateManyToMany(points);

//            // create the problem.
//            MTspProblem problem = MTspProblem.CreateFrom(vehicles, cities, minimum, maximum, matrix);

//            // calculate a solution.
//            Tools.Math.VRP.MultiSalesman.Solver.Individual solution = Tools.Math.VRP.MultiSalesman.Facade.Calculate(problem,null,null,10);

//            // TODO: convert the solution.

//            // create a spacial route.
//            return null;
//        }
//    }
//}
