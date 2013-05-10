using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Collections.Tags
{
    /// <summary>
    /// Represents a tag (a key-value pair).
    /// </summary>
    public struct Tag
    {
        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Tag(string key, string value)
            :this()
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// The key (or the actual tag name).
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value of the tag.
        /// </summary>
        public string Value { get; set; }
    }
}
