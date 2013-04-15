using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if SILVERLIGHT || WINDOWS_PHONE

namespace OsmSharp
{
    /// <summary>
    /// IClonable interface to build for Windows Phone.
    /// </summary>
    public interface ICloneable
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        object Clone();
    }
}

#endif
