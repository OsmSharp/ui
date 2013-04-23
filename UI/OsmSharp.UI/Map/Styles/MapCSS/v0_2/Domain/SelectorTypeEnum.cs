using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain
{
    /// <summary>
    /// En enumeration of all MapCSS 0.2 selector types.
    /// </summary>
    public enum SelectorTypeEnum
    {
        /// <summary>
        /// node: A node.
        /// </summary>
        Node,

        /// <summary>
        /// way: A way.
        /// </summary>
        Way,

        /// <summary>
        /// relation: A relation.
        /// </summary>
        Relation,

        /// <summary>
        /// area: A way where the start and finish nodes are the same node, or area=yes has been set.
        /// </summary>
        Area,

        /// <summary>
        /// line: A way where the start and finish nodes are not the same node, or area=no has been set.
        /// </summary>
        Line,

        /// <summary>
        /// canvas: The background of the render.
        /// </summary>
        Canvas,

        /// <summary>
        /// *: Any.
        /// </summary>
        Star
    }

    /// <summary>
    /// Class containing selector type.
    /// </summary>
    public static class SelectorTypeEnumExtensions
    {
        /// <summary>
        /// Returns a MapCSS string.
        /// </summary>
        /// <param name="selectorTypeEnum"></param>
        /// <returns></returns>
        public static string ToMapCSSString(this SelectorTypeEnum selectorTypeEnum)
        {
            switch (selectorTypeEnum)
            {
                case SelectorTypeEnum.Way:
                    return "way";
                case SelectorTypeEnum.Node:
                    return "node";
                case SelectorTypeEnum.Line:
                    return "line";
                case SelectorTypeEnum.Canvas:
                    return "canvas";
                case SelectorTypeEnum.Relation:
                    return "relation";
                case SelectorTypeEnum.Area:
                    return "area";
                case SelectorTypeEnum.Star:
                    return "*";
            }
            return "UNKNOWN SELECTOR";
        }
    }
}