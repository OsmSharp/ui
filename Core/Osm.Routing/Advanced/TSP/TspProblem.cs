//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.TSP;
//using Tools.Math.Geo;
//using Osm.Core;
//using Osm.Routing.Raw.Route;
//using Tools.Math.TSP.Problems;

//namespace Osm.Routing.Raw.Advanced.TSP
//{
//    /// <summary>
//    /// Class describing the salesman problem.
//    /// </summary>
//    internal class TspProblem : MatrixProblem
//    {
//        public TspProblem(List<List<float>> weights)
//            :base(TspProblem.Convert(weights),false)
//        {

//        }

//        private static float[][] Convert(List<List<float>> weights)
//        {
//            float[][] weights_array = new float[weights.Count][];
//            for (int i = 0; i < weights.Count; i++)
//            {
//                weights_array[i] = new float[weights.Count];
//                for (int j = 0; j < weights.Count; j++)
//                {
//                    weights_array[i][j] = weights[i][j];
//                }
//            }
//            return weights_array;
//        }
//    }
//}
