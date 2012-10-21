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
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;

namespace Osm.Core
{

    

    /// <summary>
    /// Base class of all osm objects.
    /// 
    /// ChangeSets, Nodes, Ways & Relations.
    /// </summary>    
    
    [Serializable]
    public abstract class OsmBase : IEquatable<OsmBase>
    {
        private long _id;

        internal OsmBase(long id)
        {
            _id = id;

            _tags = new Dictionary<string, string>();
        }

        /// <summary>
        /// The user that created this object
        /// </summary>
        public string User
        {
            get;
            set;
        }
        /// <summary>
        /// The user id.
        /// </summary>
        public long? UserId 
        { 
            get; 
            set; 
        }


        /// <summary>
        /// The id of this object.
        /// </summary>
        public long Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Returns the bounding box for this object.
        /// </summary>
        public abstract GeoCoordinateBox BoundingBox
        {
            get;
        }

        /// <summary>
        /// Returns the type of osm data.
        /// </summary>
        public abstract OsmType Type
        {
            get;
        }

        /// <summary>
        /// Gets/Sets the timestamp.
        /// </summary>
        public DateTime? TimeStamp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the version.
        /// </summary>
        public long? Version
        {
            get;
            set;
        }


        #region Tags

        /// <summary>
        /// Contains the tags of this osm geo object.
        /// </summary>
        private IDictionary<string, string> _tags;
        /// <summary>
        /// Returns the tags dictionary.
        /// </summary>
        public IDictionary<string, string> Tags
        {
            get
            {
                return _tags;
            }
            protected set
            {
                _tags = value;
            }
        }

        #endregion    
            
        /// <summary>
        /// Returns true if a and b represent the same object.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(OsmBase a, OsmBase b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        /// <summary>
        /// Returns true if a and b do not represent the same object.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(OsmBase a, OsmBase b)
        {
            return !(a == b);
        }

        #region IEquatable<OsmBase> Members

        public bool Equals(OsmBase other)
        {
            if (other == null)
            {
                return false;
            }
            if (other._id == this.Id
                && other.Type == this.Type)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode() 
                ^ this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is OsmBase)
            {
                return this.Equals(obj as OsmBase);
            }
            return false;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}:{1}",
                this.Type.ToString(),
                this.Id);
        }
    }
}
