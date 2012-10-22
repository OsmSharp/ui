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

namespace Tools.Math.VRP.MultiSalesman.Genetic
{
    public class Genome : List<int>, IEquatable<Genome>
    {
        public Genome()
        {

        }

        public Genome(IEnumerable<int> collection)
            : base(collection)
        {

        }

        public bool Equals(Genome other)
        {
            if (this.Count != other.Count)
            {
                return false;
            }
            else
            {
                for (int idx = 0; idx < this.Count; idx++)
                {
                    if (this[idx] != other[idx])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
