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
