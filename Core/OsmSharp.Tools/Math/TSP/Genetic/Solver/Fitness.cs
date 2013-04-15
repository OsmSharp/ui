// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
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

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver
{
    /// <summary>
    /// Represents a fitness.
    /// </summary>
    public class Fitness : IComparable
    {
        private double _weight;

        /// <summary>
        /// Creates a new fitness.
        /// </summary>
        /// <param name="weigth"></param>
        public Fitness(double weigth)
        {
            _weight = weigth;
        }

        /// <summary>
        /// The weight.
        /// </summary>
        public double Weight 
        {
            get
            {
                return _weight;
            }
        }

        /// <summary>
        /// Compares fitness.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a description of this weight.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}",Weight);
        }
    }
}
