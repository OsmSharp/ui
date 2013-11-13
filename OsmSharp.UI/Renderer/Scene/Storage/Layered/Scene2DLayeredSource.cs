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
using System.Linq;
using System.Text;
using System.IO;
using OsmSharp.IO;

namespace OsmSharp.UI.Renderer.Scene.Storage.Layered
{
    /// <summary>
    /// A source of scene primitives.
    /// </summary>
    public class Scene2DLayeredSource : IScene2DPrimitivesSource
    {
        /// <summary>
        /// Holds the data stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Holds the layered index.
        /// </summary>
        private Scene2DLayeredSerializer.Scene2DLayeredIndex _index;

        /// <summary>
        /// Creates a new scene 2D layered source.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="index"></param>
        public Scene2DLayeredSource(Stream stream, Scene2DLayeredSerializer.Scene2DLayeredIndex index)
        {
            _stream = stream;
            _index = index;

            _loadedScenes = new Dictionary<int, IScene2DPrimitivesSource>();
        }

        /// <summary>
        /// Searches for the scene appropriate for the given zoomFactor.
        /// </summary>
        /// <returns>The for scene.</returns>
        /// <param name="zoomFactor">Zoom factor.</param>
        private int SearchForScene(float zoomFactor)
        {
            if (_index.SceneCutoffs[_index.SceneCutoffs.Length - 1] > zoomFactor) { return -1; }
            for (int idx = _index.SceneCutoffs.Length - 2; idx >= 0; idx--)
            {
                if (_index.SceneCutoffs[idx] > zoomFactor)
                {
                    return idx + 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Holds the loaded scenes.
        /// </summary>
        private Dictionary<int, IScene2DPrimitivesSource> _loadedScenes;

        /// <summary>
        /// Holds the non-simplified scene.
        /// </summary>
        private IScene2DPrimitivesSource _nonSimplifiedScene;

        /// <summary>
        /// Returns the non-simplified scene.
        /// </summary>
        /// <returns></returns>
        private IScene2DPrimitivesSource GetNonSimplifiedStream()
        {
            if (_nonSimplifiedScene == null)
            {
                long position = _index.SceneIndexes[_index.SceneIndexes.Length - 1];
                _stream.Seek(position, SeekOrigin.Begin);
                LimitedStream stream = new LimitedStream(_stream);
                //Scene2DRTreeSerializer serializer = new Scene2DRTreeSerializer(true);
                //_nonSimplifiedScene = new Scene2DPrimitivesSource(serializer.Deserialize(stream));
                OsmSharp.UI.Renderer.Scene.Storage.Styled.Scene2DStyledSerializer serializer =
                    new Styled.Scene2DStyledSerializer(true);
                _nonSimplifiedScene = serializer.Deserialize(stream, true);
            }
            return _nonSimplifiedScene;
        }

        /// <summary>
        /// Fills the given simple scene with objects inside the given view and for the given zoomFactor.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="view"></param>
        /// <param name="zoomFactor"></param>
        public void Get(Scene2DSimple scene, View2D view, float zoomFactor)
        {
			lock (_stream) { // make sure there is only synchonous access to the stream.
				// check what is in the non-simplified scene.
				IScene2DPrimitivesSource simpleSource = this.GetNonSimplifiedStream ();
				simpleSource.Get (scene, view, zoomFactor);

				// check what index this zoomfactor is for.
				int idx = this.SearchForScene (zoomFactor);
				if (idx >= 0) { // the index was found.
					if (!_loadedScenes.TryGetValue (idx, out simpleSource)) { // the scene was not found.
						long position = _index.SceneIndexes [idx];
						_stream.Seek (position, SeekOrigin.Begin);
						LimitedStream stream = new LimitedStream (_stream);
						//Scene2DRTreeSerializer serializer = new Scene2DRTreeSerializer(true);
						//simpleSource = new Scene2DPrimitivesSource(serializer.Deserialize(stream));
						OsmSharp.UI.Renderer.Scene.Storage.Styled.Scene2DStyledSerializer serializer =
							new Styled.Scene2DStyledSerializer (true);
						simpleSource = serializer.Deserialize (stream, true);
						_loadedScenes.Add (idx, simpleSource);
					}
					simpleSource.Get (scene, view, zoomFactor);

					OsmSharp.Logging.Log.TraceEvent ("Scene2DLayeredSource", System.Diagnostics.TraceEventType.Verbose,
						string.Format ("Deserialized from scene at zoom {0} and idx {1} synchronized.", zoomFactor, idx));
				}
			}
        }

		/// <summary>
		/// Clears all information in this source.
		/// </summary>
		public void Clear() {
			if (_loadedScenes != null) {
				_loadedScenes.Clear ();
            }
        }

        /// <summary>
        /// Disposes all resources associated with this source.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
