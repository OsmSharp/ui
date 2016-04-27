using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;

namespace OsmSharp.Wpf.UI.Renderer
{
    public static class RenderExtensions
    {
        public static void OpenRender(this Target2DWrapper<RenderContext> target)
        {
            target.Target.OpenDrawingContext();
        }
        public static DrawingContext Render(this Target2DWrapper<RenderContext> target)
        {
            return target.Target.GetDrawingContext();
        }
        public static void CloseRender(this Target2DWrapper<RenderContext> target)
        {
            target.Target.CloseDrawingContext();
        }

        public static Point ToPoint(this double[] values)
        {
            if(values == null)
                throw new ArgumentNullException(nameof(values));
            if(values.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(values));
            return new Point(values[0], values[1]);
        }

        public static Color ToColor(this int value)
        {
            return Color.FromArgb((byte) (value >> 24 % 256), (byte) (value >> 16 % 256), (byte) (value >> 8 % 256),
                (byte) (value % 256));
        }
        public static Color? ToColor(this int? value)
        {
            return value?.ToColor();
        }

        public static Brush ToBrush(this int value)
        {
            var color = value.ToColor();
            return new SolidColorBrush(color);
        }
        public static Pen ToPen(this int value, double thickness)
        {
            var color = value.ToColor();
            var pen = new Pen(new SolidColorBrush(color), thickness);
            return pen;
        }

        public static int ToInt(this double value)
        {
            return (int)System.Math.Round(value);
        }

        public static DashStyle ToDashStyle(this int[] dashes, double offset)
        {
            if (dashes != null)
            {
                return new DashStyle(dashes.Select(d => (double)d), offset);
            }
            return null;
        }
        public static PenLineJoin ToPenLineJoin(this LineJoin lineJoin)
        {
            switch (lineJoin)
            {
                case LineJoin.Round:
                    return PenLineJoin.Round;
                case LineJoin.Miter:
                    return PenLineJoin.Miter;
                case LineJoin.Bevel:
                    return PenLineJoin.Bevel;
                default:
                    return PenLineJoin.Round;
            }
        }

        public static double Length(this Point[] points)
        {
            var num1 = 0.0;
            if (points.Length > 1)
            {
                for (int index = 1; index < points.Length; ++index)
                {
                    var num2 = points[index - 1].X - points[index].X;
                    var num3 = points[index - 1].Y - points[index].Y;
                    num1 += System.Math.Sqrt(num2 * num2 + num3 * num3);
                }
            }
            return num1;
        }

        public static void DrawImage(this DrawingContext drawingContext, Rect destRect, BitmapSource source)
        {
            drawingContext.DrawImage(source, destRect);
        }
        public static void DrawImage(this DrawingContext drawingContext, Point point, BitmapSource source)
        {
            var destRect = new Rect(point, new Size(source.Width, source.Height));
            DrawImage(drawingContext, destRect, source);
        }

