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
using OsmSharp.Tools.Collections;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Factory;
using OsmSharp.Osm.Simple;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Way class.
    /// </summary>
    public class Way : OsmGeo
    {
        /// <summary>
        /// Holds the nodes of this way.
        /// </summary>
        private List<Node> _nodes;

        /// <summary>
        /// Creates a new way.
        /// </summary>
        /// <param name="id"></param>
        internal protected Way(long id)
            : base(id)
        {
            _nodes = new List<Node>();
        }

        /// <summary>
        /// Creates a new way using a string table.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="string_table"></param>
        internal protected Way(ObjectTable<string> string_table, long id)
            : base(string_table, id)
        {
            _nodes = new List<Node>();
        }

        /// <summary>
        /// Returns the way type.
        /// </summary>
        public override OsmType Type
        {
            get { return OsmType.Way; }
        }

        /// <summary>
        /// Gets the ordered list of nodes.
        /// </summary>
        public List<Node> Nodes
        {
            get
            {
                return _nodes;
            }
        }

        /// <summary>
        /// Returns all the coordinates in this way in the same order as the nodes.
        /// </summary>
        /// <returns></returns>
        public IList<GeoCoordinate> GetCoordinates()
        {
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();

            for (int idx = 0; idx < this.Nodes.Count; idx++)
            {
                coordinates.Add(this.Nodes[idx].Coordinate);
            }

            return coordinates;
        }

        /// <summary>
        /// Copies all info in this way to the given way without changing the id.
        /// </summary>
        /// <param name="w"></param>
        public void CopyTo(Way w)
        {
            foreach (KeyValuePair<string, string> tag in this.Tags)
            {
                w.Tags.Add(tag.Key, tag.Value);
            }
            w.Nodes.AddRange(this.Nodes);
            w.TimeStamp = this.TimeStamp;
            w.User = this.User;
            w.UserId = this.UserId;
            w.Version = this.Version;
            w.Visible = this.Visible;
        }

        /// <summary>
        /// Returns an exact copy of this way.
        /// 
        /// WARNING: even the id is copied!
        /// </summary>
        /// <returns></returns>
        public Way Copy()
        {
            Way w = new Way(this.Id);
            this.CopyTo(w);
            return w;
        }

        /// <summary>
        /// Returns true if this way contains the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool HasNode(Node node)
        {
            return this.Nodes.Contains(node);
        }

        /// <summary>
        /// Converts this relation into it's simple counterpart.
        /// </summary>
        /// <returns></returns>
        public override SimpleOsmGeo ToSimple()
        {
            SimpleWay way = new SimpleWay();
            way.Id = this.Id;
            way.ChangeSetId = this.ChangeSetId;
            way.Tags = this.Tags;
            way.TimeStamp = this.TimeStamp;
            way.UserId = this.UserId;
            way.UserName = this.User;
            way.Version = (ulong?)this.Version;
            way.Visible = this.Visible;

            way.Nodes = new List<long>();
            foreach (Node node in this.Nodes)
            {
                way.Nodes.Add(node.Id);
            }
            return way;
        }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("http://www.openstreetmap.org/?way={0}",
                this.Id);
        }
    }
}
