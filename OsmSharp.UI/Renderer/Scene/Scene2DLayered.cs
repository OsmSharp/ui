// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;
using OsmSharp.UI.Renderer.Scene.Storage.Layered;

namespace OsmSharp.UI.Renderer.Scene
{
	/// <summary>
	/// A scene 2D implementation that is layer for different zoom levels containing simplified objects.
	/// </summary>
	public class Scene2DLayered : Scene2D
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.UI.Renderer.Scene.Scene2DLayered"/> class.
		/// </summary>
        /// <param name="zoomLevelCutoffs">Zoom level cutoffs.</param>
		public Scene2DLayered (List<float> zoomLevelCutoffs)
		{
			_zoomLevelCutoffs = zoomLevelCutoffs;
			_scenes = new Scene2DSimple[_zoomLevelCutoffs.Count];
			_nonSimplifiedScene = new Scene2DSimple ();
		}

		#region Scene Management

		/// <summary>
		/// Holds the boundaries between the different zoom-seperated layers.
		/// </summary>
		private readonly List<float> _zoomLevelCutoffs;

		/// <summary>
		/// Holds the list of scenes.
		/// </summary>
		private readonly Scene2DSimple[] _scenes;

		/// <summary>
		/// Holds a scene containing objects that cannot be simplified.
		/// </summary>
		private readonly Scene2DSimple _nonSimplifiedScene;

		/// <summary>
		/// Searches for the scene appropriate for the given zoomFactor.
		/// </summary>
		/// <returns>The for scene.</returns>
		/// <param name="zoomFactor">Zoom factor.</param>
		private Scene2D SearchForScene(float zoomFactor)
		{
            if (_zoomLevelCutoffs[_zoomLevelCutoffs.Count - 1] > zoomFactor) { return null; }
			for (int idx = _zoomLevelCutoffs.Count - 2; idx >= 0 ; idx--) {
				if (_zoomLevelCutoffs [idx] > zoomFactor) {
					return _scenes [idx + 1];
				}
			}
            return _scenes[0];
		}

		/// <summary>
		/// Calculates the simplification epsilon.
		/// </summary>
		/// <returns>The simplification epsilon.</returns>
		/// <param name="zoomFactor">Zoom factor.</param>
		private float CalculateSimplificationEpsilon(float zoomFactor)
		{
            double pixelWidth = 1 / zoomFactor * 4;
            return (float)pixelWidth;
		}

		/// <summary>
		/// Calculates the simplification surface condition.
		/// </summary>
		/// <returns><c>true</c>, if simplification surface was calculated, <c>false</c> otherwise.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		private bool CalculateSimplificationSurfaceCondition(float zoomFactor, double[] x, double[] y)
		{
            BoxF2D rectangle = new BoxF2D(x, y);
            float epsilon = this.CalculateSimplificationEpsilon(zoomFactor); //(1.0f / zoomFactor);
            if (rectangle.Delta[0] < epsilon && rectangle.Delta[1] < epsilon)
            {
                return false;
            }
			return true;
		}

		#endregion

        /// <summary>
        /// Returns the number of objects in this scene.
        /// </summary>
        /// <value>The count.</value>
        public override int Count
        {
            get
            {
                int count = 0;
                foreach (Scene2D scene in _scenes)
                {
                    if (scene != null)
                    {
                        count = scene.Count + count;
                    }
                }
                return count;
            }
        }

