using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Tools.Math
{
    /// <summary>
    /// Abstracts an index containing tags.
    /// </summary>
    public interface ITagsIndex
    {
        /// <summary>
        /// Returns the tags that belong to the given id.
        /// </summary>
        /// <param name="tags_int"></param>
        /// <returns></returns>
        IDictionary<string, string> Get(uint tags_int);

        /// <summary>
        /// Adds new tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        uint Add(IDictionary<string, string> tags);
    }

    /// <summary>
    /// Common extension functions for tags.
    /// </summary>
    public static class TagsExtensions
    {
        /// <summary>
        /// Converts a list of keyvalue pairs to a dictionary.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ConvertToDictionary(this IList<KeyValuePair<string, string>> list)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in list)
            {
                dictionary[pair.Key] = pair.Value;
            }
            return dictionary;
        }
    }
}
