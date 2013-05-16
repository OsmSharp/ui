using System;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES11;
using OpenTK.Platform;
using OpenTK.Platform.Android;

using Android.Views;
using Android.Util;
using Android.Content;
using Android.Opengl;
using Java.Nio;
using Java.Lang;

namespace OsmSharp.Android.UI
{
	class OpenGLRenderer2D : Java.Lang.Object, GLSurfaceView.IRenderer
	{
		private static string TAG = "MyGLRenderer";

		private Triangle mTriangle;
		private Square   mSquare;

		private float[] mMVPMatrix = new float[16];
		private float[] mProjMatrix = new float[16];
		private float[] mVMatrix = new float[16];
		private float[] mRotationMatrix = new float[16];
		
		#region IRenderer implementation
		public void OnDrawFrame (Javax.Microedition.Khronos.Opengles.IGL10 gl)
		{
			// Draw background color
			GLES20.GlClear ((int)GLES20.GlColorBufferBit);
			
			// Set the camera position (View matrix)
			Matrix.SetLookAtM (mVMatrix, 0, 0, 0, -3, 0f, 0f, 0f, 0f, 1.0f, 0.0f);
			
			// Calculate the projection and view transformation
			Matrix.MultiplyMM (mMVPMatrix, 0, mProjMatrix, 0, mVMatrix, 0);
			
			// Draw square
			mSquare.Draw (mMVPMatrix);
			
			// Create a rotation for the triangle
			// long time = SystemClock.UptimeMillis() % 4000L;
			// float angle = 0.090f * ((int) time);
			Matrix.SetRotateM (mRotationMatrix, 0, Angle, 0, 0, -1.0f);
			
			// Combine the rotation matrix with the projection and camera view
			Matrix.MultiplyMM (mMVPMatrix, 0, mRotationMatrix, 0, mMVPMatrix, 0);
			
			// Draw triangle
			mTriangle.Draw (mMVPMatrix);
		}
		
		public void OnSurfaceChanged (Javax.Microedition.Khronos.Opengles.IGL10 gl, int width, int height)
		{
			// Adjust the viewport based on geometry changes,
			// such as screen rotation
			GLES20.GlViewport (0, 0, width, height);
			
			float ratio = (float)width / height;
			
			// this projection matrix is applied to object coordinates
			// in the onDrawFrame() method
			Matrix.FrustumM (mProjMatrix, 0, -ratio, ratio, -1, 1, 3, 7);
		}
		
