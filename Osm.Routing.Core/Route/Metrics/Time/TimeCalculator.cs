using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Speed;
using Tools.Math.Units.Time;
using Osm.Routing.Core;
using Osm.Routing.Core.ArcAggregation.Output;
using Osm.Routing.Core.ArcAggregation;

namespace Osm.Routing.Core.Metrics.Time
{
    public class TimeCalculator : OsmSharpRouteMetricCalculator
    {
        public const string TIME_KEY = "Time_in_seconds";
        public const string DISTANCE_KEY = "Distance_in_meter";

        public override Dictionary<string, double> Calculate(VehicleEnum vehicle, AggregatedPoint p)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            result.Add(DISTANCE_KEY, 0);
            result.Add(TIME_KEY, 0);

            Aggregated next = p;
            while (next != null)
            {
                if (next is AggregatedPoint)
                {
                    AggregatedPoint point = (next as AggregatedPoint);
                    this.CalculatePointMetrics(vehicle, result, point);
                }
                if (next is AggregatedArc)
                {
                    AggregatedArc arc = (next as AggregatedArc);
                    this.CalculateArcMetrics(vehicle, result, arc);
                }

                next = next.GetNext();
            }

            return result;
        }

        /// <summary>
        /// Calculate metrics for a given turn.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="result"></param>
        /// <param name="point"></param>
        private void CalculatePointMetrics(VehicleEnum vehicle, Dictionary<string, double> result, AggregatedPoint point)
        {
            if (point.Angle != null)
            {
                if (AggregatedHelper.IsTurn(point.Angle.Direction))
                {
                    // no calculations for distance.

                    // update the time.
                    Second second = 0;
                    // ESTIMATE THE INCREASE IN TIME.
                    // TODO: ASSUMED DRIVING ON THE RIGHT; UPDATE TO MAKE CONFIGURABLE.
                    switch (point.Angle.Direction)
                    {
                        case Tools.Math.Geo.Meta.RelativeDirectionEnum.Left:
                        case Tools.Math.Geo.Meta.RelativeDirectionEnum.SharpLeft:
                        case Tools.Math.Geo.Meta.RelativeDirectionEnum.SlightlyLeft:
                            second = 25;
                            break;
                        case Tools.Math.Geo.Meta.RelativeDirectionEnum.Right:
                        case Tools.Math.Geo.Meta.RelativeDirectionEnum.SharpRight:
                        case Tools.Math.Geo.Meta.RelativeDirectionEnum.SlightlyRight:
                            second = 5;
                            break;
                        case Tools.Math.Geo.Meta.RelativeDirectionEnum.TurnBack:
                            second = 30;
                            break;
                    }
                    result[TIME_KEY] = result[TIME_KEY] + second.Value;
                }
                else
                {
                    if (point.ArcsNotTaken != null && point.ArcsNotTaken.Count > 0)
                    { // very simple estimate.
                        Second second = 0;

                        second = 5;

                        result[TIME_KEY] = result[TIME_KEY] + second.Value;

                    }
                }
            }
        }

        /// <summary>
        /// Calculate metrics for a given arc.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="result"></param>
        /// <param name="arc"></param>
        private void CalculateArcMetrics(VehicleEnum vehicle, Dictionary<string, double> result, AggregatedArc arc)
        {
            // update the distance.
            result[DISTANCE_KEY] = result[DISTANCE_KEY] + arc.Distance.Value;

            // update the time.
            Osm.Routing.Core.Roads.Tags.RoadTagsInterpreterBase road_interpreter =
                new Osm.Routing.Core.Roads.Tags.RoadTagsInterpreterBase(arc.Tags);
            KilometerPerHour speed = road_interpreter.MaxSpeed(vehicle);
            Second time = arc.Distance / speed;

            // FOR NOW USE A METRIC OF 75% MAX SPEED.
            // TODO: improve this for a more realistic estimated based on the type of road.
            result[TIME_KEY] = result[TIME_KEY] + time.Value;
        }
    }
}
