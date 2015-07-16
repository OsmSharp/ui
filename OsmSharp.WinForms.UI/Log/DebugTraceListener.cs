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

using System;
using OsmSharp.Logging;
using System.Diagnostics;

namespace OsmSharp.WinForms.UI.Logging
{
    /// <summary>
    /// A log trace listener that writes message to the debug.
    /// </summary>
    public class DebugTraceListener : OsmSharp.Logging.TraceListener
    {
        /// <summary>
        /// Creates a new debug trace listener.
        /// </summary>
        public DebugTraceListener()
        {

        }

        /// <summary>
        /// Writes the given message to the debug.
        /// </summary>
        public override void Write(string message)
        {
            Debug.Write(message);
        }

        /// <summary>
        /// Writes the given message to the debug.
        /// </summary>
        public override void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }
    }
}