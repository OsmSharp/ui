using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.StateMachines
{
    public class FiniteStateMachineTransitionCondition
    {
        public bool Check(object even)
        {
            if (this.EventType.Equals(even.GetType()))
            {
                if (this.CheckDelegate != null)
                {
                    return this.CheckDelegate(even);
                }
                return true;
            }
            return false;
        }

        public Type EventType { get; set; }

        public delegate bool FiniteStateMachineTransitionConditionDelegate(object even);

        public FiniteStateMachineTransitionConditionDelegate CheckDelegate { get; set; }
    }
}