		/// <summary>
		/// Gets all objects in this scene for the specified view.
		/// </summary>
		/// <param name="view">View.</param>
		/// <param name="zoom"></param>
        public override IEnumerable<Scene2DPrimitive> Get(View2D view, float zoom)
		{
			Scene2D scene = this.SearchForScene (zoom);
            List<IEnumerable<Scene2DPrimitive>> primitives = new List<IEnumerable<Scene2DPrimitive>>();
            primitives.Add(_nonSimplifiedScene.Get(view, zoom));
			if (scene != null) {
				primitives.Add(scene.Get (view, zoom));
			}
            return new Scene2DPrimitiveEnumerable(primitives);
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public override void Clear ()
		{
			for (int idx = 0; idx < _scenes.Length; idx++) {
				_scenes [idx] = null;
			}
		}

        /// <summary>
        /// Returns the readonly flag.
        /// </summary>
        public override bool IsReadOnly
        {
            get { return true; }
        }

		/// <summary>
		/// Returns the primitive with the given id if any.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public override List<IScene2DPrimitive> Get (uint id)
		{
            throw new NotSupportedException("This scene is readonly and does not keep object ids. Check readonly flag.");
            //List<IScene2DPrimitive> primitives = new List<IScene2DPrimitive>();
            //for (int idx = 0; idx < _scenes.Length; idx++) {
            //    if (_scenes [idx] != null) {
            //        primitives.AddRange(_scenes [idx].Get (id));
            //    }
            //}
            //return primitives;
		}

        /// <summary>
        /// Returns the primitive with the given id if any.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool Remove(uint id)
        {
            throw new NotSupportedException("This scene is readonly and does not keep object ids. Check readonly flag.");
            //bool removed = false;
            //for (int idx = 0; idx < _scenes.Length; idx++)
            //{
            //    if (_scenes[idx] != null)
            //    {
            //        removed = removed || _scenes[idx].Remove(id);
            //    }
            //}
            //return removed;
        }

		/// <summary>
		/// Adds an icon.
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="maxZoom"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="iconImage"></param>
		/// <param name="minZoom"></param>
		/// <returns></returns>
		public override uint AddIcon (int layer, float minZoom, float maxZoom, double x, double y, byte[] iconImage)
		{
			return _nonSimplifiedScene.AddIcon (layer, minZoom, maxZoom, x, y, iconImage);
		}

		/// <summary>
		/// Adds an image.
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="minZoom"></param>
		/// <param name="maxZoom"></param>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		/// <param name="imageData"></param>
		/// <returns></returns>
		public override uint AddImage (int layer, float minZoom, float maxZoom, double left, double top, double right, double bottom, byte[] imageData)
		{
			return _nonSimplifiedScene.AddImage (layer, minZoom, maxZoom, left, top, right, bottom, imageData);
		}

		/// <summary>
		/// Adds the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="layer">Layer.</param>
		/// <param name="minZoom">Minimum zoom.</param>
		/// <param name="maxZoom">Max zoom.</param>
		/// <param name="left">Left.</param>
		/// <param name="top">Top.</param>
		/// <param name="right">Right.</param>
		/// <param name="bottom">Bottom.</param>
		/// <param name="imageData">Image data.</param>
		/// <param name="tag">Tag.</param>
		public override uint AddImage (int layer, float minZoom, float maxZoom, double left, double top, double right, double bottom, byte[] imageData, object tag)
		{
			return _nonSimplifiedScene.AddImage (layer, minZoom, maxZoom, left, top, right, bottom, imageData, tag);
		}

		/// <summary>
		/// Adds the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="layer">Layer.</param>
		/// <param name="minZoom">Minimum zoom.</param>
		/// <param name="maxZoom">Max zoom.</param>
		/// <param name="rectangle">Rectangle.</param>
		/// <param name="imageData">Image data.</param>
		/// <param name="tag">Tag.</param>
		public override uint AddImage (int layer, float minZoom, float maxZoom, RectangleF2D rectangle, byte[] imageData, object tag)
		{
			return _nonSimplifiedScene.AddImage (layer, minZoom, maxZoom, rectangle, imageData, tag);
		}

		/// <summary>
		/// Adds a point.
		/// </summary>
		/// <param name="maxZoom"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		/// <param name="minZoom"></param>
		/// <returns>The point.</returns>
		public override uint AddPoint (float minZoom, float maxZoom, double x, double y, int color, double size)
		{
			return _nonSimplifiedScene.AddPoint(minZoom, maxZoom, x, y, color, size);
		}

		/// <summary>
		/// Adds a point.
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="maxZoom"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		/// <param name="minZoom"></param>
		/// <returns>The point.</returns>
		public override uint AddPoint (int layer, float minZoom, float maxZoom, double x, double y, int color, double size)
		{
			return _nonSimplifiedScene.AddPoint(layer, minZoom, maxZoom, x, y, color, size);
		}

		/// <summary>
		/// Adds texts.
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="maxZoom"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="size"></param>
		/// <param name="text"></param>
		/// <param name="minZoom"></param>
		/// <returns></returns>
		/// <param name="color">Color.</param>
		/// <param name="haloColor">Halo color.</param>
		/// <param name="haloRadius">Halo radius.</param>
		public override uint AddText (int layer, float minZoom, float maxZoom, double x, double y, double size, string text, int color, 
		                              int? haloColor, int? haloRadius, string font)
		{
			return _nonSimplifiedScene.AddText (layer, minZoom, maxZoom, x, y, size, text, color, haloColor, haloRadius, font);
		}

		/// <summary>
		/// Adds a line.
		/// </summary>
		/// <param name="maxZoom"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		/// <param name="minZoom"></param>
		/// <returns>The line.</returns>
		public override uint AddLine (int layer, float minZoom, float maxZoom, double[] x, double[] y,
            int color, double width, LineJoin lineJoin, int[] dashes)
		{ // add the line but simplify it for higher zoom levels.
			float currentMaxZoom = float.MaxValue;
            for (int idx = 0; idx < _zoomLevelCutoffs.Count; idx++)
            {
                float currentMinZoom = _zoomLevelCutoffs[idx];
                if (!(currentMinZoom >= maxZoom) && !(currentMaxZoom < minZoom))
                {
                    float thisMinZoom = System.Math.Max(currentMinZoom, minZoom);
                    float thisMaxZoom = System.Math.Min(currentMaxZoom, maxZoom);

                    // simplify the algorithm.
                    double epsilon = this.CalculateSimplificationEpsilon(thisMaxZoom);
                    double[][] simplified = OsmSharp.Math.Algorithms.SimplifyCurve.Simplify(new double[][] { x, y },
                                                                    epsilon);
                    double distance = epsilon * 2;
                    if (simplified[0].Length == 2)
                    { // check if the simplified version is smaller than epsilon.
                        OsmSharp.Math.Primitives.PointF2D point1 = new OsmSharp.Math.Primitives.PointF2D(
                            simplified[0][0], simplified[0][1]);
                        OsmSharp.Math.Primitives.PointF2D point2 = new OsmSharp.Math.Primitives.PointF2D(
                            simplified[1][0], simplified[0][1]);
                        distance = point1.Distance(point2);
                    }
                    if (distance >= epsilon)
                    {
                        // add to the scene.
                        if (_scenes[idx] == null)
                        {
                            _scenes[idx] = new Scene2DSimple();
                        }
                        _scenes[idx].AddLine(layer, thisMinZoom, thisMaxZoom, simplified[0], simplified[1],
                            color, width, lineJoin, dashes);
                    }
                }
                currentMaxZoom = currentMinZoom; // move to the next cutoff.
            }
			return 0;
		}

		/// <summary>
		/// Adds the polygon.
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="maxZoom"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		/// <returns>The polygon.</returns>
		/// <param name="minZoom">Minimum zoom.</param>
		/// <param name="fill">If set to <c>true</c> fill.</param>
		public override uint AddPolygon (int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, double width, 
		                                 bool fill)
		{ // add the polygon but simplify it for higher zoom levels.
			float currentMaxZoom = float.MaxValue;
			for (int idx = 0; idx < _zoomLevelCutoffs.Count; idx++) {
				float currentMinZoom = _zoomLevelCutoffs [idx];
				if (!(currentMinZoom >= maxZoom) && !(currentMaxZoom < minZoom)) {
					float thisMinZoom = System.Math.Max (currentMinZoom, minZoom);
					float thisMaxZoom = System.Math.Min (currentMaxZoom, maxZoom);

					// simplify the algorithm.
					double[][] simplified = OsmSharp.Math.Algorithms.SimplifyCurve.Simplify (new double[][] { x, y },
					this.CalculateSimplificationEpsilon(thisMaxZoom));

					if (simplified[0].Length > 2 &&
					    this.CalculateSimplificationSurfaceCondition(currentMaxZoom, x, y))
                    {
                        // add to the scene.
                        if (_scenes[idx] == null)
                        {
                            _scenes[idx] = new Scene2DSimple();
                        }
                        _scenes[idx].AddPolygon(thisMinZoom, thisMaxZoom, simplified[0], simplified[1], color, width, fill);
                    }
				}
				currentMaxZoom = currentMinZoom; // move to the next cutoff.
			}
			return 0;
		}

		/// <summary>
		/// Adds a text along a line to this scene.
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="minZoom"></param>
		/// <param name="maxZoom"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
        /// <param name="fontSize"></param>
		/// <param name="text"></param>
		/// <returns>The text line.</returns>
		/// <param name="haloColor">Halo color.</param>
		/// <param name="haloRadius">Halo radius.</param>
		public override uint AddTextLine (int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, 
		                                  float fontSize, string text, int? haloColor, int? haloRadius)
		{ // add the textline but simplify it for higher zoom levels.
			float currentMaxZoom = float.MaxValue;
			for (int idx = 0; idx < _zoomLevelCutoffs.Count; idx++) {
				float currentMinZoom = _zoomLevelCutoffs [idx];
                if (!(currentMinZoom > maxZoom) && !(currentMaxZoom < minZoom))
                {
                    float thisMinZoom = System.Math.Max(currentMinZoom, minZoom);
                    float thisMaxZoom = System.Math.Min(currentMaxZoom, maxZoom);

                    // simplify the algorithm.
                    double[][] simplified = OsmSharp.Math.Algorithms.SimplifyCurve.Simplify(new double[][] { x, y },
                        this.CalculateSimplificationEpsilon(thisMaxZoom));
                    //if (this.CalculateSimplificationSurfaceCondition (currentMaxZoom, x, y)) {
                    // add to the scene.
                    if (_scenes[idx] == null)
                    {
                        _scenes[idx] = new Scene2DSimple();
                    }
                    _scenes[idx].AddTextLine(layer, thisMinZoom, thisMaxZoom, simplified[0], simplified[1], color, fontSize, text,
                                              haloColor, haloRadius);
                    //}
                }
				currentMaxZoom = currentMinZoom; // move to the next cutoff.
			}
			return 0;
		}

        #region Serialization/Deserialization

        /// <summary>
        /// Serializes this scene2D to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="compress"></param>
        public override void Serialize(Stream stream, bool compress)
        {
            Scene2DLayeredSerializer serializer = new Scene2DLayeredSerializer();
            serializer.Serialize(stream, _nonSimplifiedScene, _scenes, _zoomLevelCutoffs, compress);
        }

        /// <summary>
        /// Deserialize a Scene2D from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static IScene2DPrimitivesSource Deserialize(Stream stream, bool compressed)
        {
            Scene2DLayeredSerializer serializer = new Scene2DLayeredSerializer();
            return serializer.DeSerialize(stream);
        }

        #endregion
	}
}