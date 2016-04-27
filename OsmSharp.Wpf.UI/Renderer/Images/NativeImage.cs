using System;
using System.IO;
using System.Windows.Media.Imaging;
using OsmSharp.Logging;
using OsmSharp.UI.Renderer.Primitives;

namespace OsmSharp.Wpf.UI.Renderer.Images
{
    /// <summary>
    /// Represents a native image.
    /// </summary>
    public class NativeImage : INativeImage
    {
        /// <summary>
        /// Holds the native image.
        /// </summary>
        private readonly byte[] _image;

        /// <summary>
        /// Creates a wrapper native image.
        /// </summary>
        /// <param name="image"></param>
        public NativeImage(byte[] image)
        {
            if (image == null) { throw new ArgumentNullException(nameof(image), "Cannot create a native image wrapper around null"); }

            _image = image;
        }

        /// <summary>
        /// Gets or sets the native image.
        /// </summary>
        public BitmapSource Image
        {
            get
            {
                using (var stream = new MemoryStream(_image))
                {
                    var imageSource = new BitmapImage();

                    imageSource.BeginInit();
                    imageSource.CacheOption = BitmapCacheOption.OnLoad;
                    imageSource.StreamSource = stream;
                    imageSource.EndInit();

                    return imageSource;
                }
            }
        }
    
        #region Disposing-pattern

        /// <summary>
        /// Diposes of all resources associated with this object.
        /// </summary>
        public void Dispose()
        {
            // If this function is being called the user wants to release the
            // resources. lets call the Dispose which will do this for us.
            Dispose(true);

            // Now since we have done the cleanup already there is nothing left
            // for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Diposes of all resources associated with this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //someone want the deterministic release of all resources
                //Let us release all the managed resources
            }

            // Release the unmanaged resource in any case as they will not be 
            // released by GC
            if (_image != null)
            { // dispose of the native image.
                Logging.Log.TraceEvent("NativeImage", TraceEventType.Information, "NativeImage dispose");
            }
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~NativeImage()
        {
            // The object went out of scope and finalized is called
            // Lets call dispose in to release unmanaged resources 
            // the managed resources will anyways be released when GC 
            // runs the next time.
            Dispose(false);
        }

        #endregion

        /// <summary>
        /// Returns true when the given object equals this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = (obj as NativeImage);
            if (other != null)
            {
                return other._image.Equals(_image);
            }
            return false;
        }

        /// <summary>
        /// Returns the hashcode of this image.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var data = _image ?? new byte[0];

            return 613294639 ^
                data.GetHashCode();
        }
    }
}