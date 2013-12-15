// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Logging;

namespace OsmSharp.Android.UI.Log
{
    /// <summary>
    /// A log trace listener writes message to the android log.
    /// </summary>
    public class LogTraceListener : TraceListener
    {
        /// <summary>
        /// Writes the given message.
        /// </summary>
        /// <param name="message"></param>
        public override void Write(string message)
        {
            global::Android.Util.Log.Info("OL", message);
        }

        /// <summary>
        /// Writes the given message.
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message)
        {
            global::Android.Util.Log.Info("OL", message);
        }
    }
}