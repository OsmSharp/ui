using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OsmSharp.Tools.Collections.Tags
{
    /// <summary>
    /// Represents a generic tags collection.
    /// </summary>
    public abstract class TagsCollection : IEnumerable<Tag>
    {
        /// <summary>
        /// Returns the number of tags in this collection.
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Adds a key-value pair to this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void Add(string key, string value);

        /// <summary>
        /// Adds a tag.
        /// </summary>
        /// <param name="tag"></param>
        public abstract void Add(Tag tag);

        /// <summary>
        /// Adds a tag or replace the existing value if any.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void AddOrReplace(string key, string value);

        /// <summary>
        /// Adds a tag or replace the existing value if any.
        /// </summary>
        /// <param name="tag"></param>
        public abstract void AddOrReplace(Tag tag);

        /// <summary>
        /// Returns true if the given tag exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool ContainsKey(string key);

        /// <summary>
        /// Returns true if the given tag exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool TryGetValue(string key, out string value);

        /// <summary>
        /// Returns true if the given tags exists with the given value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool ContainsKeyValue(string key, string value);

        /// <summary>
        /// Returns the value associated with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Throws a KeyNotFoundException when the key does not exists. Use TryGetValue.</returns>
        public virtual string this[string key]
        {
            get
            {
                string value;
                if (this.TryGetValue(key, out value))
                {
                    return value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                this.AddOrReplace(key, value);
            }
        }

        /// <summary>
        /// Returns a parsed numeric value if available.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double? GetNumericValue(string key)
        {
            string value;
            if (this.TryGetValue(key, out value))
            {
                double numericValue;
                if (double.TryParse(value, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture,
                                    out numericValue))
                {
                    return numericValue;
                }
            }
            return null;
        }

        /// <summary>
        /// Clears all tags.
        /// </summary>
        public abstract void Clear();

        #region IEnumerable<Tag>

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<Tag> GetEnumerator();

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
