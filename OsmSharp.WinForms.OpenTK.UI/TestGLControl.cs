using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharp.WinForms.OpenTK.UI
{
    public class TestGLControl : GLControl
    {
        private bool _loaded;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _loaded = true;
            GL.ClearColor(Color.SkyBlue);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (!_loaded) // Play nice
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            this.SwapBuffers();
        }
    }
}
