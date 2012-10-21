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
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
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