        public static void DrawPoint(this DrawingContext drawingContext, Point center, double width, double height, Color color)
        {
            if (width > 0 && height > 0)
            {
                drawingContext.DrawEllipse(new SolidColorBrush(color), null, new Point(width/2, height/2), width/2, height/2);
            }
        }
        public static void DrawLine(this DrawingContext drawingContext, Point[] points, double width, Color color, PenLineCap startCap,
            PenLineCap endCap, PenLineJoin lineJoin, DashStyle dashStyle)
        {
            var pen = new Pen(new SolidColorBrush(color), width)
            {
                DashStyle = dashStyle,
                LineJoin = lineJoin,
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
            for (var i = 1; i < points.Length; i++)
            {
                drawingContext.DrawLine(pen, points[i - 1], points[i]);
            }
        }
        public static void DrawPolygon(this DrawingContext drawingContext, Point[] points, double width, Color color, bool fill)
        {
            if (points.Length > 0)
            {
                var figure = new PathFigure { StartPoint = points[0] };

                var myPathSegmentCollection = new PathSegmentCollection();
                for (var i = 1; i < points.Length; i++)
                    myPathSegmentCollection.Add(new LineSegment(points[i], true));

                figure.Segments = myPathSegmentCollection;
                var pathGeometry = new PathGeometry();
                pathGeometry.Figures.Add(figure);

                var brush = new SolidColorBrush(color);
                var pen = new Pen(brush, width);
                if (fill)
                {
                    drawingContext.DrawGeometry(brush, null, pathGeometry);
                }
                else
                {
                    drawingContext.DrawGeometry(null, pen, pathGeometry);
                }
            }
        }

        public static void DrawText(this DrawingContext drawingContext, string text, Point point, Typeface typeface, double emSize,
            Color color, Color? haloColor = null, double? haloRadius = null, double angle = 0)
        {
            var formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                typeface, emSize, new SolidColorBrush(color));

            var width = formattedText.Width + haloRadius.GetValueOrDefault()*2;
            var height = formattedText.Height + haloRadius.GetValueOrDefault()*2;

            var rotate = new RotateTransform(angle);
            var newRect = rotate.TransformBounds(new Rect(new Point(0, 0), new Size(width, height)));

            drawingContext.PushTransform(new TranslateTransform(-newRect.Left,-newRect.Top));
            drawingContext.PushTransform(rotate);
      
            if (haloColor.HasValue && haloRadius.HasValue && haloRadius.Value > 0)
            {
                var pen = new Pen(new SolidColorBrush(haloColor.Value), haloRadius.Value * 2);
                var textGeometry = formattedText.BuildGeometry(point);
                drawingContext.DrawGeometry(null, pen, textGeometry);
            }
            drawingContext.DrawText(formattedText, point);

            drawingContext.Pop();
            drawingContext.Pop();
        }

        public static void DrawTextLine(this DrawingContext drawingContext, string text, Point[] points, Typeface typeface, double emSize,
            Color color, Color? haloColor = null, double? haloRadius = null)
        {
            var brush = new SolidColorBrush(color);
            var formatedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                emSize, brush);
            var formattedChars =
                text.Select(
                    ch =>
                        new FormattedText(ch.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                            emSize,
                            brush));

            var figure = new PathFigure { StartPoint = points[0] };

            var myPathSegmentCollection = new PathSegmentCollection();
            for (var i = 1; i < points.Length; i++)
                myPathSegmentCollection.Add(new LineSegment(points[i], true));

            figure.Segments = myPathSegmentCollection;
            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(figure);

            var pathLength = points.Length();
            var scalingFactor = pathLength / (formatedText.Width + text.Length* haloRadius.GetValueOrDefault() * 2);
            var progress = 0d;

            foreach (var ch in formattedChars)
            {
                var width = scalingFactor * (ch.WidthIncludingTrailingWhitespace + haloRadius.GetValueOrDefault()*2);
                var height = scalingFactor * ch.Height;
                progress += width / 2 / pathLength;
                Point point, tangent;

                pathGeometry.GetPointAtFractionLength(progress, out point, out tangent);
                drawingContext.PushTransform(new TranslateTransform(point.X - width/2, point.Y - height / 2));
                drawingContext.PushTransform(new RotateTransform(System.Math.Atan2(tangent.Y, tangent.X)* 180 / System.Math.PI, width / 2, height / 2));
                drawingContext.PushTransform(new ScaleTransform(scalingFactor, scalingFactor));

                if (haloColor.HasValue && haloRadius.HasValue && haloRadius.Value > 0)
                {
                    var pen = new Pen(new SolidColorBrush(haloColor.Value), haloRadius.Value * 2);
                    var textGeometry = ch.BuildGeometry(new Point(0, 0));
                    drawingContext.DrawGeometry(null, pen, textGeometry);
                }
                drawingContext.DrawText(ch, new Point(0, 0));

                drawingContext.Pop();
                drawingContext.Pop();
                drawingContext.Pop();

                progress += width / 2 / pathLength;
            }
        }
    }
}
