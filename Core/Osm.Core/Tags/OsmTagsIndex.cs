using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Core.Collections;

namespace Osm.Routing.CH.PreProcessing.Tags
{
    /// <summary>
    /// An osm tags index.
    /// </summary>
    public class OsmTagsIndex
    {
        /// <summary>
        /// Holds all the tags objects.
        /// </summary>
        private ObjectTable<OsmTags> _tags;

        /// <summary>
        /// Holds the string table.
        /// </summary>
        private ObjectTable<string> _string_table;

        /// <summary>
        /// Creates a new tags index with a given strings table.
        /// </summary>
        /// <param name="string_table"></param>
        public OsmTagsIndex(ObjectTable<string> string_table)
        {
            _string_table = string_table;
            _tags = new ObjectTable<OsmTags>(false);
        }

        /// <summary>
        /// Creates a new tags index.
        /// </summary>
        public OsmTagsIndex()
        {
            _string_table = new ObjectTable<string>(false);
            _tags = new ObjectTable<OsmTags>(false);
        }

        /// <summary>
        /// Returns the tags with the given id.
        /// </summary>
        /// <param name="tags_int"></param>
        /// <returns></returns>
        public IDictionary<string, string> Get(uint tags_int)
        {
            OsmTags osm_tags = _tags.Get(tags_int);
            if (osm_tags != null)
            {
                return osm_tags.GetTags(_string_table);
            }
            return null;
        }

        /// <summary>
        /// Adds tags to this index.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public uint Add(IDictionary<string, string> tags)
        {
            OsmTags osm_tags = OsmTags.CreateFrom(_string_table, tags);
            if (osm_tags != null)
            {
                return _tags.Add(osm_tags);
            }
            throw new ArgumentNullException("tags", "Tags dictionary cannot be null or empty!");
        }

        /// <summary>
        /// Holds tags in a very memory efficient way.
        /// </summary>
        private class OsmTags
        {
            /// <summary>
            /// Holds all the tags.
            /// </summary>
            private uint[,] _tags;

            /// <summary>
            /// Creates a new tags object.
            /// </summary>
            /// <param name="tags"></param>
            private OsmTags(uint[,] tags)
            {
                _tags = tags;
            }

            /// <summary>
            /// Creates a new tags object.
            /// </summary>
            /// <param name="string_table"></param>
            /// <param name="tags"></param>
            /// <returns></returns>
            internal static OsmTags CreateFrom(ObjectTable<string> string_table, IDictionary<string, string> tags)
            {
                if (tags != null && tags.Count > 0)
                {
                    uint[,] tags_int = new uint[tags.Count, 2];
                    int idx = 0;
                    foreach (KeyValuePair<string, string> tag in tags)
                    {
                        tags_int[idx, 0] = string_table.Add(tag.Key);
                        tags_int[idx, 1] = string_table.Add(tag.Value);
                        idx++;
                    }
                    return new OsmTags(tags_int);
                }
                return null;  // don't was space on tags that contain no information.
            }

            /// <summary>
            /// Returns the actual tags.
            /// </summary>
            public IDictionary<string, string> GetTags(ObjectTable<string> string_table)
            {
                Dictionary<string, string> tags = new Dictionary<string, string>();
                for (int idx = 0; idx < this._tags.GetLength(0); idx++)
                {
                    tags.Add(string_table.Get(this._tags[idx, 0]),
                        string_table.Get(this._tags[idx, 1]));
                }
                return tags;
            }

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
                                    if (this._tags[idx1, 0] == other._tags[idx2, 0] &&
                                        this._tags[idx1, 1] == other._tags[idx2, 1])
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
                int hash = _tags.Length;
                foreach (uint value in _tags)
                {
                    hash = hash ^ (int)value;
                }
                return hash;
            }
        }
    }
}
