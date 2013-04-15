using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Collections;
using OsmSharp.Tools.Math;

namespace OsmSharp.Osm
{
    /// <summary>
    /// An osm tags index.
    /// </summary>
    public class OsmTagsIndex : ITagsIndex
    {
        /// <summary>
        /// Holds all the tags objects.
        /// </summary>
        private ObjectTable<OsmTags> _tags;

        /// <summary>
        /// Creates a new tags index with a given strings table.
        /// </summary>
        public OsmTagsIndex()
        {
            //_string_table = string_table;
            _tags = new ObjectTable<OsmTags>(true);

            this.Add(new Dictionary<string, string>());
        }

        ///// <summary>
        ///// Creates a new tags index.
        ///// </summary>
        //public OsmTagsIndex()
        //{
        //    _string_table = new ObjectTable<string>(true);
        //    _tags = new ObjectTable<OsmTags>(true);

        //    this.Add(new Dictionary<string, string>());
        //}

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
                return osm_tags.GetTags();
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
            OsmTags osm_tags = OsmTags.CreateFrom( tags);
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
            private string[,] _tags;

            /// <summary>
            /// Creates a new tags object.
            /// </summary>
            /// <param name="tags"></param>
            private OsmTags(string[,] tags)
            {
                _tags = tags;
            }

            /// <summary>
            /// Creates a new tags object.
            /// </summary>
            /// <param name="tags"></param>
            /// <returns></returns>
            internal static OsmTags CreateFrom(IDictionary<string, string> tags)
            {
                if (tags != null)
                {
                    string[,] tags_int = new string[tags.Count, 2];
                    int idx = 0;
                    foreach (KeyValuePair<string, string> tag in tags)
                    {
                        tags_int[idx, 0] = tag.Key; // string_table.Add(tag.Key);
                        tags_int[idx, 1] = tag.Value; // string_table.Add(tag.Value);
                        idx++;
                    }
                    return new OsmTags(tags_int);
                }
                return null;  // don't waste space on tags that contain no information.
            }

            /// <summary>
            /// Returns the actual tags.
            /// </summary>
            public IDictionary<string, string> GetTags()
            {
                Dictionary<string, string> tags = new Dictionary<string, string>();
                for (int idx = 0; idx < this._tags.GetLength(0); idx++)
                {
                    tags.Add(this._tags[idx, 0],
                        this._tags[idx, 1]);
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
                foreach (string value in _tags)
                {
                    hash = hash ^ value.GetHashCode();
                }
                return hash;
            }
        }
    }
}
