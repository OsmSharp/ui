// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2013 Abelshausen Ben
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

namespace OsmSharp.Osm.Data.Core.Processor.List
{
    /// <summary>
    /// A data processor target for regular SimpleOsmBase objects.
    /// </summary>
    public class CollectionDataProcessorTarget : DataProcessorTarget
    {
        /// <summary>
        /// Holds the target list.
        /// </summary>
        private ICollection<SimpleOsmGeo> _base_objects;

        /// <summary>
        /// Creates a new collection data processor target.
        /// </summary>
        /// <param name="base_objects"></param>
        public CollectionDataProcessorTarget(ICollection<SimpleOsmGeo> base_objects)
        {
            _base_objects = base_objects;
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// Applies a changeset.
        /// </summary>
        /// <param name="change"></param>
        public override void ApplyChange(Osm.Simple.SimpleChangeSet change)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a node to the target.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(SimpleNode node)
        {
            if (_base_objects == null)
            { // the base object collection is null.
                throw new InvalidOperationException("No target collection set!");
            }

            // add the node to the collection.
            _base_objects.Add(node);
        }

        /// <summary>
        /// Adds a way to the target.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(SimpleWay way)
        {
            if (_base_objects == null)
            { // the base object collection is null.
                throw new InvalidOperationException("No target collection set!");
            }

            // add the way to the collection.
            _base_objects.Add(way);
        }

        /// <summary>
        /// Adds a relation to the target.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(SimpleRelation relation)
        {
            if (_base_objects == null)
            { // the base object collection is null.
                throw new InvalidOperationException("No target collection set!");
            }

            // add the relation to the collection.
            _base_objects.Add(relation);
        }
    }
}
