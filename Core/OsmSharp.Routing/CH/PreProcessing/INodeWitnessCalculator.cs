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

namespace OsmSharp.Routing.CH.PreProcessing
{
    /// <summary>
    /// A witness calculator.
    /// </summary>
    public interface INodeWitnessCalculator
    {
        /// <summary>
        /// Return true if a witness exists for the given graph vertex 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="via"></param>
        /// <param name="weight"></param>
        /// <param name="max_settles"></param>
        /// <returns></returns>
        bool Exists(uint from, uint to, uint via, float weight, int max_settles);
    }
}
