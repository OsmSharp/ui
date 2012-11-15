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

namespace OsmSharp.Tools.Math.Units.Weight
{
    public class Gram : Unit
    {        
        public Gram()
            : base(0.0d)
        {

        }

        private Gram(double value)
            : base(value)
        {

        }

        #region Conversions

        public static implicit operator Gram(double value)
        {
            return new Gram(value);
        }

        public static implicit operator Gram(Kilogram kilogram)
        {
            return kilogram.Value * 1000d;
        }

        #endregion


        public override string ToString()
        {
            return this.Value.ToString() + "g";
        }
        
    }
}
