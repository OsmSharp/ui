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
