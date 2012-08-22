using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo.Meta;
using Osm.Routing.Core.ArcAggregation.Output;

namespace Osm.Routing.Core.ArcAggregation
{
    public class AggregatedHelper
    {
        public static bool IsLeft(RelativeDirectionEnum direction)
        {
            switch (direction)
            {
                case RelativeDirectionEnum.Left:
                case RelativeDirectionEnum.SharpLeft:
                case RelativeDirectionEnum.SlightlyLeft:
                    return true;
            }
            return false;
        }

        public static bool IsRight(RelativeDirectionEnum direction)
        {
            switch (direction)
            {
                case RelativeDirectionEnum.Right:
                case RelativeDirectionEnum.SharpRight:
                case RelativeDirectionEnum.SlightlyRight:
                    return true;
            }
            return false;
        }

        public static bool IsTurn(RelativeDirectionEnum direction)
        {
            switch (direction)
            {
                case RelativeDirectionEnum.StraightOn:
                    return false;
            }
            return true;
        }
    }
}
