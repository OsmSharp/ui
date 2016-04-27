using OsmSharp.UI.Renderer.Images;
using OsmSharp.UI.Renderer.Primitives;

namespace OsmSharp.Wpf.UI.Renderer.Images
{
    /// <summary>
    /// Represents a native image cache.
    /// </summary>
    public class NativeImageCache : NativeImageCacheBase
    {
        /// <summary>
        /// Obtains a new image from this cache.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override INativeImage Obtain(byte[] data)
        {
            return new NativeImage(data);
        }

        /// <summary>
        /// Release the given image.
        /// </summary>
        /// <param name="image">The image to release.</param>
        public override void Release(INativeImage image)
        {
            image.Dispose();
        }

        /// <summary>
        /// Flushes all images from the cache.
        /// </summary>
        public override void Flush()
        {
            // nothing to do.
        }
    }
}
