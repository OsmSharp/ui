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
    /// A renderer implemented agains an XNA-GraphicsDevice.
    /// </summary>
    public class GraphicsDeviceRenderer2D : Renderer2D<GraphicsDevice>
    {
        public override Target2DWrapper<GraphicsDevice> CreateTarget2DWrapper(GraphicsDevice target)
        {
            throw new NotImplementedException();
        }

        protected override double FromPixels(Target2DWrapper<GraphicsDevice> target, View2D view, double sizeInPixels)
        {
            throw new NotImplementedException();
        }

        protected override void Transform(Target2DWrapper<GraphicsDevice> target, View2D view)
        {
            throw new NotImplementedException();
        }

        protected override void DrawBackColor(Target2DWrapper<GraphicsDevice> target, int backColor)
        {
            throw new NotImplementedException();
        }

        protected override void DrawPoint(Target2DWrapper<GraphicsDevice> target, double x, double y, int color, double size)
        {
            throw new NotImplementedException();
        }

        protected override void DrawLine(Target2DWrapper<GraphicsDevice> target, double[] x, double[] y, int color, double width, OsmSharp.UI.Renderer.Scene2DPrimitives.LineJoin lineJoin, int[] dashes)
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
