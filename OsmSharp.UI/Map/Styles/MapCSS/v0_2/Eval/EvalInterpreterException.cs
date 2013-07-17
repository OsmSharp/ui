using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2.Eval
{
    public class EvalInterpreterException : Exception
    {
        public EvalInterpreterException(string message)
            : base(message)
        {

        }

        public EvalInterpreterException(string message, params object[] args)
            : base(string.Format(message, args))
        {

        }
    }
}
