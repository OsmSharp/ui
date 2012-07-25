using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;

namespace Osm.Core.Filters
{
    internal class FilterBox : Filter
    {
        private GeoCoordinateBox _box;

        public FilterBox(GeoCoordinateBox box)
        {
            _box = box;
        }

        public override bool Evaluate(OsmBase obj)
        {
            if(obj.BoundingBox.Overlaps(_box))
            {
                if (obj.Type == OsmType.ChangeSet)
                {
                    foreach(OsmGeo geo in (obj as ChangeSet).Objects)
                    {
                        if(geo.Shape.Inside(_box))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else 
                {
                    if ((obj as OsmGeo).Shape.Inside(_box))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
