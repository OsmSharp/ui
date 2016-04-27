using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OsmSharp.Wpf.UI.Renderer
{
    public class RenderContext
    {
        private readonly DrawingVisual _drawingVisual;
        private DrawingContext _drawingContext;

        public RenderContext(Size renderSize)
        {
            RenderSize = renderSize;

            _drawingVisual = new DrawingVisual
            {
                Clip = new RectangleGeometry(RenderRect),
                CacheMode = new BitmapCache()
            };
        }

        public Rect RenderRect => new Rect(RenderSize);
        public Size RenderSize { get; }
        

        public void OpenDrawingContext()
        {
            _drawingContext = _drawingVisual.RenderOpen();
            _drawingContext.PushClip(new RectangleGeometry(RenderRect));
        }
        public DrawingContext GetDrawingContext()
        {
            return _drawingContext;
        }
        public void CloseDrawingContext()
        {
            _drawingContext.Pop();
            _drawingContext.Close();
        }

        public BitmapSource BuildScene()
        {
            var scene = new RenderTargetBitmap(RenderSize.Width.ToInt(), RenderSize.Height.ToInt(), 96, 96, PixelFormats.Pbgra32);
            scene.Render(_drawingVisual);
            return scene;
        }
    }
}