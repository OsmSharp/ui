using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Core.Output
{
    public class ConsoleOutputStream : IOutputTextStream
    {
        #region IOutputTextStream Members

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public void Write(string text)
        {
            Console.Write(text);
        }

        #endregion
    }
}
