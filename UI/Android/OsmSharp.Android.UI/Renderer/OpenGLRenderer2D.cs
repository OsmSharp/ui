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

namespace OsmSharp.Android.UI
{
	public class OpenGLRenderer2D : AndroidGameView
	{
		int viewportWidth, viewportHeight;
		
		public OpenGLRenderer2D (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}
		
		public OpenGLRenderer2D (IntPtr handle, global::Android.Runtime.JniHandleOwnership transfer)
			: base (handle, transfer)
		{
			Initialize ();
		}
		
		private void Initialize ()
		{

		}
		
		// This method is called everytime the context needs
		// to be recreated. Use it to set any egl-specific settings
		// prior to context creation
		protected override void CreateFrameBuffer ()
		{
			GLContextVersion = GLContextVersion.Gles1_1;
			
			// the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
			try {
				Log.Verbose ("GLCube", "Loading with default settings");
				
				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("GLCube", "{0}", ex);
			}
			
			// this is a graphics setting that sets everything to the lowest mode possible so
			// the device returns a reliable graphics setting.
			try {
				Log.Verbose ("GLCube", "Loading with custom Android settings (low mode)");
				GraphicsMode = new AndroidGraphicsMode (0, 0, 0, 0, 0, false);
				
				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("GLCube", "{0}", ex);
			}
			throw new Exception ("Can't load egl, aborting");
		}
		
		
		// This gets called when the drawing surface is ready
		protected override void OnLoad (EventArgs e)
		{
			// this call is optional, and meant to raise delegates
			// in case any are registered
			base.OnLoad (e);
			
			// UpdateFrame and RenderFrame are called
			// by the render loop. This is takes effect
			// when we use 'Run ()', like below
//			UpdateFrame += delegate (object sender, FrameEventArgs args) {
//				// Rotate at a constant speed
//				for (int i = 0; i < 3; i ++)
//					rot [i] += (float) (rateOfRotationPS [i] * args.Time);
//			};
			
			RenderFrame += delegate {
				RenderCube ();
			};

			GL.Enable(All.CullFace);
			GL.ShadeModel(All.Smooth);
			
			GL.Hint(All.PerspectiveCorrectionHint, All.Nicest);
			
			// Run the render loop
			Run (30);
		}

		/// <summary>
		/// Resizes the control.
		/// </summary>
		/// <param name="e">E.</param>
		protected override void OnResize (EventArgs e)
		{
			int w = this.Width;
			int h = this.Height;

//			glControl1.MakeCurrent();
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.ClearColor(global::Android.Graphics.Color.SkyBlue);
			GL.Ortho(-w / 2, w / 2, -h / 2, h / 2, -1, 1);
			GL.Viewport(0, 0, w, h);
//			GL.End();
//			glControl1.SwapBuffers();
		}

		/// <summary>
		/// Render this instance.
		/// </summary>
		private void Render()
		{

		}
	}
}

