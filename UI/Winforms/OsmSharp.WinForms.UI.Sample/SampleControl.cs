using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OsmSharp.UI.Renderer;
using OsmSharp.WinForms.UI.Renderer;

namespace OsmSharp.WinForms.UI.Sample
{
    public partial class SampleControl : UserControl
    {
        public SampleControl()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
        }

        /// <summary>
        /// Gets/sets the scene.
        /// </summary>
        public Scene2D Scene { get; set; }

        /// <summary>
        /// Gets/sets the view.
        /// </summary>
        public float[] Center { get; set; }

        /// <summary>
        /// Gets/sets the view factor.
        /// </summary>
        public float ZoomFactor { get; set; }

        /// <summary>
        /// Raises the paint event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Scene != null)
            { // only render when scene and view are set.
                var bitmap = new Bitmap((int) e.Graphics.VisibleClipBounds.Width,
                                           (int) e.Graphics.VisibleClipBounds.Height);
                Graphics g = Graphics.FromImage(bitmap);

                // create current view.
                View2D view = this.CreateCurrentView(g.VisibleClipBounds.Width,
                                                     g.VisibleClipBounds.Height);

                // initialize renderer.
                var graphicsRenderer2D = new GraphicsRenderer2D();

                // render the scene.
                graphicsRenderer2D.Render(g, this.Scene, view);

                e.Graphics.DrawImageUnscaled(bitmap, 0, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private View2D CreateCurrentView(float width, float height)
        {
            return View2D.CreateFrom(this.Center[0], this.Center[1],
                                                width, height, this.ZoomFactor);
        }

        /// <summary>
        /// Raises the onresize event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.Invalidate();
        }

        #region Drag/Zoom

        /// <summary>
        /// Coordinates where dragging started.
        /// </summary>
        private float[] _draggingCoordinates;

        /// <summary>
        /// Coordinates of the old center.
        /// </summary>
        private float[] _oldCenter;

        /// <summary>
        /// Raises the onmousedown event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                _draggingCoordinates = new float[]{ e.X, e.Y };
                _oldCenter = new float[] { this.Center[0], this.Center[1] };
            }
        }

        /// <summary>
        /// Raises the mousemove event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left &&
                _draggingCoordinates != null)
            {
                float[] currentCoordinates = new float[] { e.X, e.Y};
                float[] delta = new float[] { _draggingCoordinates[0] - currentCoordinates[0],
                        _draggingCoordinates[1] - currentCoordinates[1]};

                float[] newCenter = new float[] { (float)this.Width/2.0f + delta[0], (float)this.Height/2.0f + delta[1]};

                this.Center = _oldCenter;
                this.Center = this.CreateCurrentView(this.Width, this.Height).ToViewPort(this.Width, this.Height,
                                                                                         newCenter[0], newCenter[1]);

                //_draggingCoordinates = currentCoordinates;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Raises the mouseup event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _draggingCoordinates = null;
        }

        /// <summary>
        /// Raizes the on mouse click event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            //switch (e.Button)
            //{
            //    case MouseButtons.Left:

            //        // calculate the scene coordinates.
            //        float[] sceneCoordinates = this.CreateCurrentView(this.Width, this.Height).ToViewPort(this.Width, this.Height,
            //                                            e.X, e.Y);

            //        this.Center = sceneCoordinates;
            //        this.ZoomFactor++;

            //        this.Invalidate();
            //        break;
            //    case MouseButtons.None:
            //        break;
            //    case MouseButtons.Right:
            //        this.ZoomFactor--;
            //        this.Invalidate();
            //        break;
            //    case MouseButtons.Middle:
            //        break;
            //    case MouseButtons.XButton1:
            //        break;
            //    case MouseButtons.XButton2:
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
        }

        /// <summary>
        /// Raises the mousewheel event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            this.ZoomFactor += (float) (e.Delta/120.0);
            this.Invalidate();
        }

        #endregion
    }
}