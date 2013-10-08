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
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using Javax.Microedition.Khronos.Egl;
using Java.Nio;
using OsmSharp.UI;
using System.Collections.Generic;

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// Open GL 2D Target.
	/// </summary>
	public class OpenGLTarget2D: Java.Lang.Object, GLSurfaceView.IRenderer
	{
		/// <summary>
		/// Holds the triangles list.
		/// </summary>
		private List<TriangleProcessed> _triangles;

		/// <summary>
		/// Holds the lines list.
		/// </summary>
		private List<LineProcessed> _lines;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.OpenGLTarget2D"/> class.
		/// </summary>
		public OpenGLTarget2D()
		{
			_triangles = new List<TriangleProcessed> ();
			_lines = new List<LineProcessed> ();
		}

        public void Clear()
        {
            lock (_triangles)
            {
                _lines.Clear();
                _triangles.Clear();
            }
        }

		/// <summary>
		/// Adds the line.
		/// </summary>
		/// <param name="points">The points.</param>
		/// <param name="argb">ARGB.</param>
		public void AddLine(float[] points, float width, int argb)
		{
            lock (_triangles)
            {
                // a float is 4 bytes, therefore we multiply the number if
                // vertices with 4.
                ByteBuffer vbb = ByteBuffer.AllocateDirect(points.Length * 4);
                vbb.Order(ByteOrder.NativeOrder());
                FloatBuffer vertexBuffer = vbb.AsFloatBuffer();
                vertexBuffer.Put(points);
                vertexBuffer.Position(0);

                LineProcessed line = new LineProcessed()
                {
                    Vertices = vertexBuffer,
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
		public void AddTriangles(float[] corners, int argb)
		{
            //lock (_triangles)
            //{
            //    // a float is 4 bytes, therefore we multiply the number if
            //    // vertices with 4.
            //    ByteBuffer vbb = ByteBuffer.AllocateDirect(corners.Length * 4);
            //    vbb.Order(ByteOrder.NativeOrder());
            //    FloatBuffer vertexBuffer = vbb.AsFloatBuffer();
            //    vertexBuffer.Put(corners);
            //    vertexBuffer.Position(0);

            //    // The order we like to connect them.
            //    SimpleColor color = new SimpleColor()
            //    {
            //        Value = argb
            //    };
            //    int count = corners.Length / 3;
            //    byte[] colors = new byte[count * 4];
            //    for (int idx = 0; idx < count; idx++)
            //    {
            //        colors[idx * 4] = (byte)color.R;
            //        colors[idx * 4 + 1] = (byte)color.G;
            //        colors[idx * 4 + 2] = (byte)color.B;
            //        colors[idx * 4 + 3] = (byte)color.A;
            //    }

            //    // a float is 4 bytes, therefore we multiply the number if
            //    // vertices with 4.
            //    ByteBuffer cbb = ByteBuffer.AllocateDirect(colors.Length);
            //    cbb.Order(ByteOrder.NativeOrder());
            //    cbb.Put(colors);
            //    cbb.Position(0);

            //    _triangles.Add(new TriangleProcessed()
            //    {
            //        Vertices = vertexBuffer,
            //        Colors = cbb,
            //        Count = corners.Length / 3
            //    });
            //}
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
		/// <param name="gl">Gl.</param>
        public void OnDrawFrame(IGL10 gl)
        {
            lock (_triangles)
            {
                // Replace the current matrix with the identity matrix
                gl.GlMatrixMode(GL10.GlProjection);
                gl.GlLoadIdentity(); // OpenGL docs
                gl.GlOrthof(_left, _right, _bottom, _top, -1, 1);


                SimpleColor color = SimpleColor.FromKnownColor(KnownColor.White);
                gl.GlClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
                gl.GlClear(GL10.GlColorBufferBit);

                for (int idx = 0; idx < _triangles.Count; idx++)
                {
                    gl.GlVertexPointer(3, GL10.GlFloat, 0, _triangles[idx].Vertices);
                    gl.GlEnableClientState(GL10.GlVertexArray);
                    gl.GlColorPointer(4, GL10.GlUnsignedByte, 0, _triangles[idx].Colors);
                    gl.GlEnableClientState(GL10.GlColorArray);

                    gl.GlDrawArrays(GL10.GlTriangleStrip, 0, _triangles[idx].Count);
                }

                for (int idx = 0; idx < _lines.Count; idx++)
                {
                    gl.GlVertexPointer(3, GL10.GlFloat, 0, _lines[idx].Vertices);
                    gl.GlEnableClientState(GL10.GlVertexArray);

                    color = new SimpleColor()
                    {
                        Value = _lines[idx].Color
                    };
                    gl.GlColor4f(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
                    gl.GlLineWidth(_lines[idx].Width);
                    gl.GlDrawArrays(GL10.GlLineStrip, 0, _lines[idx].Count);
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
		public void OnSurfaceChanged (IGL10 gl, int width, int height)
		{
			// Replace the current matrix with the identity matrix
			gl.GlMatrixMode (GL10.GlProjection);
			gl.GlLoadIdentity(); // OpenGL docs
			gl.GlOrthof (0, width, 0, height, -1, 1);

			// Translates 4 units into the screen.
			gl.GlMatrixMode (GL10.GlModelview);
			gl.GlLoadIdentity(); // OpenGL docs
			//gl.GlTranslatef(0, 0, -4); // OpenGL docs

			gl.GlClearColor (255, 255, 255, 255);
			gl.GlClear(GL10.GlColorBufferBit | // OpenGL docs.
			           GL10.GlDepthBufferBit);
		}

		/// <summary>
		/// Raises the surface created event.
		/// </summary>
		/// <param name="gl">Gl.</param>
		/// <param name="config">Config.</param>
		public void OnSurfaceCreated (IGL10 gl, EGLConfig config)
		{

		}

		#endregion

		private class TriangleProcessed
		{
			public FloatBuffer Vertices { get; set; }

			public ByteBuffer Colors { get; set; }

			public int Count {
				get;
				set;
			}
		}

		private class LineProcessed
		{


			public FloatBuffer Vertices { get; set; }

			public int Color { get; set; }

			public float Width {
				get;
				set;
			}

			public int Count {
				get;
				set;
			}
		}
	}
}

