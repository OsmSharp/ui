// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Core.Output
{
    public static class OutputTextStreamHost
    {
        private static IList<IOutputTextStream> _output_streams;

        private static void InitializeIfNeeded()
        {
            if (_output_streams == null)
            {
                _output_streams = new List<IOutputTextStream>();
                //_output_streams.Add(new ConsoleOutputStream());
            }
        }

        public static void WriteLine()
        {
            OutputTextStreamHost.WriteLine(string.Empty);
        }

        public static void WriteLine(string text)
        {
            OutputTextStreamHost.InitializeIfNeeded();

            foreach (IOutputTextStream stream in _output_streams)
            {
                stream.WriteLine(text);
            }
        }

        public static void WriteLine(string format, params object[] arg)
        {
            OutputTextStreamHost.WriteLine(
                string.Format(format, arg));
        }

        public static void Write(string text)
        {
            OutputTextStreamHost.InitializeIfNeeded();

            foreach (IOutputTextStream stream in _output_streams)
            {
                stream.Write(text);
            }
        }

        public static void Write(string format, params object[] arg)
        {
            OutputTextStreamHost.Write(
                string.Format(format, arg));
        }

        public static void RegisterOutputStream(
            IOutputTextStream output_stream)
        {
            OutputTextStreamHost.InitializeIfNeeded();

            _output_streams.Add(output_stream);
        }

        public static void UnRegisterOutputStream(
            IOutputTextStream output_stream)
        {
            OutputTextStreamHost.InitializeIfNeeded();

            _output_streams.Remove(output_stream);
        }
        
    }
}
