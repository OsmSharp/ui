using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneClassLibrary4.Layers
{
    /// <summary>
    /// A standard tile layer.
    /// </summary>
    public class TileLayer : Microsoft.Phone.Controls.Maps.TileSource
    {
        /// <summary>
        /// Holds the different tile server.
        /// </summary>
        private readonly string[] _servers;

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        private readonly int _minZoom = 1;
        
        /// <summary>
        /// The maximum zoom.
        /// </summary>
        private readonly int _maxZoom = 18;

        /// <summary>
        /// Creates a new tile layer.
        /// </summary>
        /// <param name="servers"></param>
        public TileLayer(IEnumerable<string> servers)
        {
            _servers = new List<string>(servers).ToArray();
        }

        /// <summary>
        /// Creates a new tile layer.
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        public TileLayer(IEnumerable<string> servers, int minZoom, int maxZoom)
        {
            _minZoom = minZoom;
            _maxZoom = maxZoom;
            _servers = new List<string>(servers).ToArray();
        }

        /// <summary>
        /// Holds the current server.
        /// </summary>
        private int _currentServer = 0;

        /// <summary>
        /// Returns the Uri to retrieve the given tile.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            if (_minZoom <= zoomLevel && _maxZoom >= zoomLevel)
            { // the zoom level is verified.
                // build uri.
                var uri = new Uri(string.Format(_servers[_currentServer], zoomLevel, x, y));

                // move to the next server.
                _currentServer++;
                if (_currentServer >= _servers.Length)
                { // reset to zero if the last server was reached.
                    _currentServer = 0;
                }

                return uri;
            }
            return null;
        }
    }
}