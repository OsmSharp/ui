using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.StateMachines
{
    /// <summary>
    /// Represents a state in a finite-state machine.
    /// </summary>
    public sealed class FiniteStateMachineState
    {
        /// <summary>
        /// The unique id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The list of possible outgoing transition.
        /// </summary>
        public IList<FiniteStateMachineTransition> Outgoing { get; private set; }

        /// <summary>
        /// Boolean representing if the state is final.
        /// </summary>
        public bool Final { get; set; }

        /// <summary>
        /// Boolean representing if this state consumes event even if there is no outgoing transition.
        /// </summary>
        public bool ConsumeAll { get; set; }

        /// <summary>
        /// Returns a description of this state.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("State[{0}]: Final: {1}",
                this.Id,
                this.Final);
        }

        #region Generate States

        /// <summary>
        /// Generates an amount of states.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<FiniteStateMachineState> Generate(int count)
        {
            List<FiniteStateMachineState> states = new List<FiniteStateMachineState>();
            for (int idx = 0; idx < count; idx++)
            {
                states.Add(new FiniteStateMachineState()
                {
                    Id = idx,
                    Outgoing = new List<FiniteStateMachineTransition>(),
                    Final = false,
                    ConsumeAll = false
                });
            }
            return states;
        }

        #endregion
    }
}
