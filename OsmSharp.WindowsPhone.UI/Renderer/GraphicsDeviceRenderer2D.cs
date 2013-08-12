using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using OsmSharp.UI.Renderer;
using Microsoft.Xna.Framework.Graphics;

namespace PhoneClassLibrary4.Renderer
{
    /// <summary>
    /// A renderer implemented against an XNA-GraphicsDevice.
    /// </summary>
    public class GraphicsDeviceRenderer2D : Renderer2D<GraphicsDevice>
    {
        /// <summary>
        /// Holds the view.
        /// </summary>
        private View2D _view;

        /// <summary>
        /// Holds the target.
        /// </summary>
        private Target2DWrapper<GraphicsDevice> _target;

        /// <summary>
        /// Returns the target wrapper.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override Target2DWrapper<GraphicsDevice> CreateTarget2DWrapper(GraphicsDevice target)
        {
            return new Target2DWrapper<GraphicsDevice>(
                target, target.Viewport.Width, target.Viewport.Height);
        }

        /// <summary>
        /// Returns the size in pixels.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="view"></param>
        /// <param name="sizeInPixels"></param>
        /// <returns></returns>
        protected override double FromPixels(Target2DWrapper<GraphicsDevice> target, 
            View2D view, double sizeInPixels)
        {
            double scaleX = target.Width / view.Width;

            return sizeInPixels / scaleX;
        }

        /// <summary>
        /// Transforms the canvas to the coordinate system of the view.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="view"></param>
        protected override void Transform(Target2DWrapper<GraphicsDevice> target, View2D view)
        {
            _target = target;
            _view = view;
        }

        /// <summary>
        /// Transforms the x-coordinate to screen coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private float TransformX(double x)
        {
            return (float)_view.ToViewPortX(_target.Width, x);
        }

        /// <summary>
        /// Transforms the y-coordinate to screen coordinates.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        private float TransformY(double y)
        {
            return (float)_view.ToViewPortY(_target.Height, y);
        }

        /// <summary>
        /// Returns the size in pixels.
        /// </summary>
        /// <returns></returns>
        private float ToPixels(double sceneSize)
        {
            double scaleX = _target.Width / _view.Width;

            return (float)(sceneSize * scaleX) * 2;
        }


        protected override void DrawBackColor(Target2DWrapper<GraphicsDevice> target, int backColor)
        {
            //target.Target.
            //target.Target.DrawColor(new global::Android.Graphics.Color(backColor));
        }

        protected override void DrawPoint(Target2DWrapper<GraphicsDevice> target, double x, double y, int color, double size)
        {
            throw new NotImplementedException();
        }

        protected override void DrawLine(Target2DWrapper<GraphicsDevice> target, double[] x, double[] y, int color, double width, OsmSharp.UI.Renderer.Scene.Scene2DPrimitives.LineJoin lineJoin, int[] dashes)
        {
            throw new NotImplementedException();
        }

        protected override void DrawPolygon(Target2DWrapper<GraphicsDevice> target, double[] x, double[] y, int color, double width, bool fill)
        {
            throw new NotImplementedException();
        }

        protected override void DrawIcon(Target2DWrapper<GraphicsDevice> target, double x, double y, byte[] imageData)
        {
            throw new NotImplementedException();
        }

        protected override object DrawImage(Target2DWrapper<GraphicsDevice> target, double left, double top, double right, double bottom, byte[] imageData, object tag)
        {
            throw new NotImplementedException();
        }

        protected override void DrawText(Target2DWrapper<GraphicsDevice> target, double x, double y, string text, int color, double size, int? haloColor, int? haloRadius)
        {
            throw new NotImplementedException();
        }

        protected override void DrawLineText(Target2DWrapper<GraphicsDevice> target, double[] x, double[] y, string text, int color, double size, int? haloColor, int? haloRadius)
        {
            throw new NotImplementedException();
        }
    }
}
