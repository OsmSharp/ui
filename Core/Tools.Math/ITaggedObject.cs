using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math
{
    /// <summary>
    /// Represents an object that can be tagged with key-value pairs of strings.
    /// </summary>
    public interface ITaggedObject
    {
        /// <summary>
        /// The tags list.
        /// </summary>
        List<KeyValuePair<string, string>> Tags { get; }
    }
}
