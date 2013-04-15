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

namespace OsmSharp.Tools.Output
{
    /// <summary>
    /// Writes the output to console.
    /// </summary>
    public class ConsoleOutputStream : IOutputStream
    {
        #region IOutputTextStream Members

        /// <summary>
        /// Writes text.
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        /// <summary>
        /// Writes text.
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {
            Console.Write(text);
        }

        private string _previous_progress_string;

        //private string _previous_key;

        /// <summary>
        /// Reports progress.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="key"></param>
        /// <param name="message"></param>
        public void ReportProgress(double progress, string key, string message)
        {
            string current_progress_string = message;//string.Format("{0}:{1}", key, message);
            if (current_progress_string == _previous_progress_string)
            {
#if __ANDROID__
                Console.WriteLine(); // resetting cursor position not support in Mono for Android!
#elif WINDOWS_PHONE
                Console.WriteLine(); // resetting cursor position not support in Mono for Android!
#elif IOS
				Console.WriteLine(); // resetting cursor position not supported in Monotouch!
#else
				Console.SetCursorPosition(0, Console.CursorTop);
#endif
            }
            else
            {
                Console.WriteLine();
            }

            Console.Write(string.Format("{0} : {1}%",
                current_progress_string, System.Math.Round(progress * 100, 2).ToString().PadRight(6), key, message));
            _previous_progress_string = current_progress_string;
        }

        #endregion
    }
}
