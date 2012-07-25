using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.MultiSalesman.Problems;
using Tools.Math.Units.Time;
using Tools.Math.TSP.Problems;
using Tools.Math.VRP.Core;

namespace Osm.Routing.Core.VRP.WithDepot.MinimaxTime.Genetic
{
    /// <summary>
    /// The problem description for the genetic algorithm.
    /// </summary>
    internal class Problem : IProblemWeights, Tools.Math.AI.Genetic.IProblem
    {
        private IProblemWeights _weights;


    
        public Problem(ICollection<int> depots,ICollection<int> customers, IProblemWeights weights)
        {
            this.Depots = depots;
            this.Customers = customers;

            _weights = weights;
        }

        public ICollection<int> Depots { get; private set; }

 
        public ICollection<int> Customers { get; private set; }

        public int Size
        {
            get
            {
                return _weights.Size;
            }
        }

        public IProblemWeights Weights
        {
            get
            {
                return _weights;
            }
        }

        public float Weight(int from, int to)
        {
            return _weights.Weight(from, to);
        }

        public bool Symmetric
        {
            get
            {
                return false;
            }
        }

        public bool Euclidean
        {
            get 
            {
                return false;
            }
        }

        public float[][] WeightMatrix
        {
            get 
            { 
                return _weights.WeightMatrix; 
            }
        }
    }
}
