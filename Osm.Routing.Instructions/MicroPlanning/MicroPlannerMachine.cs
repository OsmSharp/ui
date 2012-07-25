using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Automata;
using Tools.Math.StateMachines;

namespace Osm.Routing.Instructions.MicroPlanning
{
    internal abstract class MicroPlannerMachine : FiniteStateMachine<MicroPlannerMessage>
    {
        /// <summary>
        /// The contains the microplanner to report back to.
        /// </summary>
        private MicroPlanner _planner;

        /// <summary>
        /// The priority.
        /// </summary>
        private int _priority;

        /// <summary>
        /// Creates a new event machine.
        /// </summary>
        /// <param name="consumer"></param>
        protected MicroPlannerMachine(FiniteStateMachineState initial, MicroPlanner planner, int priority)
            :base(initial)
        {
            _planner = planner;
            _priority = priority;
        }

        /// <summary>
        /// Returns the microplanner.
        /// </summary>
        protected MicroPlanner Planner
        {
            get
            {
                return _planner;
            }
        }

        /// <summary>
        /// Returns the priority.
        /// </summary>
        public int Priority
        {
            get
            {
                return _priority;
            }
        }


        private IList<MicroPlannerMessage> _messages;

        public IList<MicroPlannerMessage> FinalMessages
        {
            get
            {
                return _messages;
            }
        }

        /// <summary>
        /// Called when this machine is succesfull.
        /// </summary>
        public abstract void Succes();

        /// <summary>
        /// Called when a final state is reached.
        /// </summary>
        /// <param name="messages"></param>
        protected override void RaiseFinalStateEvent(IList<MicroPlannerMessage> messages)
        {
            _messages = new List<MicroPlannerMessage>(messages);

            this.Planner.ReportFinal(this, messages);
        }

        /// <summary>
        /// Called when a reset event occured.
        /// </summary>
        /// <param name="even"></param>
        /// <param name="state"></param>
        protected override void RaiseResetEvent(MicroPlannerMessage even, FiniteStateMachineState state)
        {
            this.Planner.ReportReset(this);
        }
    }
}
