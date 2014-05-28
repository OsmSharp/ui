using OsmSharp.WinForms.UI.Renderer.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.WinForms.UI
{
    /// <summary>
    /// Class responsable for creating native hooks for platform-specific functionality.
    /// </summary>
    public static class Native
    {
        /// <summary>
        /// Initializes some iOS-specifics for OsmSharp to use.
        /// </summary>
        public static void Initialize()
        {
            // intialize the native image cache factory.
            OsmSharp.UI.Renderer.Images.NativeImageCacheFactory.SetDelegate(
                () =>
                {
                    return new NativeImageCache();
                });
        }
    }
}