using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Instructions.MicroPlanning
{
    internal class MicroPlannerException : Exception
    {
        /// <summary>
        /// Creates a new microplanner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messages_stack"></param>
        public MicroPlannerException(string message, List<MicroPlannerMessage> messages_stack)
            :base(message)
        {
            this.MessagesStack = messages_stack;
        }

        public List<MicroPlannerMessage> MessagesStack { get; private set; }
    }
}
