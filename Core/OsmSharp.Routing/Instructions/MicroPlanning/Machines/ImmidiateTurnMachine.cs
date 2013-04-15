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
using OsmSharp.Tools.Math.Geo.Meta;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.ArcAggregation.Output;
using OsmSharp.Tools.Math.Automata;

namespace OsmSharp.Routing.Instructions.MicroPlanning.Machines
{
    /// <summary>
    /// Machine to detect significant turns.
    /// </summary>
    internal class ImmidateTurnMachine : MicroPlannerMachine
    {
        public ImmidateTurnMachine(MicroPlanner planner)
            : base(ImmidateTurnMachine.Initialize(), planner, 101)
        {

        }

        /// <summary>
        /// Initializes this machine.
        /// </summary>
        /// <returns></returns>
        private static FiniteStateMachineState<MicroPlannerMessage> Initialize()
        {
            // generate states.
            List<FiniteStateMachineState<MicroPlannerMessage>> states = FiniteStateMachineState<MicroPlannerMessage>.Generate(5);

            // state 3 is final.
            states[4].Final = true;

            // 0
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 0, 1, typeof(MicroPlannerMessageArc));

            // 1
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 1, 0, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition<MicroPlannerMessage>.FiniteStateMachineTransitionConditionDelegate(TestNonSignificantTurn));
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 1, 2, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition<MicroPlannerMessage>.FiniteStateMachineTransitionConditionDelegate(TestSignificantTurn));
            // 2
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 2, 3, typeof(MicroPlannerMessageArc),
                new FiniteStateMachineTransitionCondition<MicroPlannerMessage>.FiniteStateMachineTransitionConditionDelegate(TestVeryShortArc));

            // 3
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 3, 4, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition<MicroPlannerMessage>.FiniteStateMachineTransitionConditionDelegate(TestSignificantTurn));

            // return the start automata with intial state.
            return states[0];
        }

        /// <summary>
        /// Returns true if the given test object is a very short arc!
        /// </summary>
        /// <param name="test"></param>
        /// <param name="machine"></param>
        /// <returns></returns>
        private static bool TestVeryShortArc(FiniteStateMachine<MicroPlannerMessage> machine, object test)
        {
            if (test is MicroPlannerMessageArc)
            {
                MicroPlannerMessageArc arc = (test as MicroPlannerMessageArc);
                return arc.Arc.Distance.Value < 20;
            }
            return false;
        }

        /// <summary>
        /// Tests if the given turn is significant.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool TestNonSignificantTurn(FiniteStateMachine<MicroPlannerMessage> machine, object test)
        {
            if (!ImmidateTurnMachine.TestSignificantTurn(machine, test))
            { // it is no signficant turn.
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tests if the given turn is significant.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool TestSignificantTurn(FiniteStateMachine<MicroPlannerMessage> machine, object test)
        {
            if (test is MicroPlannerMessagePoint)
            {
                MicroPlannerMessagePoint point = (test as MicroPlannerMessagePoint);
                if (point.Point.Angle != null)
                {
                    if (point.Point.ArcsNotTaken == null || point.Point.ArcsNotTaken.Count == 0)
                    {
                        return false;
                    }
                    switch (point.Point.Angle.Direction)
                    {
                        case OsmSharp.Tools.Math.Geo.Meta.RelativeDirectionEnum.StraightOn:
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public override void Succes()
        {
            // get the last arc and the last point.
            AggregatedArc latest_arc = (this.FinalMessages[this.FinalMessages.Count - 2] as MicroPlannerMessageArc).Arc;
            AggregatedPoint latest_point = (this.FinalMessages[this.FinalMessages.Count - 1] as MicroPlannerMessagePoint).Point;
            AggregatedArc second_latest_arc = (this.FinalMessages[this.FinalMessages.Count - 4] as MicroPlannerMessageArc).Arc;
            AggregatedPoint second_latest_point = (this.FinalMessages[this.FinalMessages.Count - 3] as MicroPlannerMessagePoint).Point;

            // count the number of streets in the same turning direction as the turn
            // that was found.
            int count = 0;
            if (MicroPlannerHelper.IsLeft(latest_point.Angle.Direction, this.Planner.Interpreter))
            {
                count = MicroPlannerHelper.GetLeft(this.FinalMessages, this.Planner.Interpreter);
            }
            else if (MicroPlannerHelper.IsRight(latest_point.Angle.Direction, this.Planner.Interpreter))
            {
                count = MicroPlannerHelper.GetRight(this.FinalMessages, this.Planner.Interpreter);
            }

            // construct the box indicating the location of the resulting find by this machine.
            GeoCoordinate point1 = latest_point.Location;
            GeoCoordinateBox box = new GeoCoordinateBox(
                new GeoCoordinate(point1.Latitude - 0.001f, point1.Longitude - 0.001f),
                new GeoCoordinate(point1.Latitude + 0.001f, point1.Longitude + 0.001f));

            // get all the names/direction/counts.
            List<KeyValuePair<string, string>> next_name = latest_point.Next.Tags;
            List<KeyValuePair<string, string>> between_name = latest_arc.Tags;
            List<KeyValuePair<string, string>> before_name = second_latest_arc.Tags;

            int first_count = count;

            RelativeDirection first_turn = second_latest_point.Angle;
            RelativeDirection second_turn = latest_point.Angle;
            
            // let the scentence planner generate the correct information.
            this.Planner.SentencePlanner.GenerateImmidiateTurn(box, before_name, first_turn, first_count, second_turn, between_name, next_name, latest_point.Points);
        }

        public override bool Equals(object obj)
        {
            if (obj is ImmidateTurnMachine)
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
