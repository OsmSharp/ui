using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Renderer
{
    public interface ITarget
    {
        int XRes
        {
            get;
        }

        int YRes
        {
            get;
        }

        bool DisplayStatus
        {
            get;
        }

        bool DisplayAttributions
        {
            get;
        }

        bool DisplayCardinalDirections
        {
            get;
        }
    }
}
