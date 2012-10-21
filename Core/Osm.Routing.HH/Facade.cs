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
using System.Diagnostics;
using Osm.Routing.HH.Neigbourhoods;
using Osm.Routing.HH.Primitives;

namespace Osm.Routing.HH
{
    public class Facade
    {
        /// <summary>
        /// Pre-processes the given highway hierarchy.
        /// </summary>
        /// <param name="hh"></param>
        /// <param name="progress_reporter"></param>
        public static void PreProcessData(IHighwayHierarchy hh, IHighwayHierarchyProgressReporter progress_reporter, int max_level)
        {
            // clear the target first.
            hh.ClearTarget();

            // calculate level 1.
            int level = 0;

            // calculate per level until no more improvements are made.
            bool succes = true;
            while (succes)
            {
                // report the start of a new level.
                progress_reporter.NewLevel(level);

                // create the construction class.
                NeighhourhoodConstructor construction = new NeighhourhoodConstructor(level, progress_reporter);
                
                // loop over all highway nodes and computer their neighbourhoods and highway edges.
                int progress = 0;
                foreach (HighwayVertex vertex in hh.GetVertices(level))
                {
                    //// report the start of a new vertex.
                    //progress_reporter.StartedVertex(vertex);

                    progress++;
                    succes = true;

                    // compute highway edges and add to highway graph.
                    construction.Construct(hh, vertex);
                }
                
                // increase level.
                level++;

                // calculate core for level 1.
                progress_reporter.StartCore();
                Core.CoreCalculator core_calculator = new Core.CoreCalculator(level, progress_reporter);
                core_calculator.Calculate(hh);
            }
        }
    }
}
