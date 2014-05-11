using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Renderer.Scene.Simplification
{
    class Scene2DStylePolygon : Scene2DStyle
    {
        /// <summary>
        /// Gets or sets the style polygon id.
        /// </summary>
        public uint StylePolygonId { get; set; }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = (obj as Scene2DStylePolygon);
            if (other != null)
            {
                return other.StylePolygonId == this.StylePolygonId;
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return "Scene2DStylePolygon".GetHashCode() ^
                this.StylePolygonId.GetHashCode();
        }
    }
}
