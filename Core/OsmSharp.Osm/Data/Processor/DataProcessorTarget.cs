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

namespace OsmSharp.Osm.Data.Core.Processor
{
    /// <summary>
    /// Any target of osm data (Nodes, Ways and Relations).
    /// </summary>
    public abstract class DataProcessorTarget
    {
        /// <summary>
        /// Holds the source for this target.
        /// </summary>
        private DataProcessorSource _source;

        /// <summary>
        /// Creates a new target.
        /// </summary>
        public DataProcessorTarget()
        {

        }

        /// <summary>
        /// Initializes the target.
        /// </summary>
        public abstract void Initialize();
        
        /// <summary>
        /// Applies a change to the target.
        /// </summary>
        /// <param name="change"></param>
        public abstract void ApplyChange(SimpleChangeSet change);

        /// <summary>
        /// Adds a node to the target.
        /// </summary>
        /// <param name="node"></param>
        public abstract void AddNode(SimpleNode node);

        /// <summary>
        /// Adds a way to the target.
        /// </summary>
        /// <param name="way"></param>
        public abstract void AddWay(SimpleWay way);

        /// <summary>
        /// Adds a relation to the target.
        /// </summary>
        /// <param name="relation"></param>
        public abstract void AddRelation(SimpleRelation relation);

        /// <summary>
        /// Registers a source on this target.
        /// </summary>
        /// <param name="source"></param>
        public void RegisterSource(DataProcessorSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Returns the registered source.
        /// </summary>
        protected DataProcessorSource Source
        {
            get
            {
                return _source;
            }
        }

        /// <summary>
        /// Pulls the changes from the source to this target.
        /// </summary>
        public void Pull()
        {
            _source.Initialize();
            this.Initialize();
            while (_source.MoveNext())
            {
                object source_object = _source.Current();
                if (source_object is SimpleNode)
                {
                    this.AddNode(source_object as SimpleNode);
                }
                else if (source_object is SimpleWay)
                {
                    this.AddWay(source_object as SimpleWay);
                }
                else if (source_object is SimpleRelation)
                {
                    this.AddRelation(source_object as SimpleRelation);
                }
            }
            this.Close();
        }

        /// <summary>
        /// Closes the current target.
        /// </summary>
        public virtual void Close()
        {

        }
    }
}
