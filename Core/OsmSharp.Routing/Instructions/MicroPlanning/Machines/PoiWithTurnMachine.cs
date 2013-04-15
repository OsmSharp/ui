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
using OsmSharp.Tools.Math.StateMachines;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Geo.Meta;
using OsmSharp.Tools.Math.Automata;

namespace OsmSharp.Routing.Instructions.MicroPlanning.Machines
{
    internal class PoiWithTurnMachine : MicroPlannerMachine
    {
        public PoiWithTurnMachine(MicroPlanner planner)
            : base(PoiWithTurnMachine.Initialize(), planner, 1001)
        {

        }

        /// <summary>
        /// Initializes this machine.
        /// </summary>
        /// <returns></returns>
        private static FiniteStateMachineState<MicroPlannerMessage> Initialize()
        {
            // generate states.
            List<FiniteStateMachineState<MicroPlannerMessage>> states = FiniteStateMachineState<MicroPlannerMessage>.Generate(3);

            // state 3 is final.
            states[2].Final = true;

            // 0
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 0, 1, typeof(MicroPlannerMessageArc));

            // 1
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 1, 1, typeof(MicroPlannerMessageArc));
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 1, 1, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition<MicroPlannerMessage>.FiniteStateMachineTransitionConditionDelegate(TestNonSignificantTurnNonPoi));
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 1, 2, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition<MicroPlannerMessage>.FiniteStateMachineTransitionConditionDelegate(TestPoi));

            // return the start automata with intial state.
            return states[0];
        }

        /// <summary>
        /// Tests if the given turn is significant.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="machine"></param>
        /// <returns></returns>
        private static bool TestNonSignificantTurnNonPoi(FiniteStateMachine<MicroPlannerMessage> machine, object test)
        {
            if (!PoiWithTurnMachine.TestPoi(machine, test))
            {
                if (test is MicroPlannerMessagePoint)
                {
                    MicroPlannerMessagePoint point = (test as MicroPlannerMessagePoint);
                    if (point.Point.Angle != null)
                    {
                        if (point.Point.ArcsNotTaken == null || point.Point.ArcsNotTaken.Count == 0)
                        {
                            return true;
                        }
                        switch (point.Point.Angle.Direction)
                        {
                            case OsmSharp.Tools.Math.Geo.Meta.RelativeDirectionEnum.StraightOn:
                            case RelativeDirectionEnum.SlightlyLeft:
                            case RelativeDirectionEnum.SlightlyRight:
                                return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Tests if the given point is a poi.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="machine"></param>
        /// <returns></returns>
        private static bool TestPoi(FiniteStateMachine<MicroPlannerMessage> machine, object test)
        {
            if (test is MicroPlannerMessagePoint)
            {
                MicroPlannerMessagePoint point = (test as MicroPlannerMessagePoint);
                if (point.Point.Points != null && point.Point.Points.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Succes()
        {
            OsmSharp.Routing.ArcAggregation.Output.AggregatedPoint pois_point = (this.FinalMessages[this.FinalMessages.Count - 1] as MicroPlannerMessagePoint).Point;
            OsmSharp.Routing.ArcAggregation.Output.AggregatedArc previous_arc = (this.FinalMessages[this.FinalMessages.Count - 2] as MicroPlannerMessageArc).Arc;

            // get the pois list.
            List<Routing.ArcAggregation.Output.PointPoi> pois = (this.FinalMessages[this.FinalMessages.Count - 1] as MicroPlannerMessagePoint).Point.Points;

            // get the angle from the pois point.
            RelativeDirection direction = pois_point.Angle;

            // calculate the box.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            foreach (Routing.ArcAggregation.Output.PointPoi poi in pois)
            {
                coordinates.Add(poi.Location);
            }
            coordinates.Add(pois_point.Location);
            GeoCoordinateBox box = new GeoCoordinateBox(coordinates.ToArray());

            // let the scentence planner generate the correct information.
            this.Planner.SentencePlanner.GeneratePoi(box, pois, direction);
        }

        public override bool Equals(object obj)
        {
            if (obj is PoiWithTurnMachine)
            { // if the machine can be used more than once 
                // this comparision will have to be updated.
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {// if the machine can be used more than once 
            // this hashcode will have to be updated.
            return this.GetType().GetHashCode();
        }
    }
}
