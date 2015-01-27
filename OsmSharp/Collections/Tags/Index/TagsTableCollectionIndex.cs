// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

namespace OsmSharp.Collections.Tags.Index
{
    /// <summary>
    /// An osm tags index.
    /// </summary>
    public class TagsTableCollectionIndex : ITagsCollectionIndex
    {
        /// <summary>
        /// Holds all the tags objects.
        /// </summary>
        private readonly ObjectTable<OsmTags> _tagsCollectionTable;

        /// <summary>
        /// Holds all the tags objects.
        /// </summary>
        private readonly ObjectTable<Tag> _tagsTable;

        /// <summary>
        /// Creates a new tags index with a given strings table.
        /// </summary>
        public TagsTableCollectionIndex()
        {
            _tagsTable = new ObjectTable<Tag>(true);
            _tagsCollectionTable = new ObjectTable<OsmTags>(true);

            this.Add(new TagsCollection());
        }

        /// <summary>
        /// Creates a new tags index with a given strings table.
        /// </summary>
        /// <param name="checkDuplicates">Flag to prevent this index from checking for duplicates, used when sure all tag collections are unique.</param>
        public TagsTableCollectionIndex(bool checkDuplicates)
        {
            _tagsTable = new ObjectTable<Tag>(true);
            _tagsCollectionTable = new ObjectTable<OsmTags>(false, ObjectTable<Tag>.INITIAL_CAPACITY, !checkDuplicates);

            this.Add(new TagsCollection());
        }

        /// <summary>
        /// Creates a new tags index with a given strings table.
        /// </summary>
        /// <param name="reverseIndex">The reverse index is enable if true.</param>
        /// <param name="allowDuplicates">Flag preventing this object table for checking for duplicates. Use this when sure almost all objects will be unique.</param>
        public TagsTableCollectionIndex(bool reverseIndex, bool checkDuplicates)
            : this(reverseIndex, ObjectTable<OsmTags>.INITIAL_CAPACITY, checkDuplicates)
        {
            
        }

        /// <summary>
        /// Creates a new tags index with a given strings table.
        /// </summary>
        /// <param name="reverseIndex">The reverse index is enable if true.</param>
        /// <param name="initCapacity">The inital capacity.</param>
        /// <param name="allowDuplicates">Flag preventing this object table for checking for duplicates. Use this when sure almost all objects will be unique.</param>
        public TagsTableCollectionIndex(bool reverseIndex, int initCapacity, bool checkDuplicates)
        {
            _tagsTable = new ObjectTable<Tag>(true);
            _tagsCollectionTable = new ObjectTable<OsmTags>(reverseIndex, initCapacity, !checkDuplicates);

            this.Add(new TagsCollection());
        }

        /// <summary>
        /// Creates a new tags index with a given strings table.
        /// </summary>
        /// <param name="tagsTable"></param>
        public TagsTableCollectionIndex(ObjectTable<Tag> tagsTable)
        {
            _tagsTable = tagsTable;
            _tagsCollectionTable = new ObjectTable<OsmTags>(true);

            this.Add(new TagsCollection());
        }

        /// <summary>
        /// Drops reverse indexes only need when adding tags.
        /// </summary>
        public void DropReverseIndexex()
        {
            _tagsCollectionTable.DropReverseIndex();
            _tagsTable.DropReverseIndex();
        }

        /// <summary>
        /// Builds the reverse indexes (again) for adding tags.
        /// </summary>
        public void BuildReverseIndex()
        {
            _tagsCollectionTable.BuildReverseIndex();
            _tagsTable.BuildReverseIndex();
        }

        /// <summary>
        /// Returns the tags with the given id.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public TagsCollectionBase Get(uint tagsId)
        {
            OsmTags osmTags = _tagsCollectionTable.Get(tagsId);
            if (osmTags != null)
            {
                TagsCollection collection = new TagsCollection();
                for (int idx = 0; idx < osmTags.Tags.Length; idx++)
                {
                    collection.Add(
                        _tagsTable.Get(osmTags.Tags[idx]));
                }
                return collection;
            }
            return null;
        }

        /// <summary>
        /// Returns true if the tags with the given id are in this collection.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public bool Contains(uint tagsId)
        {
            return _tagsCollectionTable.Contains(tagsId);
        }

        /// <summary>
        /// Adds tags to this index.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public uint Add(TagsCollectionBase tags)
        {
            int idx = 0;
            uint[] tagsArray = new uint[tags.Count];
            foreach (Tag tag in tags)
            {
                tagsArray[idx] = _tagsTable.Add(tag);
                idx++;
            }
            var osmTags = new OsmTags(tagsArray);
            if (osmTags != null)
            {
                return _tagsCollectionTable.Add(osmTags);
            }
            throw new ArgumentNullException("tags", "Tags dictionary cannot be null or empty!");
        }

        /// <summary>
        /// Adds tags to this index without check if they exists already.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public uint AddObject(TagsCollectionBase tags)
        {
            int idx = 0;
            uint[] tagsArray = new uint[tags.Count];
            foreach (Tag tag in tags)
            {
                tagsArray[idx] = _tagsTable.Add(tag);
                idx++;
            }
            var osmTags = new OsmTags(tagsArray);
            if (osmTags != null)
            {
                return _tagsCollectionTable.AddObject(osmTags);
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
            private readonly uint[] _tags;

            /// <summary>
            /// Creates a new tags object.
            /// </summary>
            /// <param name="tags"></param>
            public OsmTags(uint[] tags)
            {
                _tags = tags;
            }

            /// <summary>
            /// Returns the tags array.
            /// </summary>
            public uint[] Tags
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
                                if(!other._tags.Contains(this._tags[idx1]))
                                {
                                    return false;
                                }
                            }
                            // make sure all object in the first are in the second and vice-versa.
                            for (int idx1 = 0; idx1 < other._tags.GetLength(0); idx1++)
                            {
                                if (!this._tags.Contains(other._tags[idx1]))
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
                    (current, tag) => current ^ tag.GetHashCode());
            }

            #endregion
        }

        /// <summary>
        /// Returns maximum possible number of tags in this index.
        /// </summary>
        public uint Max
        {
            get { return _tagsCollectionTable.Count; }
        }
    }
}