using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.Collections.SpatialIndexes.Serialization;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives.Storage
{
    /// <summary>
    /// A source of scene primitives.
    /// </summary>
    internal class Scene2DPrimitivesSource : IScene2DPrimitivesSource
    {
        /// <summary>
        /// Holds the serializer.
        /// </summary>
        private readonly ISpatialIndexReadonly<Scene2DEntry> _index;

        /// <summary>
        /// Creates a new scene 2D primitives source.
        /// </summary>
        /// <param name="index"></param>
        public Scene2DPrimitivesSource(ISpatialIndexReadonly<Scene2DEntry> index)
        {
            _index = index;
        }

        /// <summary>
        /// Adds all primitives inside the given box for the given zoom.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="view"></param>
        /// <param name="zoomFactor"></param>
        public void Get(Scene2D scene, View2D view, float zoomFactor)
        {
            // get all primitives.
            IEnumerable<Scene2DEntry> entries = 
                _index.Get(new RectangleF2D(view.Left, view.Top, view.Right, view.Bottom));

            foreach (var scene2DEntry in entries)
            {
                scene.AddPrimitive(scene2DEntry.Layer, scene2DEntry.Id, scene2DEntry.Scene2DPrimitive);
            }
        }
    }
}
