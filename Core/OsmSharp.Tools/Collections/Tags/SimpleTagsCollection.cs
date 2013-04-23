using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Tools.Collections.Tags
{
    /// <summary>
    /// Represents a simple tags collection based on a list.
    /// </summary>
    public class SimpleTagsCollection : TagsCollection
    {
        /// <summary>
        /// Holds the tags.
        /// </summary>
        private readonly List<Tag> _tags;

        /// <summary>
        /// Creates a new tags collection.
        /// </summary>
        public SimpleTagsCollection()
        {
            _tags = new List<Tag>();
        }

        /// <summary>
        /// Creates a new tags collection initialized with the given existing tags.
        /// </summary>
        /// <param name="tags"></param>
        public SimpleTagsCollection(IEnumerable<Tag> tags)
        {
            _tags = new List<Tag>();
            _tags.AddRange(tags);
        }

        /// <summary>
        /// Returns the number of tags in this collection.
        /// </summary>
        public override int Count
        {
            get { return _tags.Count; }
        }

        /// <summary>
        /// Adds a new tag (key-value pair) to this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void Add(string key, string value)
        {
            _tags.Add(new Tag()
            {
                Key = key,
                Value = value
            });
        }

        /// <summary>
        /// Adds a new tag to this collection.
        /// </summary>
        /// <param name="tag"></param>
        public override void Add(Tag tag)
        {
            _tags.Add(tag);
        }

        /// <summary>
        /// Adds a new tag (key-value pair) to this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void AddOrReplace(string key, string value)
        {
            for(int idx = 0; idx < _tags.Count; idx++)
            {
                Tag tag = _tags[idx];
                if (tag.Key == key)
                {
                    tag.Value = value;
                    _tags[idx] = tag;
                    return;
                }
            }
            this.Add(key, value);
        }

        /// <summary>
        /// Adds a new tag to this collection.
        /// </summary>
        /// <param name="tag"></param>
        public override void AddOrReplace(Tag tag)
        {
            this.AddOrReplace(tag.Key, tag.Value);
        }

        /// <summary>
        /// Returns true if the given key is found in this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool ContainsKey(string key)
        {
            return this.Any(tag => tag.Key == key);
        }

        /// <summary>
        /// Returns true if the given key exists and sets the value parameter.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetValue(string key, out string value)
        {
            foreach (var tag in this.Where(tag => tag.Key == key))
            {
                value = tag.Value;
                return true;
            }
            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Returns true if the given key-value pair is found in this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool ContainsKeyValue(string key, string value)
        {
            return this.Any(tag => tag.Key == key && tag.Value == value);
        }

        /// <summary>
        /// Clears all tags.
        /// </summary>
        public override void Clear()
        {
            _tags.Clear();
        }

        /// <summary>
        /// Returns the enumerator for this tags collection.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<Tag> GetEnumerator()
        {
            return _tags.GetEnumerator();
        }
    }
}