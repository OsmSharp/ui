using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.StateMachines;
using Tools.Math.Geo.Meta;
using Tools.Math.Geo;
using Osm.Routing.Core.ArcAggregation.Output;

namespace Osm.Routing.Instructions.MicroPlanning.Machines
{
    /// <summary>
    /// Machine to detect significant turns.
    /// </summary>
    internal class TurnMachine : MicroPlannerMachine
    {
        public TurnMachine(MicroPlanner planner)
            : base(TurnMachine.Initialize(), planner, 100)
        {

        }

        /// <summary>
        /// Initializes this machine.
        /// </summary>
        /// <returns></returns>
        private static FiniteStateMachineState Initialize()
        {
            // generate states.
            List<FiniteStateMachineState> states = FiniteStateMachineState.Generate(3);

            // state 2 is final.
            states[2].Final = true;

            // 0
            FiniteStateMachineTransition.Generate(states, 0, 0, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition.FiniteStateMachineTransitionConditionDelegate(TestNonSignificantTurn));
            FiniteStateMachineTransition.Generate(states, 0, 1, typeof(MicroPlannerMessageArc));

            // 1
            FiniteStateMachineTransition.Generate(states, 1, 0, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition.FiniteStateMachineTransitionConditionDelegate(TestNonSignificantTurn));
            FiniteStateMachineTransition.Generate(states, 1, 2, typeof(MicroPlannerMessagePoint),
                new FiniteStateMachineTransitionCondition.FiniteStateMachineTransitionConditionDelegate(TestSignificantTurn));

            // return the start automata with intial state.
            return states[0];
        }

        /// <summary>
        /// Tests if the given turn is significant.
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool TestNonSignificantTurn(object test)
        {
            if (!TurnMachine.TestSignificantTurn(test))
            { // it is no signficant turn.
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tests if the given turn is significant.
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private static bool TestSignificantTurn(object test)
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
                        case RelativeDirectionEnum.SlightlyLeft:
                        case RelativeDirectionEnum.SlightlyRight:
                            // test to see if is needed to generate instruction.
                            // if there is no other straight on
                            int straight_count = MicroPlannerHelper.GetStraightOn(point);
                            if (straight_count > 0)
                            {
                                return true;
                            }
                            return false;
                        case Tools.Math.Geo.Meta.RelativeDirectionEnum.StraightOn:
                            // test to see if this is cross road or anything.
                            int left_count = MicroPlannerHelper.GetLeft(point);
                            int right_count = MicroPlannerHelper.GetRight(point);
                            if (left_count > 0 && right_count > 0)
                            { // this straight-on is important.
                                return true;
                            }
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

            // count the number of streets in the same turning direction as the turn
            // that was found.
            int count = 0;
            if (MicroPlannerHelper.IsLeft(latest_point.Angle.Direction))
            {
                count = MicroPlannerHelper.GetLeft(this.FinalMessages);
            }
            else if (MicroPlannerHelper.IsRight(latest_point.Angle.Direction))
            {
                count = MicroPlannerHelper.GetRight(this.FinalMessages);
            }

            // construct the box indicating the location of the resulting find by this machine.
            GeoCoordinate point1 = latest_point.Location;
            GeoCoordinateBox box = new GeoCoordinateBox(
                new GeoCoordinate(point1.Latitude - 0.001f, point1.Longitude - 0.001f),
                new GeoCoordinate(point1.Latitude + 0.001f, point1.Longitude + 0.001f));
        

            // let the scentence planner generate the correct information.
            this.Planner.SentencePlanner.GenerateTurn(box, latest_point.Angle, 0, count, latest_arc.Tags, latest_point.Next.Tags, latest_point.Points);
        }
//<<<<<<< .mine
        
//        public override bool Equals(object obj)
//        {
//            if (obj is ImmidateTurnMachine)
//            { // if the machine can be used more than once 
//                // this comparision will have to be updated.
//                return true;
//            }
//            return false;
//        }

//        public override int GetHashCode()
//        {// if the machine can be used more than once 
//            // this hashcode will have to be updated.
//            return this.GetType().GetHashCode();
//        }
//=======

        public override bool Equals(object obj)
        {
            if (obj is TurnMachine)
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
//>>>>>>> .r303
    }
}
