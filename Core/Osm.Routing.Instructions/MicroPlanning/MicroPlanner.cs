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
using Osm.Routing.Instructions.MicroPlanning.Machines;
using Osm.Routing.Instructions.LanguageGeneration;
using Osm.Routing.Core.ArcAggregation.Output;

namespace Osm.Routing.Instructions.MicroPlanning
{
    /// <summary>
    /// Plans aggregated messages into instructions.
    /// </summary>
    internal class MicroPlanner
    {
        /// <summary>
        /// Creates a new planner.
        /// </summary>
        public MicroPlanner(ILanguageGenerator language_generator)
        {
            this.InitializeMachines();

            this.InitializeMessagesStack();

            this.SentencePlanner = new SentencePlanner(language_generator);
        }

        /// <summary>
        /// The scentence planner for this micro planner.
        /// </summary>
        internal SentencePlanner SentencePlanner
        {
            get;
            private set;
        }

        /// <summary>
        /// Holds the current object from the aggregated stream.
        /// </summary>
        private Aggregated _current;

        /// <summary>
        /// Plans all the messages in the aggregated 
        /// </summary>
        /// <param name="p"></param>
        public List<Instruction> Plan(AggregatedPoint p)
        {
            // set the current aggregated object.
            _current = p;

            // loop until the current object is null.
            while (_current != null)
            {
                while (_current != null)
                {
                    if (_current is AggregatedPoint)
                    {
                        if ((_current as AggregatedPoint).Location.Latitude == 51.254875183105469)
                        {
                            Console.WriteLine("BINGO!");
                        }
                    }
                    // plan the current message.
                    this.PlanNewMessage(_current);

                    // get the next object.
                    _current = _current.GetNext();
                }

                // show the latest success anyway.
                if (_latest_final >= 0)
                { // do the latest succes.
                    this.Success(_latest_machine);

                    // get the next object.
                    if (_current != null)
                    {
                        _current = _current.GetNext();
                    }
                }
                else if(_messages_stack.Count > 0)
                { // no machine matches everything until the end of the route.
                    throw new MicroPlannerException("No machine could be found matching the current stack of messages!", _messages_stack);
                }
            }

            // return the instructions list accumulated in the scentence planner.
            return this.SentencePlanner.Instructions;
        }

        /// <summary>
        /// Creates and plans a new message.
        /// </summary>
        /// <param name="aggregated"></param>
        private void PlanNewMessage(Aggregated aggregated)
        {
            // create the message.
            MicroPlannerMessage message = null;
            if (aggregated is AggregatedPoint)
            {
                MicroPlannerMessagePoint point = new MicroPlannerMessagePoint();
                point.Point = aggregated as AggregatedPoint;

                message = point;
            }
            else if (aggregated is AggregatedArc)
            {
                MicroPlannerMessageArc arc = new MicroPlannerMessageArc();
                arc.Arc = aggregated as AggregatedArc;

                message = arc;
            }

            // plan the message.
            this.Plan(message);
        }

        #region Machines 
        
        /// <summary>
        /// Keeps a list of microplanners.
        /// </summary>
        private List<MicroPlannerMachine> _machines;

        /// <summary>
        /// Initializes the list of machines.
        /// </summary>
        private void InitializeMachines()
        {
            _machines = new List<MicroPlannerMachine>();
            _machines.Add(new TurnMachine(this));
            _machines.Add(new PoiMachine(this));
            _machines.Add(new PoiWithTurnMachine(this));
            _machines.Add(new ImmidateTurnMachine(this));
            _machines.Add(new RoundaboutMachine(this));
        }

        #endregion

        #region Planning Queue

        /// <summary>
        /// Holds the current messages stack.
        /// </summary>
        private List<MicroPlannerMessage> _messages_stack;

        /// <summary>
        /// Holds the current list of invalid machines.
        /// </summary>
        private List<MicroPlannerMachine> _invalid_machines;
        
        /// <summary>
        /// Holds the current list of machines that reached a final machine.
        /// </summary>
        private List<MicroPlannerMachine> _valid_machines;

        /// <summary>
        /// Holds the position of the latest final.
        /// </summary>
        private int _latest_final;

        /// <summary>
        /// Holds the machine that finaled latest.
        /// </summary>
        private MicroPlannerMachine _latest_machine;

        /// <summary>
        /// Initializes the messages stack.
        /// </summary>
        private void InitializeMessagesStack()
        {
            this.ResetMessagesStack(true);
        }

