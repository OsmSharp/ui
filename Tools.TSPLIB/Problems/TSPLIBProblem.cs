using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.TSP;
using System.IO;
using System.Text.RegularExpressions;
using Tools.Math.TSP.Problems;

namespace Tools.TSPLIB.Problems
{
    /// <summary>
    /// Represents and parses a TSPLIB problem.
    /// </summary>
    public class TSPLIBProblem : IProblem
    {
        private int _size;

        private float[][] _weights;

        public TSPLIBProblem(string name, string comment, int size, float[][] weights,
            TSPLIBProblemWeightTypeEnum weight_type,
            TSPLIBProblemTypeEnum problem_type)
        {
            this.Name = name;
            this.Type = problem_type;
            this.WeightType = weight_type;
            _weights = weights;
            _size = size;
        }

        public TSPLIBProblemTypeEnum Type { get; private set; }

        public TSPLIBProblemWeightTypeEnum WeightType { get; private set; }
        
        public float[][] WeightMatrix
        {
            get 
            {
                return _weights; 
            }
        }

        public int Size
        {
            get 
            {
                return _size;
            }
        }

        public float Weight(int from, int to)
        {
            return _weights[from][to];
        }


        public int? First
        {
            get 
            { 
                return 0; 
            }
        }

        public int? Last
        {
            get 
            { 
                return 0; 
            }
        }

        public bool Symmetric
        {
            get 
            {
                return this.Type == TSPLIBProblemTypeEnum.TSP; 
            }
        }

        public bool Euclidean
        {
            get 
            { 
                return true; 
            }
        }

        public float Best { get; set; }

        public string Name { get; private set; }

        public string Comment { get; private set; }

        #region Nearest Neighbour

        /// <summary>
        /// Keeps the nearest neighbour list.
        /// </summary>
        private Dictionary<int, HashSet<int>> _neighbours = new Dictionary<int, HashSet<int>>();

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public HashSet<int> Get10NearestNeighbours(int v)
        {
            HashSet<int> result = null;
            if (!_neighbours.TryGetValue(v, out result))
            {
                SortedDictionary<float, List<int>> neighbours = new SortedDictionary<float, List<int>>();
                for (int customer = 0; customer < this.Size; customer++)
                {
                    if (customer != v)
                    {
                        float weight = this.WeightMatrix[v][customer];
                        List<int> customers = null;
                        if (!neighbours.TryGetValue(weight, out customers))
                        {
                            customers = new List<int>();
                            neighbours.Add(weight, customers);
                        }
                        customers.Add(customer);
                    }
                }

                result = new HashSet<int>();
                foreach (KeyValuePair<float, List<int>> pair in neighbours)
                {
                    foreach (int customer in pair.Value)
                    {
                        if (result.Count < 5)
                        {
                            result.Add(customer);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                _neighbours.Add(v, result);
            }
            return result;
        }

        #endregion



        //#region Nearest Neighbour

        ///// <summary>
        ///// Keeps the nearest neighbour list.
        ///// </summary>
        //private HashSet<int>[] _neighbours;

        ///// <summary>
        ///// Generate the nearest neighbour list.
        ///// </summary>
        ///// <param name="customer"></param>
        ///// <returns></returns>
        //public HashSet<int> Get10NearestNeighbours(int v)
        //{
        //    if (_neighbours == null)
        //    {
        //        _neighbours = new HashSet<int>[this.Size];
        //    }
        //    HashSet<int> result = _neighbours[v];
        //    if (result == null)
        //    {
        //        SortedDictionary<float, List<int>> neighbours = new SortedDictionary<float, List<int>>();
        //        for (int customer = 0; customer < this.Size; customer++)
        //        {
        //            if (customer != v)
        //            {
        //                float weight = this.WeightMatrix[v][customer];
        //                List<int> customers = null;
        //                if (!neighbours.TryGetValue(weight, out customers))
        //                {
        //                    customers = new List<int>();
        //                    neighbours.Add(weight, customers);
        //                }
        //                customers.Add(customer);
        //            }
        //        }

        //        result = new HashSet<int>();
        //        foreach (KeyValuePair<float, List<int>> pair in neighbours)
        //        {
        //            foreach (int customer in pair.Value)
        //            {
        //                if (result.Count < 10)
        //                {
        //                    result.Add(customer);
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        _neighbours[v] = result;
        //    }
        //    return result;
        //}

        //#endregion
    }
}