		public void OnSurfaceCreated (Javax.Microedition.Khronos.Opengles.IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
		{
			// Set the background frame color
			GLES20.GlClearColor (0.0f, 0.0f, 0.0f, 1.0f);
			
			mTriangle = new Triangle ();
			mSquare = new Square ();
		}
		#endregion
		
		public static int LoadShader (int type, string shaderCode)
		{
			// create a vertex shader type (GLES20.GL_VERTEX_SHADER)
			// or a fragment shader type (GLES20.GL_FRAGMENT_SHADER)
			int shader = GLES20.GlCreateShader (type);
			
			// add the source code to the shader and compile it
			GLES20.GlShaderSource (shader, shaderCode);
			GLES20.GlCompileShader (shader);
			
			return shader;
		}
		
		/**
		* Utility method for debugging OpenGL calls. Provide the name of the call
		* just after making it:
		*
		* <pre>
		* mColorHandle = GLES20.glGetUniformLocation(mProgram, "vColor");
		* MyGLRenderer.checkGlError("glGetUniformLocation");</pre>
		*
		* If the operation is not successful, the check throws an error.
		*
		* @param glOperation - Name of the OpenGL call to check.
		*/
		public static void CheckGlError (string glOperation)
		{
			int error;
			while ((error = GLES20.GlGetError ()) != GLES20.GlNoError) {
				Log.Error (TAG, glOperation + ": glError " + error);
				throw new RuntimeException (glOperation + ": glError " + error);
			}
		}
		
		public float Angle {
			get;
			set;
		}

		class Square
		{
			private string vertexShaderCode =
				// This matrix member variable provides a hook to manipulate
				// the coordinates of the objects that use this vertex shader
				"uniform mat4 uMVPMatrix;" +
					
					"attribute vec4 vPosition;" +
					"void main() {" +
					// the matrix must be included as a modifier of gl_Position
					"  gl_Position = vPosition * uMVPMatrix;" +
					"}";
			private string fragmentShaderCode =
				"precision mediump float;" +
					"uniform vec4 vColor;" +
					"void main() {" +
					"  gl_FragColor = vColor;" +
					"}";
			private FloatBuffer vertexBuffer;
			private ShortBuffer drawListBuffer;
			private int mProgram;
			private int mPositionHandle;
			private int mColorHandle;
			private int mMVPMatrixHandle;
			
			// number of coordinates per vertex in this array
			static int COORDS_PER_VERTEX = 3;
			static float[] squareCoords = new float[] { 
				-0.5f,  0.5f, 0.0f,   // top left
				-0.5f, -0.5f, 0.0f,   // bottom left
				0.5f, -0.5f, 0.0f,    // bottom right
				0.5f,  0.5f, 0.0f };  // top right
			
			private short[] drawOrder = new short[] { 
				0, 
				1, 
				2, 
				0, 
				2, 
				3
			}; // order to draw vertices
			
			private int vertexStride = COORDS_PER_VERTEX * 4; // 4 bytes per vertex
			
			// Set color with red, green, blue and alpha (opacity) values
			float[] color = new float[] { 
				0.2f, 
				0.709803922f, 
				0.898039216f, 
				1.0f
			};
			
			public Square ()
			{
				// initialize vertex byte buffer for shape coordinates
				ByteBuffer bb = ByteBuffer.AllocateDirect (
					// (# of coordinate values * 4 bytes per float)
					squareCoords.Length * 4);
				bb.Order (ByteOrder.NativeOrder ());
				vertexBuffer = bb.AsFloatBuffer ();
				vertexBuffer.Put (squareCoords);
				vertexBuffer.Position (0);
				
				// initialize byte buffer for the draw list
				ByteBuffer dlb = ByteBuffer.AllocateDirect (
					// (# of coordinate values * 2 bytes per short)
					drawOrder.Length * 2);
				dlb.Order (ByteOrder.NativeOrder ());
				drawListBuffer = dlb.AsShortBuffer ();
				drawListBuffer.Put (drawOrder);
				drawListBuffer.Position (0);
				
				// prepare shaders and OpenGL program
				int vertexShader = OpenGLRenderer2D.LoadShader (GLES20.GlVertexShader,
				                                            vertexShaderCode);
				int fragmentShader = OpenGLRenderer2D.LoadShader (GLES20.GlFragmentShader,
				                                              fragmentShaderCode);
				
				mProgram = GLES20.GlCreateProgram ();             // create empty OpenGL Program
				GLES20.GlAttachShader (mProgram, vertexShader);   // add the vertex shader to program
				GLES20.GlAttachShader (mProgram, fragmentShader); // add the fragment shader to program
				GLES20.GlLinkProgram (mProgram);                  // create OpenGL program executables
			}
			
			public void Draw (float[] mvpMatrix)
			{
				// Add program to OpenGL environment
				GLES20.GlUseProgram (mProgram);
				
				// get handle to vertex shader's vPosition member
				mPositionHandle = GLES20.GlGetAttribLocation (mProgram, "vPosition");
				
				// Enable a handle to the triangle vertices
				GLES20.GlEnableVertexAttribArray (mPositionHandle);
				
				// Prepare the triangle coordinate data
				GLES20.GlVertexAttribPointer (mPositionHandle, COORDS_PER_VERTEX,
				                              GLES20.GlFloat, false,
				                              vertexStride, vertexBuffer);
				
				// get handle to fragment shader's vColor member
				mColorHandle = GLES20.GlGetUniformLocation (mProgram, "vColor");
				
				// Set color for drawing the triangle
				GLES20.GlUniform4fv (mColorHandle, 1, color, 0);
				
				// get handle to shape's transformation matrix
				mMVPMatrixHandle = GLES20.GlGetUniformLocation (mProgram, "uMVPMatrix");
				OpenGLRenderer2D.CheckGlError ("glGetUniformLocation");
				
				// Apply the projection and view transformation
				GLES20.GlUniformMatrix4fv (mMVPMatrixHandle, 1, false, mvpMatrix, 0);
				OpenGLRenderer2D.CheckGlError ("glUniformMatrix4fv");
				
				// Draw the square
				GLES20.GlDrawElements (GLES20.GlTriangles, drawOrder.Length,
				                       GLES20.GlUnsignedShort, drawListBuffer);
				
				// Disable vertex array
				GLES20.GlDisableVertexAttribArray (mPositionHandle);
			}
		}

		class Triangle
		{
			private string vertexShaderCode =
				// This matrix member variable provides a hook to manipulate
				// the coordinates of the objects that use this vertex shader
				"uniform mat4 uMVPMatrix;" +
					
					"attribute vec4 vPosition;" +
					"void main() {" +
					// the matrix must be included as a modifier of gl_Position
					"  gl_Position = vPosition * uMVPMatrix;" +
					"}";
			private string fragmentShaderCode =
				"precision mediump float;" +
					"uniform vec4 vColor;" +
					"void main() {" +
					"  gl_FragColor = vColor;" +
					"}";
			private FloatBuffer vertexBuffer;
			private int mProgram;
			private int mPositionHandle;
			private int mColorHandle;
			private int mMVPMatrixHandle;
			
			// number of coordinates per vertex in this array
			static int COORDS_PER_VERTEX = 3;
			static float[] triangleCoords = new float [] { // in counterclockwise order:
				0.0f,  0.9f, 0.0f,   // top
				-0.5f, -0.311004243f, 0.0f,   // bottom left
				0.5f, -0.311004243f, 0.0f    // bottom right
			};
			private int vertexCount = triangleCoords.Length / COORDS_PER_VERTEX;
			private int vertexStride = COORDS_PER_VERTEX * 4; // 4 bytes per vertex
			
			// Set color with red, green, blue and alpha (opacity) values
			float[] color = new float[] { 
				0.63671875f, 
				0.76953125f, 
				0.22265625f, 
				1.0f };
			
			public Triangle ()
			{
				// initialize vertex byte buffer for shape coordinates
				ByteBuffer bb = ByteBuffer.AllocateDirect (
					// (number of coordinate values * 4 bytes per float)
					triangleCoords.Length * 4);
				// use the device hardware's native byte order
				bb.Order (ByteOrder.NativeOrder ());
				
				// create a floating point buffer from the ByteBuffer
				vertexBuffer = bb.AsFloatBuffer ();
				// add the coordinates to the FloatBuffer
				vertexBuffer.Put (triangleCoords);
				// set the buffer to read the first coordinate
				vertexBuffer.Position (0);
				
				// prepare shaders and OpenGL program
				int vertexShader = OpenGLRenderer2D.LoadShader (GLES20.GlVertexShader,
				                                            vertexShaderCode);
				int fragmentShader = OpenGLRenderer2D.LoadShader (GLES20.GlFragmentShader,
				                                              fragmentShaderCode);
				
				mProgram = GLES20.GlCreateProgram ();             // create empty OpenGL Program
				GLES20.GlAttachShader (mProgram, vertexShader);   // add the vertex shader to program
				GLES20.GlAttachShader (mProgram, fragmentShader); // add the fragment shader to program
				GLES20.GlLinkProgram (mProgram);                  // create OpenGL program executables
			}
			
			public void Draw (float[] mvpMatrix)
			{
				// Add program to OpenGL environment
				GLES20.GlUseProgram (mProgram);
				
				// get handle to vertex shader's vPosition member
				mPositionHandle = GLES20.GlGetAttribLocation (mProgram, "vPosition");
				
				// Enable a handle to the triangle vertices
				GLES20.GlEnableVertexAttribArray (mPositionHandle);
				
				// Prepare the triangle coordinate data
				GLES20.GlVertexAttribPointer (mPositionHandle, COORDS_PER_VERTEX,
				                              GLES20.GlFloat, false,
				                              vertexStride, vertexBuffer);
				
				// get handle to fragment shader's vColor member
				mColorHandle = GLES20.GlGetUniformLocation (mProgram, "vColor");
				
				// Set color for drawing the triangle
				GLES20.GlUniform4fv (mColorHandle, 1, color, 0);
				
				// get handle to shape's transformation matrix
				mMVPMatrixHandle = GLES20.GlGetUniformLocation (mProgram, "uMVPMatrix");
				OpenGLRenderer2D.CheckGlError ("glGetUniformLocation");
				
				// Apply the projection and view transformation
				GLES20.GlUniformMatrix4fv (mMVPMatrixHandle, 1, false, mvpMatrix, 0);
				OpenGLRenderer2D.CheckGlError ("glUniformMatrix4fv");
				
				// Draw the triangle
				//GLES20.GlDrawArrays (GLES20.GlTriangles, 0, vertexCount);
				GLES20.GlDrawArrays (GLES20.GlLineStrip, 0, vertexCount);
				
				// Disable vertex array
				GLES20.GlDisableVertexAttribArray (mPositionHandle);
			}
		}
		
	}

}

