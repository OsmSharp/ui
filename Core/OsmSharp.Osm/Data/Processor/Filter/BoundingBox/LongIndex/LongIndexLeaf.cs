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

namespace OsmSharp.Osm.Data.Core.Processor.Filter.LongIndex
{
    internal class LongIndexLeaf : ILongIndexNode
    {
        public long CalculateDigit(long number)
        {
            long base_number_plus = 10;
            long base_number =1;
            return (number - ((number / base_number_plus) * base_number_plus)) / base_number;
        }

        public bool Contains(long number)
        {
            switch (this.CalculateDigit(number))
            {
                case 0:
                    return this.has_0;
                case 1:
                    return this.has_1;
                case 2:
                    return this.has_2;
                case 3:
                    return this.has_3;
                case 4:
                    return this.has_4;
                case 5:
                    return this.has_5;
                case 6:
                    return this.has_6;
                case 7:
                    return this.has_7;
                case 8:
                    return this.has_8;
                case 9:
                    return this.has_9;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Add(long number)
        {
            switch (this.CalculateDigit(number))
            {
                case 0:
                    this.has_0 = true;
                    break;
                case 1:
                    this.has_1 = true;
                    break;
                case 2:
                    this.has_2 = true;
                    break;
                case 3:
                    this.has_3 = true;
                    break;
                case 4:
                    this.has_4 = true;
                    break;
                case 5:
                    this.has_5 = true;
                    break;
                case 6:
                    this.has_6 = true;
                    break;
                case 7:
                    this.has_7 = true;
                    break;
                case 8:
                    this.has_8 = true;
                    break;
                case 9:
                    this.has_9 = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void Remove(long number)
        {
            switch (this.CalculateDigit(number))
            {
                case 0:
                    this.has_0 = false;
                    break;
                case 1:
                    this.has_1 = false;
                    break;
                case 2:
                    this.has_2 = false;
                    break;
                case 3:
                    this.has_3 = false;
                    break;
                case 4:
                    this.has_4 = false;
                    break;
                case 5:
                    this.has_5 = false;
                    break;
                case 6:
                    this.has_6 = false;
                    break;
                case 7:
                    this.has_7 = false;
                    break;
                case 8:
                    this.has_8 = false;
                    break;
                case 9:
                    this.has_9 = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool has_0 { get; set; }
        public bool has_1 { get; set; }
        public bool has_2 { get; set; }
        public bool has_3 { get; set; }
        public bool has_4 { get; set; }
        public bool has_5 { get; set; }
        public bool has_6 { get; set; }
        public bool has_7 { get; set; }
        public bool has_8 { get; set; }
        public bool has_9 { get; set; }

        public short Base
        {
            get 
            { 
                return 0; 
            }
        }

        #region ILongIndexNode Members


        public long CalculateBaseNumber()
        {
            return 1;
        }

        #endregion
    }
}
