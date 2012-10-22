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

namespace Tools.Math.Random
{
    public class RandomGenerator : IRandomGenerator
    {
        private System.Random _random;

        public RandomGenerator()
        {
            _random = new System.Random();
        }

        #region IRandomGenerator Members

        public int Generate(int max)
        {
            return _random.Next(max);
        }

        public double Generate(double max)
        {
            return _random.NextDouble() * max;
        }

        #endregion
    }
}
