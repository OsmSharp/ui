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
using OsmSharp.Tools;

namespace OsmSharp.Routing.Instructions.MicroPlanning.Machines
{
    /// <summary>
    /// Machine to detect roundabouts.
    /// </summary>
    internal class RoundaboutMachine : MicroPlannerMachine
    {
        public RoundaboutMachine(MicroPlanner planner)
            : base(RoundaboutMachine.Initialize(), planner, 200)
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

            // state 2 is final.
            states[2].Final = true;

            // 0
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 0, 0, typeof(MicroPlannerMessageArc));
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 0, 1, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition<MicroPlannerMessage>.FiniteStateMachineTransitionConditionDelegate(TestRoundaboutEntry));

            // 1
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 1, 1, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition<MicroPlannerMessage>.FiniteStateMachineTransitionConditionDelegate(TestNonRoundaboutExit));
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 1, 1, typeof(MicroPlannerMessageArc),
                new FiniteStateMachineTransitionCondition<MicroPlannerMessage>.FiniteStateMachineTransitionConditionDelegate(TestRoundaboutArc));

            // 2
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 1, 2, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition<MicroPlannerMessage>.FiniteStateMachineTransitionConditionDelegate(TestRoundaboutExit));

            // return the start automata with intial state.
            return states[0];
        }

        /// <summary>
        /// Tests if the given turn is a turn onto a roundabout.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool TestRoundaboutEntry(FiniteStateMachine<MicroPlannerMessage> machine, object test)
        {
            if (test is MicroPlannerMessagePoint)
            {
                MicroPlannerMessagePoint point = (test as MicroPlannerMessagePoint);
                if (point.Point.Next != null)
                {
                    if ((machine as MicroPlannerMachine).Planner.Interpreter.EdgeInterpreter.IsRoundabout(
                        point.Point.Next.Tags.ConvertFrom()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given object is an arc of a roundabout.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool TestRoundaboutArc(FiniteStateMachine<MicroPlannerMessage> machine, object test)
        {
            if (test is MicroPlannerMessageArc)
            {
                MicroPlannerMessageArc arc = (test as MicroPlannerMessageArc);
                if ((machine as MicroPlannerMachine).Planner.Interpreter.EdgeInterpreter.IsRoundabout(arc.Arc.Tags.ConvertFrom()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tests if the given turn is a turn out of a roundabout.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="machine"></param>
        /// <returns></returns>
        public static bool TestNonRoundaboutExit(FiniteStateMachine<MicroPlannerMessage> machine, object test)
        {
            return !RoundaboutMachine.TestRoundaboutExit(machine, test);
        }

        /// <summary>
        /// Tests if the given turn is a turn out of a roundabout.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool TestRoundaboutExit(FiniteStateMachine<MicroPlannerMessage> machine, object test)
        {
            if (test is MicroPlannerMessagePoint)
            {
                MicroPlannerMessagePoint point = (test as MicroPlannerMessagePoint);
                if (point.Point.Next != null)
                {
                    if ((machine as MicroPlannerMachine).Planner.Interpreter.EdgeInterpreter.IsRoundabout(point.Point.Next.Tags.ConvertFrom()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Succes()
        {
            // get the last arc and the last point.
            AggregatedArc latest_arc = (this.FinalMessages[this.FinalMessages.Count - 2] as MicroPlannerMessageArc).Arc;
            AggregatedPoint latest_point = (this.FinalMessages[this.FinalMessages.Count - 1] as MicroPlannerMessagePoint).Point;

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

            //string next_street = latest_point.Next.Name;

            // let the scentence planner generate the correct information.
            this.Planner.SentencePlanner.GenerateRoundabout(box, count - 1, latest_point.Next.Tags);
        }

        public override bool Equals(object obj)
        {
            if (obj is RoundaboutMachine)
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
