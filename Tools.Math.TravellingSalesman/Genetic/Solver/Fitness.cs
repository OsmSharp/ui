using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Graph;

namespace Tools.Math.TSP.Genetic.Solver
{
    public class Fitness : IComparable
    {
        private float _weight;

        public Fitness(float weigth)
        {
            _weight = weigth;
        }

        public float Weight 
        {
            get
            {
                return _weight;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return -1;
            }
            if(obj is Fitness)
            {
                return _weight.CompareTo((obj as Fitness).Weight);
            }
            throw new InvalidCastException();
        }

        public override string ToString()
        {
            return string.Format("{0}",Weight);
        }
    }
}
