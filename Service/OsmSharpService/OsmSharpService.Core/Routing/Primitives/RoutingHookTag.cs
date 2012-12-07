using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharpService.Core.Routing.Primitives
{
    /// <summary>
    /// Represents a string pair, key-value, as a pair of tags for a routing hook.
    /// </summary>
    public class RoutingHookTag
    {
        /// <summary>
        /// The key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Containts extension methods for the RoutingHook-class.
    /// </summary>
    public static class RoutingHookExtensions
    {
        /// <summary>
        /// Converts the tags in the given dictionary to RoutingHookTags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static RoutingHookTag[] ConvertToRoutingHookTags(this IDictionary<string, string> tags)
        {
            RoutingHookTag[] hook_tags = new RoutingHookTag[tags.Count];
            int idx = 0;
            foreach (KeyValuePair<string, string> tag in tags)
            { // loop over all the tags.
                hook_tags[idx] = new RoutingHookTag()
                {
                    Key = tag.Key,
                    Value = tag.Value
                };

                idx++;
            }
            return hook_tags;
        }

        /// <summary>
        /// Converts the RoutingHookTags to a regular tags dictionary.
        /// </summary>
        /// <param name="hook_tags"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ConvertToDictionary(this RoutingHookTag[] hook_tags)
        {
            IDictionary<string, string> tags = new Dictionary<string, string>();
            if (hook_tags != null)
            {
                for (int idx = 0; idx < hook_tags.Length; idx++)
                {
                    tags[hook_tags[idx].Key] = hook_tags[idx].Value;
                }
            }
            return tags;
        }

        /// <summary>
        /// Converts the RoutingHookTags to a regular tags dictionary.
        /// </summary>
        /// <param name="hook_tags"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> ConvertToList(this RoutingHookTag[] hook_tags)
        {
            List<KeyValuePair<string, string>> tags = new List<KeyValuePair<string, string>>();
            if (hook_tags != null)
            {
                for (int idx = 0; idx < hook_tags.Length; idx++)
                {
                    tags.Add(new KeyValuePair<string, string>(hook_tags[idx].Key, hook_tags[idx].Value));
                }
            }
            return tags;
        }
    }
}