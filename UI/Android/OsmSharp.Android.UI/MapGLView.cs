//using System;
//using Android.Opengl;
//using Android.Content;
//
//namespace OsmSharp.Android.UI
//{
//	public class MapGLView : GLSurfaceView
//	{
//		private OpenGLRenderer2D mRenderer;
//		
//		public MapGLView (Context context) : base (context)
//		{
//			// Create an OpenGL ES 2.0 context.
//			SetEGLContextClientVersion (2);
//			
//			// Set the Renderer for drawing on the GLSurfaceView
//			mRenderer = new OpenGLRenderer2D ();
//			SetRenderer (mRenderer);
//			
//			// Render the view only when there is a change in the drawing data
//			this.RenderMode = Rendermode.WhenDirty;
//		}
//	}
//}
//
