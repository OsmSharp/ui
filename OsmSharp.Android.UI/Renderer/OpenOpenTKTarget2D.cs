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

using OpenTK.Graphics.ES11;
using OsmSharp.UI;
using System.Collections.Generic;

namespace OsmSharp.Android.UI
{
    class OpenTKTarget2D
    {
        /// <summary>
        /// Holds the triangles list.
        /// </summary>
        private List<PolygonProcessed> _polygons;

        /// <summary>
        /// Holds the lines list.
        /// </summary>
        private List<LineProcessed> _lines;

        public OpenTKTarget2D()
        {
            _polygons = new List<PolygonProcessed>();
            _lines = new List<LineProcessed>();
        }

        public void Clear()
        {
            lock (_polygons)
            {
                _polygons.Clear();
            }
            lock (_lines)
            {
                _lines.Clear();
            }
        }

        /// <summary>
        /// Adds the line.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="argb">ARGB.</param>
        public void AddLine(float[] points, float width, int argb)
        {
            lock (_lines)
            {
                LineProcessed line = new LineProcessed()
                {
                    Vertices = points,
                    Color = argb,
                    Width = width,
                    Count = points.Length / 3
                };
                _lines.Add(line);
            }
        }

        /// <summary>
        /// Adds a basic series of triangles to render.
        /// </summary>
        /// <param name="corners">Triangle data.</param>
        /// <param name="argb">The ARGB colors.</param>
        public void AddPolygon(float[] corners, int argb)
        {
            lock (_polygons)
            {
                _polygons.Add(new PolygonProcessed()
                {
                    Vertices = corners,
                    Color = argb,
                    Count = corners.Length / 3
                });
            }
        }

        /// <summary>
        /// Gets the orginal width in pixels.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets the orignal height in pixels.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Holds the orthoprojection bounds.
        /// </summary>
        private float _left = -1, _right = 1, _top = 1, _bottom = -1;

        #region IRenderer implementation

        /// <summary>
        /// Raises the draw frame event.
        /// </summary>
        public void OnDrawFrame()
        {
            // Replace the current matrix with the identity matrix
            GL.MatrixMode(All.ProjectionMatrix);
            GL.LoadIdentity(); // OpenGL docs
            GL.Ortho(_left, _right, _bottom, _top, -1, 1);

            // clear.
            GL.ClearColor(1, 1, 1, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //lock (_polygons)
            //{
            //    for (int idx = 0; idx < _polygons.Count; idx++)
            //    {
            //        // GL.VertexPointer(3, All.Float 0, _polygons[idx].Vertices);
            //        GL.EnableClientState(All.VertexArray);

            //        var color = SimpleColor.FromArgb(_polygons[idx].Color);
            //        GL.Color4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            //        // GL.DrawArrays(PrimitiveType.Polygon, 0, _polygons[idx].Count);
            //    }
            //}

            lock (_lines)
            {
                for (int idx = 0; idx < _lines.Count; idx++)
                {
                    GL.VertexPointer(3, All.Float, 0, _lines[idx].Vertices);
                    GL.EnableClientState(All.VertexArray);

                    var color = SimpleColor.FromArgb(_polygons[idx].Color);
                    GL.Color4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
                    GL.LineWidth(_lines[idx].Width);
                    GL.DrawArrays(All.LineStrip, 0, _lines[idx].Count);
                }
            }
        }

        /// <summary>
        /// Sets the orhto transformation.
        /// </summary>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        /// <param name="top">Top.</param>
        /// <param name="bottom">Bottom.</param>
        internal void SetOrtho(float left, float right, float bottom, float top)
        {
            _left = left;
            _right = right;
            _bottom = bottom;
            _top = top;
        }

        /// <summary>
        /// Raises the surface changed event.
        /// </summary>
        /// <param name="gl">Gl.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public void OnSurfaceChanged(int width, int height)
        {
            // Replace the current matrix with the identity matrix
            GL.MatrixMode(All.ProjectionMatrix);
            GL.LoadIdentity();
            GL.Ortho(0, width, 0, height, -1, 1);

            // Translates 4 units into the screen.
            GL.MatrixMode(All.ModelviewMatrix);
            GL.LoadIdentity(); // OpenGL docs

            var color = SimpleColor.FromKnownColor(KnownColor.White);
            GL.ClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        #endregion

        private class PolygonProcessed
        {
            public float[] Vertices { get; set; }

            public int Color { get; set; }

            public int Count
            {
                get;
                set;
            }
        }

        private class LineProcessed
        {
            public float[] Vertices { get; set; }

            public int Color { get; set; }

            public float Width
            {
                get;
                set;
            }

            public int Count
            {
                get;
                set;
            }
        }
    }
}