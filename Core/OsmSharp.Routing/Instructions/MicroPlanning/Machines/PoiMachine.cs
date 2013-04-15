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
using OsmSharp.Tools.Math.Automata;
using OsmSharp.Routing.Interpreter;

namespace OsmSharp.Routing.Instructions.MicroPlanning.Machines
{
    internal class PoiMachine : MicroPlannerMachine
    {
        public PoiMachine(MicroPlanner planner)
            : base(PoiMachine.Initialize(), planner, 1000)
        {

        }

        /// <summary>
        /// Initializes this machine.
        /// </summary>
        /// <returns></returns>
        private static FiniteStateMachineState<MicroPlannerMessage> Initialize()
        {
            // generate states.
            List<FiniteStateMachineState<MicroPlannerMessage>> states = FiniteStateMachineState<MicroPlannerMessage>.Generate(2);

            // state 2 is final.
            states[1].Final = true;

            // 0
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 0, 0, typeof(MicroPlannerMessageArc));
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 0, 0, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition<MicroPlannerMessage>.FiniteStateMachineTransitionConditionDelegate(TestNonSignificantTurnNonPoi));
            FiniteStateMachineTransition<MicroPlannerMessage>.Generate(states, 0, 1, typeof(MicroPlannerMessagePoint),
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
            //if (!PoiMachine.TestPoi(test))
            //{
            //    if (test is MicroPlannerMessagePoint)
            //    {
            //        MicroPlannerMessagePoint point = (test as MicroPlannerMessagePoint);
            //        if (point.Point.Angle != null)
            //        {
            //            if (point.Point.ArcsNotTaken == null || point.Point.ArcsNotTaken.Count == 0)
            //            {
            //                return true;
            //            }
            //            switch (point.Point.Angle.Direction)
            //            {
            //                case OsmSharp.Tools.Math.Geo.Meta.RelativeDirectionEnum.StraightOn:
            //                    return true;
            //            }
            //        }
            //    }
            //    return false;
            //}
            return false;
        }

        ///// <summary>
        ///// Tests if the given turn is significant.
        ///// </summary>
        ///// <param name="test"></param>
        ///// <returns></returns>
        //private static bool TestNonSignificantTurnPoi(object test)
        //{
        //    if (PoiMachine.TestPoi(test))
        //    {
        //        if (test is MicroPlannerMessagePoint)
        //        {
        //            MicroPlannerMessagePoint point = (test as MicroPlannerMessagePoint);
        //            if (point.Point.Angle != null)
        //            {
        //                if (point.Point.ArcsNotTaken == null || point.Point.ArcsNotTaken.Count == 0)
        //                {
        //                    return true;
        //                }
        //                switch (point.Point.Angle.Direction)
        //                {
        //                    case OsmSharp.Tools.Math.Geo.Meta.RelativeDirectionEnum.StraightOn:
        //                        return true;
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}


        /// <summary>
        /// Tests if the given point is a poi.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="test"></param>
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
            List<Routing.ArcAggregation.Output.PointPoi> pois = 
                (this.FinalMessages[this.FinalMessages.Count - 1] as MicroPlannerMessagePoint).Point.Points;

            // construct the box indicating the location of the resulting find by this machine.
            GeoCoordinate point1 = pois[0].Location;
            GeoCoordinateBox box = new GeoCoordinateBox(
                new GeoCoordinate(point1.Latitude - 0.001f, point1.Longitude - 0.001f),
                new GeoCoordinate(point1.Latitude + 0.001f, point1.Longitude + 0.001f));

            // let the scentence planner generate the correct information.
            this.Planner.SentencePlanner.GeneratePoi(box, pois, null);
        }

        public override bool Equals(object obj)
        {
            if (obj is PoiMachine)
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
