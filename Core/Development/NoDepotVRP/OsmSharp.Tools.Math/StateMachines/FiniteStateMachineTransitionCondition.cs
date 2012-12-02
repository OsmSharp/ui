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
using OsmSharp.Tools.Math.Automata;

namespace OsmSharp.Tools.Math.StateMachines
{
    public class FiniteStateMachineTransitionCondition<EventType>
    {
        public bool Check(FiniteStateMachine<EventType> machine, object even)
        {
            if (this.EventTypeObject.Equals(even.GetType()))
            {
                if (this.CheckDelegate != null)
                {
                    return this.CheckDelegate(machine, even);
                }
                return true;
            }
            return false;
        }

        public Type EventTypeObject { get; set; }

        public delegate bool FiniteStateMachineTransitionConditionDelegate(FiniteStateMachine<EventType> machine, object even);

        public FiniteStateMachineTransitionConditionDelegate CheckDelegate { get; set; }
    }
}
