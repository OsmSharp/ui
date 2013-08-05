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

namespace OsmSharp.Collections.LongIndex.LongIndex
{
    internal class LongIndexLeaf : ILongIndexNode
    {
        public long CalculateDigit(long number)
        {
            const long baseNumberPlus = 10;
            const long baseNumber = 1;
            return (number - ((number / baseNumberPlus) * baseNumberPlus)) 
                / baseNumber;
        }

        public bool Contains(long number)
        {
            switch (this.CalculateDigit(number))
            {
                case 0:
                    return this.Has0;
                case 1:
                    return this.Has1;
                case 2:
                    return this.Has2;
                case 3:
                    return this.Has3;
                case 4:
                    return this.Has4;
                case 5:
                    return this.Has5;
                case 6:
                    return this.Has6;
                case 7:
                    return this.Has7;
                case 8:
                    return this.Has8;
                case 9:
                    return this.Has9;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Add(long number)
        {
            switch (this.CalculateDigit(number))
            {
                case 0:
                    this.Has0 = true;
                    break;
                case 1:
                    this.Has1 = true;
                    break;
                case 2:
                    this.Has2 = true;
                    break;
                case 3:
                    this.Has3 = true;
                    break;
                case 4:
                    this.Has4 = true;
                    break;
                case 5:
                    this.Has5 = true;
                    break;
                case 6:
                    this.Has6 = true;
                    break;
                case 7:
                    this.Has7 = true;
                    break;
                case 8:
                    this.Has8 = true;
                    break;
                case 9:
                    this.Has9 = true;
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
                    this.Has0 = false;
                    break;
                case 1:
                    this.Has1 = false;
                    break;
                case 2:
                    this.Has2 = false;
                    break;
                case 3:
                    this.Has3 = false;
                    break;
                case 4:
                    this.Has4 = false;
                    break;
                case 5:
                    this.Has5 = false;
                    break;
                case 6:
                    this.Has6 = false;
                    break;
                case 7:
                    this.Has7 = false;
                    break;
                case 8:
                    this.Has8 = false;
                    break;
                case 9:
                    this.Has9 = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool Has0 { get; set; }
        public bool Has1 { get; set; }
        public bool Has2 { get; set; }
        public bool Has3 { get; set; }
        public bool Has4 { get; set; }
        public bool Has5 { get; set; }
        public bool Has6 { get; set; }
        public bool Has7 { get; set; }
        public bool Has8 { get; set; }
        public bool Has9 { get; set; }

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