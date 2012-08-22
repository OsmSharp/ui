using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Core.Output
{
    /// <summary>
    /// Interface representing a class that can be used as an output stream.
    /// </summary>
    public interface IOutputTextStream
    {
        /// <summary>
        /// Writes a line of text to the output stream.
        /// </summary>
        /// <param name="text"></param>
        void WriteLine(string text);

        /// <summary>
        /// Writes text to the output stream.
        /// </summary>
        /// <param name="text"></param>
        void Write(string text);
    }
}
