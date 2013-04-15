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
using OsmSharp.Tools.Math.TSP.Problems;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core;

namespace OsmSharp.Tools.Math.TSP
{
    /// <summary>
    /// Represents an improvement heuristic/solver.
    /// </summary>
    public interface IImprovement
    {
        /// <summary>
        /// Returns the name of the improvement.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Returns true if there was an improvement.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        bool Improve(IProblemWeights problem, IRoute route, out double difference);
    }
}
