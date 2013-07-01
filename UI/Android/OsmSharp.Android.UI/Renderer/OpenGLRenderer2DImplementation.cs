using System;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using Java.Nio;

namespace OsmSharp.Android.UI
{
	/// <summary>
	/// Open GL ES 2D Renderer.
	/// </summary>
	public class OpenGLRenderer2DImplementation : Java.Lang.Object, GLSurfaceView.IRenderer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Android.UI.OpenGLRenderer2DImplementation"/> class.
		/// </summary>
		/// <param name="square">Square.</param>
		public OpenGLRenderer2DImplementation (OpenGLRenderer2D renderer)
		{

		}

		#region IRenderer implementation

		public void OnDrawFrame (IGL10 gl)
		{
			// Our vertices.
			float[] vertices = {
				-1.5f,  1.5f, 0.0f,  // 0, Top Left
				-1.5f, -1.5f, 0.0f,  // 1, Bottom Left
				1.5f,  1.5f, 0.0f,  // 3, Top Right
				1.5f, -1.5f, 0.0f,  // 2, Bottom Right
			};

			// a float is 4 bytes, therefore we multiply the number if
			// vertices with 4.
			ByteBuffer vbb = ByteBuffer.AllocateDirect(vertices.Length * 4);
			vbb.Order(ByteOrder.NativeOrder());
			FloatBuffer vertexBuffer = vbb.AsFloatBuffer();
			vertexBuffer.Put(vertices);
			vertexBuffer.Position(0);

			// The order we like to connect them.
			byte[] colors = {
				(byte)0.0, (byte)255, (byte)0.0, (byte)255,  
				(byte)0.0, (byte)255, (byte)0.0, (byte)255,  
				(byte)0.0, (byte)255, (byte)0.0, (byte)255,  
				(byte)0.0, (byte)255, (byte)0.0, (byte)255  
			};

			// a float is 4 bytes, therefore we multiply the number if
			// vertices with 4.
			ByteBuffer cbb = ByteBuffer.AllocateDirect(colors.Length * 4);
			cbb.Order(ByteOrder.NativeOrder());
			cbb.Put (colors);
			cbb.Position (0);

			gl.GlVertexPointer (3, GL10.GlFloat, 0, vertexBuffer);
			gl.GlEnableClientState (GL10.GlVertexArray);
			gl.GlColorPointer(4, GL10.GlUnsignedByte, 0, cbb);
			gl.GlEnableClientState (GL10.GlColorArray);

			gl.GlDrawArrays (GL10.GlTriangleStrip, 0, 4);
		}

		public void OnSurfaceChanged (IGL10 gl, int width, int height)
		{
			// Replace the current matrix with the identity matrix
			gl.GlMatrixMode (GL10.GlProjection);
			gl.GlLoadIdentity(); // OpenGL docs
			gl.GlOrthof (-2, 2, -2, 2, -1, 10);

			// Translates 4 units into the screen.
			gl.GlMatrixMode (GL10.GlModelview);
			//gl.GlTranslatef(0, 0, -4); // OpenGL docs

			gl.GlClearColor (255, 255, 255, 255);
			gl.GlClear(GL10.GlColorBufferBit | // OpenGL docs.
			           GL10.GlDepthBufferBit);
		}

		public void OnSurfaceCreated (IGL10 gl, EGLConfig config)
		{

		}

		#endregion
	}
}

