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

namespace OsmSharp.Tools.Core.Output
{
    public static class OutputStreamHost
    {
        private static IList<IOutputStream> _output_streams;

        private static void InitializeIfNeeded()
        {
            if (_output_streams == null)
            {
                _output_streams = new List<IOutputStream>();
                //_output_streams.Add(new ConsoleOutputStream());
            }
        }

        public static void WriteLine()
        {
            OutputStreamHost.WriteLine(string.Empty);
        }

        public static void WriteLine(string text)
        {
            OutputStreamHost.InitializeIfNeeded();

            foreach (IOutputStream stream in _output_streams)
            {
                stream.WriteLine(text);
            }
        }

        public static void WriteLine(string format, params object[] arg)
        {
            OutputStreamHost.WriteLine(
                string.Format(format, arg));
        }

        public static void Write(string text)
        {
            OutputStreamHost.InitializeIfNeeded();

            foreach (IOutputStream stream in _output_streams)
            {
                stream.Write(text);
            }
        }

        public static void Write(string format, params object[] arg)
        {
            OutputStreamHost.Write(
                string.Format(format, arg));
        }

        public static void ReportProgress(double progress, string key, string message)
        {
            OutputStreamHost.InitializeIfNeeded();

            if (progress > 1)
            {
                progress = 1;
            }
            if (progress < 0)
            {
                progress = 0;
            }
            foreach (IOutputStream stream in _output_streams)
            {
                stream.ReportProgress(progress, key, message);
            }
        }

        public static void ReportProgress(long current, long total, string key, string message)
        {
            OutputStreamHost.ReportProgress((double)current / (double)total, key, message);
        }

        public static void RegisterOutputStream(
            IOutputStream output_stream)
        {
            OutputStreamHost.InitializeIfNeeded();

            _output_streams.Add(output_stream);
        }

        public static void UnRegisterOutputStream(
            IOutputStream output_stream)
        {
            OutputStreamHost.InitializeIfNeeded();

            _output_streams.Remove(output_stream);
        }
        
    }
}
