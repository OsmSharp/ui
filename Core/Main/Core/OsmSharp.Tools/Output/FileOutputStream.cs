using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OsmSharp.Tools.Output
{
    public class FileOutputStream : IOutputStream
    {
        private TextWriter _stream;

        public FileOutputStream(string file)
            :this((new FileInfo(file)).OpenWrite())
        {

        }

        public FileOutputStream(Stream stream)
        {
            _stream = new StreamWriter(stream);
        }

        #region IOutputTextStream Members

        public void WriteLine(string text)
        {
            _stream.WriteLine(text);

            _stream.Flush();
        }

        public void Write(string text)
        {
            _stream.Write(text);
        }

        private string _previous_progress_string;

        //private string _previous_key;

        public void ReportProgress(double progress, string key, string message)
        {
            //string current_progress_string = message;//string.Format("{0}:{1}", key, message);
            ////if (current_progress_string == _previous_progress_string)
            ////{
            ////    _stream.WriteLine(0, Console.CursorTop);
            ////}
            ////else
            ////{
            ////    _stream.WriteLine();
            ////}

            //////if (_previous_key != key)
            //////{
            //////    Console.WriteLine(key);
            //////}
            //////_previous_key = key;

            //Console.WriteLine(string.Format("{0} : {1}%",
            //    current_progress_string, System.Math.Round(progress * 100, 2).ToString().PadRight(6), key, message));
            //_previous_progress_string = current_progress_string;
        }

        #endregion


    }
}
