using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace OsmSharp.WinForms.OpenTK.UI.Renderer
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
            lock(_lines)
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
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity(); // OpenGL docs
            GL.Ortho(_left, _right, _bottom, _top, -1, 1);

            // clear.
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            lock (_polygons)
            {
                for (int idx = 0; idx < _polygons.Count; idx++)
                {
                    GL.VertexPointer(3, VertexPointerType.Float, 0, _polygons[idx].Vertices);
                    GL.EnableClientState(ArrayCap.VertexArray);
                    GL.Color4(Color.FromArgb(_polygons[idx].Color));

                    GL.DrawArrays(PrimitiveType.Polygon, 0, _polygons[idx].Count);
                }
            }

            lock (_lines)
            {
                for (int idx = 0; idx < _lines.Count; idx++)
                {
                    GL.VertexPointer(3, VertexPointerType.Float, 0, _lines[idx].Vertices);
                    GL.EnableClientState(ArrayCap.VertexArray);

                    GL.Color4(Color.FromArgb(_lines[idx].Color));
                    GL.LineWidth(_lines[idx].Width);
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, _lines[idx].Count);
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
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, width, 0, height, -1, 1);

            // Translates 4 units into the screen.
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity(); // OpenGL docs

            GL.ClearColor(Color.White);
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