using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2
{
    /// <summary>
    /// Enumeration of MapCSS types.
    /// </summary>
    public enum MapCSSTypes
    {
        /// <summary>
        /// A node.
        /// </summary>
        Node,

        /// <summary>
        /// A way.
        /// </summary>
        Way,

        /// <summary>
        /// A relation.
        /// </summary>
        Relation,

        /// <summary>
        /// A line (A way where the start and finish nodes are not the same node, or area=no has been set).
        /// </summary>
        Line,

        /// <summary>
        /// An area (A way where the start and finish nodes are the same node, or area=yes has been set).
        /// </summary>
        Area
    }

    /// <summary>
    /// Contains MapCSS extensions.
    /// </summary>
    public static class MapCSSTypesExtensions
    {
        /// <summary>
        /// Returns true if the given object is of the given type.
        /// </summary>
        /// <param name="osmGeo"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static bool IsOfType(this CompleteOsmGeo osmGeo, MapCSSTypes types)
        {
            string area = string.Empty;
            switch (types)
            {
                case MapCSSTypes.Node:
                    return osmGeo.Type == CompleteOsmType.Node;
                case MapCSSTypes.Way:
                    return osmGeo.Type == CompleteOsmType.Way;
                case MapCSSTypes.Relation:
                    return osmGeo.Type == CompleteOsmType.Relation;
                case MapCSSTypes.Line:
                    if (osmGeo.Type == CompleteOsmType.Way)
                    { // the type is way way. now check for a line.
                        var way = (osmGeo as CompleteWay);
                        if (way != null &&
                            way.Nodes[0] == way.Nodes[way.Nodes.Count - 1])
                        { // first node is the same as the last one.
                            if (way.Tags != null &&
                                way.Tags.TryGetValue("area", out area) &&
                                area == "yes")
                            { // oeps, an area.
                                return false;
                            }
                            return true;
                        }
                        else
                        { // first node is different from the last one.
                            return true; // even if there is an area=yes tag this cannot be an area.
                        }
                    }
					break;
                case MapCSSTypes.Area:
                    if (osmGeo.Type == CompleteOsmType.Way)
                    { // the type is way way. now check for a line.
                        var way = (osmGeo as CompleteWay);
                        if (way != null &&
                            way.Nodes != null &&
                            (way.Nodes.Count > 2 && (way.Nodes[0] == way.Nodes[way.Nodes.Count - 1])))
                        { // first node is the same as the last one.
                            return true;
                        }
                    }
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("types");
            }
            return false;
        }
    }
}