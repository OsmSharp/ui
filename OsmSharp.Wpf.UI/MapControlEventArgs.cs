using System.Windows.Input;
using OsmSharp.Math.Geo;

namespace OsmSharp.Wpf.UI
{
    /// <summary>
    /// Represents map control event information.
    /// </summary>
    public class MapControlEventArgs : MouseEventArgs
    {
        internal MapControlEventArgs(MouseEventArgs mouseArgs,
            GeoCoordinate position)
            : base(mouseArgs.MouseDevice, mouseArgs.Timestamp)
        {
            Position = position;
        }

        /// <summary>
        /// Returns the position this event occured at.
        /// </summary>
        public GeoCoordinate Position { get; }
    }
}


//public class MapControlEventArgs : EventArgs
//{
//    internal MapControlEventArgs(MouseEventArgs mouseState, KeyEventArgs keyState, int mouseWheelDelta, GeoCoordinate position)
//    {
//        MouseState = mouseState;
//        KeyState = keyState;
//        MouseWheelDelta = mouseWheelDelta;
//        Position = position;
//    }

//    public MouseEventArgs MouseState { get; }
//    public KeyEventArgs KeyState { get; }
//    public int MouseWheelDelta { get; }

//    public GeoCoordinate Position { get; }
//}