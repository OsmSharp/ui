using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Map.Styles.MapCSS
{
    /// <summary>
    /// Abstract representation of a MapCSS image source.
    /// </summary>
    public interface IMapCSSImageSource
    {
        /// <summary>
        /// Returns true if the image with the given name exists and sets the output parameter with the image data.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="imageData"></param>
        /// <returns></returns>
        bool TryGet(string name, out byte[] imageData);
    }
}
