using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Core.Constraints
{
    /// <summary>
    /// Represents a routing label used to implement routing constraints.
    /// </summary>
    public class RoutingLabel
    {
        /// <summary>
        /// Creates a new routing label.
        /// </summary>
        /// <param name="label"></param>
        internal RoutingLabel(char label, string description)
        {
            this.Label = label;
            this.Description = description;
        }

        /// <summary>
        /// The actual label.
        /// </summary>
        public char Label
        {
            get;
            private set;
        }


        /// <summary>
        /// The description of the label.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns true if equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
            {
                return false;
            }
            else if (obj is RoutingLabel)
            {
                return (obj as RoutingLabel).Label == this.Label;
            }
            return false;
        }
    }

    /// <summary>
    /// Contains extensions.
    /// </summary>
    public static class RoutingLabelExtensions
    {
        /// <summary>
        /// Constructs a string out of a list of labels.
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public static string CreateString(this IList<RoutingLabel> labels)
        {
            char[] label_array = new char[labels.Count];
            for (int idx = 0; idx < labels.Count; idx++)
            {
                label_array[idx] = labels[idx].Label;
            }
            return new string(label_array);
        }

        /// <summary>
        /// Constructs a string out of a list of labels and one extra label.
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public static string CreateString(this IList<RoutingLabel> labels, RoutingLabel extra)
        {
            StringBuilder builder = new StringBuilder(labels.CreateString());
            builder.Append(extra.Label);
            return builder.ToString();
        }
    }
}
