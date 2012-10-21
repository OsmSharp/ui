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
using Tools.Math.Geo;
using Osm.Core.Factory;

namespace Osm.Core
{
    public enum HighwayTypeEnum
    {
        not_set,
        living_Street,
        residential,
        tertiary,
        secondary,
        primary,
        trunk,
        motorway,
        service,
        proposed,
        cycleway,
        pedestrian,
        others,
        track
    }

    /// <summary>
    /// Way class.
    /// </summary>
    [Serializable]
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

        private string _oldTag = null;
        private HighwayTypeEnum _highwaytype = HighwayTypeEnum.not_set;

        public HighwayTypeEnum HighwayType
        {
            get
            {
                if (!base.Tags.ContainsKey("highway")) return HighwayTypeEnum.not_set;
                if (_oldTag == base.Tags["highway"]) return _highwaytype;
                _oldTag = base.Tags["highway"];
                switch (_oldTag)
                {
                    case "residential":
                        _highwaytype = HighwayTypeEnum.residential;
                        break;
                    case "tertiary":
                        _highwaytype = HighwayTypeEnum.tertiary;
                        break;
                    case "secondary":
                        _highwaytype = HighwayTypeEnum.secondary;
                        break;
                    case "primary":
                    case "primary_link":
                        _highwaytype = HighwayTypeEnum.primary;
                        break;
                    case "trunk":
                    case "trunk_link":
                        _highwaytype = HighwayTypeEnum.trunk;
                        break;
                    case "motorway":
                    case "motorway_link":
                        _highwaytype = HighwayTypeEnum.motorway;
                        break;
                    case "service":
                        _highwaytype = HighwayTypeEnum.service;
                        break;
                    case "living_street":
                    case "unclassified":
                        _highwaytype = HighwayTypeEnum.living_Street;
                        break;
                    case "pedestrian":
                    case "steps":
                    case "path":
                    case "footway":
                        _highwaytype = HighwayTypeEnum.pedestrian;
                        break;
                    case "proposed":
                        _highwaytype = HighwayTypeEnum.proposed;
                        break;
                    case "cycleway":
                        _highwaytype = HighwayTypeEnum.cycleway;
                        break;
                    case "track":
                        _highwaytype = HighwayTypeEnum.track;
                        break;
                    default:
                        _highwaytype = HighwayTypeEnum.others;
                        break;
                }
                return _highwaytype;
            }
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

        public override string ToString()
        {
            return string.Format("http://www.openstreetmap.org/?way={0}",
                this.Id);
        }
    }
}
