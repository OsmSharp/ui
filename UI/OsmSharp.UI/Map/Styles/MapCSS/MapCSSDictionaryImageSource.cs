using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Map.Styles.MapCSS
{
    /// <summary>
    /// An in-memory image source.
    /// </summary>
    public class MapCSSDictionaryImageSource : IMapCSSImageSource
    {
        /// <summary>
        /// Holds all images.
        /// </summary>
        private Dictionary<string, byte[]> _images;

        /// <summary>
        /// Creates a new dictionary image source.
        /// </summary>
        public MapCSSDictionaryImageSource()
        {
            _images = new Dictionary<string, byte[]>();
        }

        /// <summary>
        /// Adds a new image.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="imageData"></param>
        public void Add(string key, byte[] imageData)
        {
            _images[key] = imageData;
        }

        /// <summary>
        /// Adds a new image.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stream"></param>
        public void Add(string key, Stream stream)
        {
            byte[] imageData = new byte[stream.Length];
            stream.Read(imageData, 0, (int)stream.Length);

            this.Add(key, imageData);
        }

        /// <summary>
        /// Returns an image for the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public bool TryGet(string name, out byte[] imageData)
        {
            return _images.TryGetValue(name, out imageData);
        }
    }
}