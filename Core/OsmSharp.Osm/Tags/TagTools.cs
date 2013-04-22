using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp
{
    /// <summary>
    /// Contains common tag tools.
    /// </summary>
    public static class TagsTools
    {
        /// <summary>
        /// Returns true if the key-value pair is contained in the given tags collection.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsKeyValue(this IDictionary<string, string> tags, string key, string value)
        {
            if (tags != null)
            { // the tags are there!
                string keyValue;
                return tags.TryGetValue(key, out keyValue) &&
                       keyValue == value;
            }
            return false;
        }
    }
}