        /// <summary>
        /// Resets the messages stack.
        /// </summary>
        private void ResetMessagesStack(bool reset_errors)
        {
            _invalid_machines = new List<MicroPlannerMachine>();
            _valid_machines = new List<MicroPlannerMachine>();
            _messages_stack = new List<MicroPlannerMessage>();
            _latest_final = -1;
            _latest_machine = null;
        }

        /// <summary>
        /// Boolean holding planning succes flag.
        /// </summary>
        private bool _succes = false;

        /// <summary>
        /// Boolean holding planning error flag.
        /// </summary>
        private bool _error = false;

        /// <summary>
        /// Plan the given message.
        /// </summary>
        /// <param name="message"></param>
        private void Plan(MicroPlannerMessage message)
        {
            // add the message to the stack.
            _messages_stack.Add(message);

            // put the message through the machine.
            foreach (MicroPlannerMachine machine in _machines)
            {
                if (!_invalid_machines.Contains(machine)
                    && !_valid_machines.Contains(machine))
                { // only use machines that are still valid!
                    machine.Consume(message);

                    if (_succes)
                    {
                        break;
                    }

                    if (_error)
                    {
                        break;
                    }
                }
            }
            _succes = false;
            _error = false;
        }

        /// <summary>
        /// The given machine was successfull.
        /// </summary>
        /// <param name="machine"></param>
        internal void Success(MicroPlannerMachine machine)
        {
            // reset the current point/arc.
            if (_messages_stack.Count > _latest_final + 1)
            {
                MicroPlannerMessage message = _messages_stack[_latest_final];
                if (message is MicroPlannerMessageArc)
                {
                    _current = (message as MicroPlannerMessageArc).Arc;
                }                
                if (message is MicroPlannerMessagePoint)
                {
                    _current = (message as MicroPlannerMessagePoint).Point;
                }
            }

            // reset the mesages stack.
            this.ResetMessagesStack(true);

            // tell the machine again it was successfull.
            machine.Succes();

            // re-initialize the machines.
            this.InitializeMachines();

            _succes = true;
        }

        /// <summary>
        /// Checks the machine for success.
        /// </summary>
        internal void CheckMachine(MicroPlannerMachine machine)
        {
            // check the other machines and their priorities.
            int priority = machine.Priority;
            foreach (MicroPlannerMachine other_machine in _machines)
            {
                if (!_invalid_machines.Contains(other_machine))
                {
                    if (other_machine.Priority > priority)
                    { // not sure this machine's final state is actually the final state.
                        return;
                    }
                }
            }

            // no other machines exist with higher priority.
            this.Success(machine);
        }

        /// <summary>
        /// Reports a final state to this microplanner when some machine reaches it.
        /// </summary>
        /// <param name="microPlannerMachine"></param>
        /// <param name="messages"></param>
        internal void ReportFinal(MicroPlannerMachine machine, IList<MicroPlannerMessage> messages)
        {
            if (_latest_final == _messages_stack.Count - 1)
            { // check if a machine with the same match length has higher priority.
                if (_latest_machine.Priority >= machine.Priority)
                { // the current machine has the same match length and has higher or the same priority.
                    return;
                }
            }

            // update the latest final value.
            _latest_final = _messages_stack.Count - 1;
            _latest_machine = machine;

            // add the machine to the valid machines.
            _valid_machines.Add(machine);

            // check and see if all other machines with higher priority are invalid.
            this.CheckMachine(machine);
        }

        /// <summary>
        /// Reports when a machine resets (meaning it reached an invalid state).
        /// </summary>
        /// <param name="microPlannerMachine"></param>
        internal void ReportReset(MicroPlannerMachine machine)
        {
            // the machine cannot be used anymore until a reset occurs.
            _invalid_machines.Add(machine);

            // check if the latest machine is now successfull.
            if (_latest_machine != null)
            {
                this.CheckMachine(_latest_machine);
            }

            // check to see if not all machine are invalid! 
            if (_invalid_machines.Count == _machines.Count)
            {
                if(_latest_machine == null)
                { // all machine went in error!
                    throw new MicroPlannerException("No machine could be found matching the current stack of messages!", _messages_stack);
                }
                else
                { // start all over with the current stack of messages.
                    this.Success(machine);
                }
            }
        }

        #endregion
    }
}
