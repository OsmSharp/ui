using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.TSP.Problems
{
    /// <summary>
    /// A general TSP problem with it's weights in a matrix.
    /// </summary>
    public class MatrixProblem : IProblem
    {
        /// <summary>
        /// Hold the symmetric flag.
        /// </summary>
        private bool _symmetric;

        /// <summary>
        /// Holds the euclidean flag.
        /// </summary>
        private bool _euclidean;

        /// <summary>
        /// Holds the weights.
        /// </summary>
        private float[][] _weights;

        /// <summary>
        /// Holds the first customer.
        /// </summary>
        private int? _first;

        /// <summary>
        /// Holds the last customer.
        /// </summary>
        private int? _last;

        /// <summary>
        /// Creates a new matrix problem.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="symmetric"></param>
        public MatrixProblem(float[][] weights
            , bool symmetric)
        {
            _symmetric = symmetric;
            _weights = weights;
            //_first = 0;
            //_last = 0;
            _euclidean = false; // assume false just to be safe.
        }

        /// <summary>
        /// Creates a new matrix problem.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="symmetric"></param>
        public MatrixProblem(float[][] weights
            , bool symmetric
            , bool euclidean)
        {
            _symmetric = symmetric;
            _weights = weights;
            _first = 0;
            _last = 0;
            _euclidean = euclidean;
        }

        /// <summary>
        /// Creates a new matrix problem.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="symmetric"></param>
        public MatrixProblem(float[][] weights
            , bool symmetric
            , int first
            , int? last)
        {
            _symmetric = symmetric;
            _weights = weights;
            _first = first;
            _last = last;
            _euclidean = false; // assume false just to be safe.
        }

        /// <summary>
        /// Creates a new matrix problem.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="symmetric"></param>
        public MatrixProblem(float[][] weights
            , bool symmetric
            , bool euclidean
            , int first
            , int last)
        {
            _symmetric = symmetric;
            _weights = weights;
            _first = first;
            _last = last;
            _euclidean = euclidean;
        }


        /// <summary>
        /// Returns the weight between two customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public float Weight(int from, int to)
        {
            return _weights[from][to];
        }

        /// <summary>
        /// Returns true if the problem is symmetric.
        /// </summary>
        public bool Symmetric
        {
            get 
            {
                return _symmetric;
            }
        }

        /// <summary>
        /// Returns true if the problem is euclidean.
        /// </summary>
        public bool Euclidean
        {
            get
            {
                return _euclidean;
            }
        }

        /// <summary>
        /// Returns the size of the problem.
        /// </summary>
        public int Size
        {
            get 
            {
                return _weights.GetLength(0);
            }
        }

        /// <summary>
        /// Returns the first customer.
        /// </summary>
        public int? First
        {
            get 
            { 
                return _first; 
            }
        }

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        public int? Last
        {
            get 
            {
                return _last;
            }
        }

        /// <summary>
        /// Returns the actual weight matrix.
        /// </summary>
        public float[][] WeightMatrix
        {
            get 
            { 
                return _weights; 
            }
        }

        #region Nearest Neighbour

        /// <summary>
        /// Keeps the nearest neighbour list.
        /// </summary>
        private HashSet<int>[] _neighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public HashSet<int> Get10NearestNeighbours(int v)
        {
            if (_neighbours == null)
            {
                _neighbours = new HashSet<int>[this.Size];
            }
            HashSet<int> result = _neighbours[v];
            if (result == null)
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
                        if (result.Count < 10)
                        {
                            result.Add(customer);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                _neighbours[v] = result;
            }
            return result;
        }

        #endregion
    }
}
