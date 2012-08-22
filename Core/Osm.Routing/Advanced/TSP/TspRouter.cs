//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Geo;
//using Osm.Routing.Raw.Route;
//using Tools.Math.AI.Genetic;
//using Tools.Math.TSP.Genetic.Solver;
//using Osm.Core;
//using Tools.Core.Progress;
//using Tools.Math.TSP;

//namespace Osm.Routing.Raw.Advanced.TSP
//{
//    public static class TspRouter
//    {
//        /// <summary>
//        /// Calculates a solution to the Tsp Problem.
//        /// </summary>
//        /// <param name="router"></param>
//        /// <param name="points"></param>
//        /// <param name="progress_reporter"></param>
//        /// <returns></returns>
//        public static IList<int> Calculate(Router router, 
//            List<ResolvedPoint> points,
//            IProgressReporter progress_reporter)
//        {
//            // calculate weight matrix.
//            List<List<float>> matrix = router.CalculateManyToManyWeights(points);

//            // create the problem.
//            TspProblem problem = new TspProblem(matrix);

//            // calculate a solution.
//            ISolver solver = Tools.Math.TSP.Main.Facade.CreateSolver(problem);
//            if (solver is IProgressEnabled)
//            {
//                (solver as IProgressEnabled).RegisterProgressReporter(progress_reporter);
//            }
//            IList<int> solution = solver.Solve();
//            if (solver is IProgressEnabled)
//            {
//                (solver as IProgressEnabled).UnRegisterProgressReporter(progress_reporter);
//            }

//            // add the from and to nodes to the solution.
//            solution.Insert(0, 0);
//            solution.Add(points.Count - 1);

//            // create a spacial route.
//            return solution;
//        }
//    }
//}
