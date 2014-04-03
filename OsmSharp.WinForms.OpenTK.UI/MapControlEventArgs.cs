using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsmSharp.WinForms.OpenTK.UI
{
    /// <summary>
    /// Represents map control event information.
    /// </summary>
    public class MapControlEventArgs : MouseEventArgs
    {
        private GeoCoordinate _position;

        internal MapControlEventArgs(MouseEventArgs mouseArgs,
            GeoCoordinate position)
            : base(mouseArgs.Button, mouseArgs.Clicks, mouseArgs.X, mouseArgs.Y, mouseArgs.Delta)
        {
            _position = position;
        }

        /// <summary>
        /// Returns the position this event occured at.
        /// </summary>
        public GeoCoordinate Position
        {
            get
            {
                return _position;
            }
        }
    }
}
