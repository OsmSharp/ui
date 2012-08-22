using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Angle;

namespace Tools.Math.Geo.Meta
{
    public static class RelativeDirectionCalculator
    {
        public static RelativeDirection Calculate(GeoCoordinate from, GeoCoordinate along, GeoCoordinate to)
        {
            RelativeDirection direction = new RelativeDirection();

            double margin = 65;
            double straight_on = 10;
            double turn_back = 5;

            GeoCoordinateLine line_from = new GeoCoordinateLine(from, along);
            GeoCoordinateLine line_to = new GeoCoordinateLine(along, to);

            Degree angle = line_from.Direction.Angle(line_to.Direction);

            if (angle >= new Degree(360 - straight_on)
                || angle < new Degree(straight_on))
            {
                direction.Direction = RelativeDirectionEnum.StraightOn;
            }
            else if (angle >= new Degree(straight_on)
                && angle < new Degree(90 - margin))
            {
                direction.Direction = RelativeDirectionEnum.SlightlyLeft;
            }
            else if (angle >= new Degree(90 - margin)
                && angle < new Degree(90 + margin))
            {
                direction.Direction = RelativeDirectionEnum.Left;
            }
            else if (angle >= new Degree(90 + margin)
                && angle < new Degree(180 - turn_back))
            {
                direction.Direction = RelativeDirectionEnum.SharpLeft;
            }
            else if (angle >= new Degree(180 - turn_back)
                && angle < new Degree(180 + turn_back))
            {
                direction.Direction = RelativeDirectionEnum.TurnBack;
            }
            else if (angle >= new Degree(180 + turn_back)
                && angle < new Degree(270-margin))
            {
                direction.Direction = RelativeDirectionEnum.SharpRight;
            }
            else if (angle >= new Degree(270 - margin)
                && angle < new Degree(270 + margin))
            {
                direction.Direction = RelativeDirectionEnum.Right;
            }
            else if (angle >= new Degree(270 + margin)
                && angle < new Degree(360- straight_on))
            {
                direction.Direction = RelativeDirectionEnum.SlightlyRight;
            }
            //direction.Direction = RelativeDirectionEnum.StraightOn;
            direction.Angle = angle;

            return direction;
        }
    }
}
