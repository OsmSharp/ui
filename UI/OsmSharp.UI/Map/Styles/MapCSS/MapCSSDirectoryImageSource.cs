using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Map.Styles.MapCSS
{
    /// <summary>
    /// An image source that gets it's image from a file system directory.
    /// </summary>
    public class MapCSSDirectoryImageSource : IMapCSSImageSource
    {
        /// <summary>
        /// Holds the directory info.
        /// </summary>
        private readonly DirectoryInfo _directoryInfo;

        /// <summary>
        /// Creates a new MapCSS image source.
        /// </summary>
        public MapCSSDirectoryImageSource(string path)
        {
            _directoryInfo = new DirectoryInfo(path);
        }

        /// <summary>
        /// Returns true if the image with the given name exists and sets the output parameter with the image data.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public bool TryGet(string name, out byte[] imageData)
        {
            var imageFile = new FileInfo(_directoryInfo.FullName + name);
            if (imageFile.Exists)
            {
                FileStream fileStream = imageFile.OpenRead();
                imageData = new byte[fileStream.Length];
                fileStream.Read(imageData, 0, (int)fileStream.Length);
            }
            imageData = null;
            return false;
        }
    }
}