using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;

namespace TiledDataService
{
    [Route("/data/{X}/{Y}/{Zoom}/{File}", "GET")] 
    public class DataOperation : IReturn<string>
    {
        /// <summary>
        /// The filename to get a piece out of.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// The x-coordinate of the tile.
        /// </summary>
        public string X { get; set; }

        /// <summary>
        /// The y-coordinate of the tile.
        /// </summary>
        public string Y { get; set; }

        /// <summary>
        /// The zoom level of the tile.
        /// </summary>
        public string Zoom { get; set; }
    }
}
