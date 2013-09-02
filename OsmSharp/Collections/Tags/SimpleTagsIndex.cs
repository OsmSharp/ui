// OsmSharp - OpenStreetMap (OSM) SDK
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

namespace OsmSharp.Collections.Tags
{
    /// <summary>
    /// An osm tags index.
    /// </summary>
    public class SimpleTagsIndex : ITagsIndex
    {
        /// <summary>
        /// Holds all the tags objects.
        /// </summary>
        private readonly ObjectTable<OsmTags> _tagsTable;

        /// <summary>
        /// Creates a new tags index with a given strings table.
        /// </summary>
        public SimpleTagsIndex()
        {
            _tagsTable = new ObjectTable<OsmTags>(true);

            this.Add(new SimpleTagsCollection());
        }

        /// <summary>
        /// Returns the tags with the given id.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public TagsCollection Get(uint tagsId)
        {
            OsmTags osmTags = _tagsTable.Get(tagsId);
            if (osmTags != null)
            {
                return new SimpleTagsCollection(osmTags.Tags);
            }
            return null;
        }

        /// <summary>
        /// Adds tags to this index.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public uint Add(TagsCollection tags)
        {
            var osmTags = new OsmTags(tags);
            if (osmTags != null)
            {
                return _tagsTable.Add(osmTags);
            }
            throw new ArgumentNullException("tags", "Tags dictionary cannot be null or empty!");
        }

        /// <summary>
        /// Holds tags.
        /// </summary>
        public class OsmTags
        {
            /// <summary>
            /// Holds all the tags.
            /// </summary>
            private readonly Tag[] _tags;

            /// <summary>
            /// Creates a new tags object.
            /// </summary>
            /// <param name="tags"></param>
            public OsmTags(IEnumerable<Tag> tags)
            {
                _tags = tags.ToArray();
            }

            /// <summary>
            /// Returns the tags array.
            /// </summary>
            public Tag[] Tags
            {
                get { return _tags; }
            }

            #region Equals

            /// <summary>
            /// Returns true if the objects represent the same information.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (!object.ReferenceEquals(this, obj))
                {
                    if (obj is OsmTags)
                    {
                        OsmTags other = (obj as OsmTags);
                        if (other._tags.Length == this._tags.Length)
                        {
                            // make sure all object in the first are in the second and vice-versa.
                            for (int idx1 = 0; idx1 < this._tags.GetLength(0); idx1++)
                            {
                                bool found = false;
                                for (int idx2 = 0; idx2 < other._tags.GetLength(0); idx2++)
                                {
                                    if (this._tags[idx1].Key == other._tags[idx2].Key &&
                                        this._tags[idx1].Value == other._tags[idx2].Value)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    return false;
                                }
                            }
                            return true; // no loop was done without finding the same key-value pair.
                        }
                    }
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Serves as a hash function.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return _tags.Aggregate(_tags.Length, 
                    (current, tag) => current ^ tag.Key.GetHashCode() ^ tag.Value.GetHashCode());
            }

            #endregion
        }
    }
}