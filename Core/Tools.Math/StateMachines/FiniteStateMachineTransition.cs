// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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

namespace Tools.Math.StateMachines
{
    /// <summary>
    /// Represents a transition in a finite-state machine.
    /// </summary>
    public sealed class FiniteStateMachineTransition
    {
        /// <summary>
        /// The source state.
        /// </summary>
        public FiniteStateMachineState SourceState { get; private set; }

        /// <summary>
        /// The target state.
        /// </summary>
        public FiniteStateMachineState TargetState { get; private set; }

        /// <summary>
        /// The list of events this transition repsonds to.
        /// </summary>
        public List<FiniteStateMachineTransitionCondition> TransitionConditions { get; private set; }

        /// <summary>
        /// Boolean indicating not to respond to the listed events.
        /// </summary>
        public bool Inverted { get; private set; }

        /// <summary>
        /// Returns true if the given event triggers a response in this transition.
        /// </summary>
        /// <param name="incoming"></param>
        /// <returns></returns>
        internal bool Match(object even)
        {
            // get the value of the match.
            bool val = false;
            foreach (FiniteStateMachineTransitionCondition condition in this.TransitionConditions)
            {
                if (condition.Check(even))
                {
                    val = true;
                    break;
                }
            }

            // revert the value if needed.
            if (this.Inverted)
            {
                return !val;
            }
            else
            {
                return val;
            }
        }

        #region Generation

        /// <summary>
        /// Generates a non-inverted transition.
        /// </summary>
        /// <param name="states"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="event_types"></param>
        /// <returns></returns>
        public static FiniteStateMachineTransition Generate(List<FiniteStateMachineState> states, int start, int end, params Type[] event_types)
        {
            return FiniteStateMachineTransition.Generate(states, start, end, false, event_types);
        }

        /// <summary>
        /// Generates a non-inverted transition with an extra check-delegate!
        /// </summary>
        /// <param name="states"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="inverted"></param>
        /// <param name="event_type"></param>
        /// <param name="check_delegate"></param>
        /// <returns></returns>
        public static FiniteStateMachineTransition Generate(
            List<FiniteStateMachineState> states, int start, int end,Type event_type,
            Tools.Math.StateMachines.FiniteStateMachineTransitionCondition.FiniteStateMachineTransitionConditionDelegate check_delegate)
        {
            return FiniteStateMachineTransition.Generate(states, start, end, false, event_type, check_delegate);
        }

        /// <summary>
        /// Generates a transition with an extra check-delegate!
        /// </summary>
        /// <param name="states"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="event_types"></param>        
        /// <returns></returns>
        public static FiniteStateMachineTransition Generate(
            List<FiniteStateMachineState> states, int start, int end, bool inverted, Type event_type,
            Tools.Math.StateMachines.FiniteStateMachineTransitionCondition.FiniteStateMachineTransitionConditionDelegate check_delegate)
        {
            List<FiniteStateMachineTransitionCondition> conditions = new List<FiniteStateMachineTransitionCondition>();

            conditions.Add(new FiniteStateMachineTransitionCondition()
            {
                EventType = event_type,
                CheckDelegate = check_delegate
            });
            FiniteStateMachineTransition trans = new FiniteStateMachineTransition()
            {
                SourceState = states[start],
                TargetState = states[end],
                TransitionConditions = conditions,
                Inverted = inverted
            };
            states[start].Outgoing.Add(trans);
            return trans;
        }

        /// <summary>
        /// Generates a transition.
        /// </summary>
        /// <param name="states"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="inverted"></param>
        /// <param name="event_types"></param>
        /// <returns></returns>
        public static FiniteStateMachineTransition Generate(List<FiniteStateMachineState> states, int start, int end, bool inverted, params Type[] event_types)
        {
            List<FiniteStateMachineTransitionCondition> conditions = new List<FiniteStateMachineTransitionCondition>();
            foreach (Type type in event_types)
            {
                conditions.Add(new FiniteStateMachineTransitionCondition()
                {
                    EventType = type
                });
            }

            FiniteStateMachineTransition trans = new FiniteStateMachineTransition()
            {
                SourceState = states[start],
                TargetState = states[end],
                TransitionConditions = conditions,
                Inverted = inverted
            };
            states[start].Outgoing.Add(trans);
            return trans;
        }

        #endregion
    }
}
