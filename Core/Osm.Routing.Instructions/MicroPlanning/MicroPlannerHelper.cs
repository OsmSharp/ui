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
using Tools.Math.Geo.Meta;
using Osm.Routing.Core.Roads.Tags;
using Osm.Routing.Core.ArcAggregation.Output;

namespace Osm.Routing.Instructions.MicroPlanning
{
    internal class MicroPlannerHelper
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


        public static int GetStraightOn(IList<MicroPlannerMessage> messages)
        {
            int straight = 0;
            foreach (MicroPlannerMessage message in messages)
            {
                if (message is MicroPlannerMessagePoint)
                {
                    MicroPlannerMessagePoint point = (message as MicroPlannerMessagePoint);
                    straight = straight + MicroPlannerHelper.GetStraightOn(point);
                }
            }
            return straight;
        }

        public static int GetStraightOn(MicroPlannerMessagePoint point)
        {
            int straight = 0;
            if (point.Point.ArcsNotTaken != null)
            {
                foreach (KeyValuePair<RelativeDirection, AggregatedArc> arc_pair in point.Point.ArcsNotTaken)
                {
                    if (!MicroPlannerHelper.IsTurn(arc_pair.Key.Direction))
                    {
                        RoadTagsInterpreterBase interpreter = new RoadTagsInterpreterBase(arc_pair.Value.Tags);

                        if (interpreter.IsImportantSideStreet())
                        {
                            straight++;
                        }
                    }
                }
            }
            return straight;
        }

        public static int GetLeft(IList<MicroPlannerMessage> messages)
        {
            int left = 0;
            foreach (MicroPlannerMessage message in messages)
            {
                if (message is MicroPlannerMessagePoint)
                {
                    MicroPlannerMessagePoint point = (message as MicroPlannerMessagePoint);
                    left = left + MicroPlannerHelper.GetLeft(point);
                }
            }
            return left;
        }

        public static int GetLeft(MicroPlannerMessagePoint point)
        {
            int left = 0;
            if (point.Point.ArcsNotTaken != null)
            {
                foreach (KeyValuePair<RelativeDirection, AggregatedArc> arc_pair in point.Point.ArcsNotTaken)
                {
                    if (MicroPlannerHelper.IsLeft(arc_pair.Key.Direction))
                    {
                        RoadTagsInterpreterBase interpreter = new RoadTagsInterpreterBase(arc_pair.Value.Tags);

                        if (interpreter.IsImportantSideStreet())
                        {
                            left++;
                        }
                    }
                }
            }
            return left;
        }

        public static int GetRight(IList<MicroPlannerMessage> messages)
        {
            int right = 0;
            foreach (MicroPlannerMessage message in messages)
            {
                if (message is MicroPlannerMessagePoint)
                {
                    MicroPlannerMessagePoint point = (message as MicroPlannerMessagePoint);
                    right = right + MicroPlannerHelper.GetRight(point);
                }
            }
            return right;
        }

        public static int GetRight(MicroPlannerMessagePoint point)
        {
            int right = 0;
            if (point.Point.ArcsNotTaken != null)
            {
                foreach (KeyValuePair<RelativeDirection, AggregatedArc> arc_pair in point.Point.ArcsNotTaken)
                {
                    if (MicroPlannerHelper.IsRight(arc_pair.Key.Direction))
                    {
                        RoadTagsInterpreterBase interpreter = new RoadTagsInterpreterBase(arc_pair.Value.Tags);

                        if (interpreter.IsImportantSideStreet())
                        {
                            right++;
                        }
                    }
                }
            }
            return right;
        }
    }
}
