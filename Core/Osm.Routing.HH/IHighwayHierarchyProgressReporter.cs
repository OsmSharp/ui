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
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.HH.Core;

namespace Osm.Routing.HH
{
    /// <summary>
    /// Interface to give progress.
    /// </summary>
    public interface IHighwayHierarchyProgressReporter
    {
        /// <summary>
        /// Notifies listeners that a new level has been started.
        /// </summary>
        /// <param name="level"></param>
        void NewLevel(int level);

        ///// <summary>
        ///// Notifies listeners that a next vertex has been selected from processing.
        ///// </summary>
        ///// <param name="vertex"></param>
        //void StartedVertex(GraphVertex vertex);

        ///// <summary>
        ///// Notifies listeners that a list of new edges was found.
        ///// </summary>
        ///// <param name="edges"></param>
        //void HighwayEdge(HighwayEdge edges);

        /// <summary>
        /// Notifies listeners that the core calculation has been started.
        /// </summary>
        void StartCore();

        ///// <summary>
        ///// Notifies listeners that edges have been removed.
        ///// </summary>
        ///// <param name="edges"></param>
        //void RemovedFromCore(HighwayEdge edge);

        ///// <summary>
        ///// Notifies listeners that edges have been added.
        ///// </summary>
        ///// <param name="edges"></param>
        //void AddedToCore(HighwayEdge edge);
    }
}
