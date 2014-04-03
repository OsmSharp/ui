using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharp.WinForms.OpenTK.UI.Renderer
{
    class OpenTKRenderer2D : Renderer2D<OpenTKTarget2D>
    {
        /// <summary>
        /// Holds the view.
        /// </summary>
        private View2D _view;

        /// <summary>
        /// Holds the target.
        /// </summary>
        private Target2DWrapper<OpenTKTarget2D> _target;

        private float _z;

        /// <summary>
        /// Called before rendering starts.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="scenes"></param>
        /// <param name="view"></param>
        protected override void OnBeforeRender(Target2DWrapper<OpenTKTarget2D> target, View2D view)
        {
            base.OnBeforeRender(target, view);

            _target.Target.Clear();
            _z = 0.0001f;
        }

        /// <summary>
        /// Transforms the y-coordinate to screen coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private double[] Tranform(double x, double y)
        {
            return _view.ToViewPort(_target.Width, _target.Height, x, y);
        }

        /// <summary>
        /// Returns the size in pixels.
        /// </summary>
        /// <returns></returns>
        private float ToPixels(double sceneSize)
        {
            double scaleX = _target.Width / _view.Width;

            return (float)(sceneSize * scaleX);
        }

        #region implemented abstract members of Renderer2D

        public override Target2DWrapper<OpenTKTarget2D> CreateTarget2DWrapper(OpenTKTarget2D target)
        {
            _target = new Target2DWrapper<OpenTKTarget2D>(target,
                                                          target.Width,
                                                          target.Height);
            return _target;
        }

        protected override double FromPixels(Target2DWrapper<OpenTKTarget2D> target, View2D view, double sizeInPixels)
        {
            double scaleX = target.Width / view.Width;

            return sizeInPixels / scaleX;
        }

        protected override void Transform(Target2DWrapper<OpenTKTarget2D> target, View2D view)
        {
            _view = view;
            _target = target;

            //_target.Target.SetOrtho((float)_view.LeftTop[0], (float)_view.RightTop[0], 
            //                        (float)_view.LeftBottom[1], (float)_view.LeftTop[1]);
        }

        protected override void DrawBackColor(Target2DWrapper<OpenTKTarget2D> target, int backColor)
        {

        }

        protected override void DrawPoint(Target2DWrapper<OpenTKTarget2D> target, double x, double y, int color, double size)
        {

        }

        /// <summary>
        /// Draws a line on the target. The coordinates given are scene coordinates.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="color">Color.</param>
        /// <param name="width">Width.</param>
        /// <param name="lineJoin"></param>
        /// <param name="dashes"></param>
        protected override void DrawLine(Target2DWrapper<OpenTKTarget2D> target, double[] x, double[] y, int color,
                                          double width, LineJoin lineJoin, int[] dashes)
        {
            _z = _z + 0.0001f;

            float[] points = new float[x.Length * 3];
            for (int idx = 0; idx < x.Length; idx++)
            {
                int pathIdx = idx * 3;
                points[pathIdx + 0] = (float)x[idx];
                points[pathIdx + 1] = (float)y[idx];
                points[pathIdx + 2] = _z;
            }

            _target.Target.AddLine(points, this.ToPixels(width), color);
        }

        /// <summary>
        /// Draws a polygon on the target. The coordinates given are scene coordinates.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="color">Color.</param>
        /// <param name="width">Width.</param>
        /// <param name="fill">If set to <c>true</c> fill.</param>
        protected override void DrawPolygon(Target2DWrapper<OpenTKTarget2D> target, double[] x, double[] y,
                                             int color, double width, bool fill)
        {
            _z = _z + 0.0001f;

            float[] points = new float[x.Length * 3];
            for (int idx = 0; idx < x.Length; idx++)
            {
                int pathIdx = idx * 3;
                points[pathIdx + 0] = (float)x[idx];
                points[pathIdx + 1] = (float)y[idx];
                points[pathIdx + 2] = _z;
            }

            _target.Target.AddPolygon(points, color);
        }

        protected override void DrawIcon(Target2DWrapper<OpenTKTarget2D> target, double x, double y, byte[] imageData)
        {

        }

        protected override object DrawImage(Target2DWrapper<OpenTKTarget2D> target, double left, double top, double right, double bottom, byte[] imageData, object tag)
        {
            return null;
        }

        protected override object DrawImage(Target2DWrapper<OpenTKTarget2D> target, OsmSharp.Math.Primitives.RectangleF2D bounds, byte[] imageData, object tag)
        {
            return tag;
        }

        protected override void DrawText(Target2DWrapper<OpenTKTarget2D> target, double x, double y, string text, int color,
                                          double size, int? haloColor, int? haloRadius, string fontName)
        {

        }

        protected override void DrawLineText(Target2DWrapper<OpenTKTarget2D> target, double[] x, double[] y, string text, int color,
                                              double size, int? haloColor, int? haloRadius, string fontName)
        {

        }

        #endregion
    }
}
