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
using OsmSharp.Osm.Simple;

namespace OsmSharp.Osm.Data.Core.Processor.Default
{
    /// <summary>
    /// An empty data processing target doing nothing with objects being streamed to it.
    /// </summary>
    public class DataProcessorTargetEmpty : DataProcessorTarget
    {
        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// Applies a change.
        /// </summary>
        /// <param name="change"></param>
        public override void ApplyChange(SimpleChangeSet change)
        {

        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(SimpleNode node)
        {

        }

        /// <summary>
        /// Adds a way.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(SimpleWay way)
        {

        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(SimpleRelation relation)
        {

        }
    }
}
