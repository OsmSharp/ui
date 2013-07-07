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

namespace OsmSharp.Osm.Data.Streams.Filters.LongIndex
{
    internal class LongIndexNode : ILongIndexNode
    {
        private readonly short _baseNumber;

        public LongIndexNode(short baseNumber)
        {
            _baseNumber = baseNumber;
        }

        public static ILongIndexNode CreateForBase(short baseNumber)
        {
            if (baseNumber == 0)
            {
                return new LongIndexLeaf();
            }
            else
            {
                return new LongIndexNode(baseNumber);
            }
        }

        public static long CalculateBaseNumber(short baseNumber)
        {
            switch (baseNumber)
            { // for performance reasons
                case 0:
                    return 1;
                case 1:
                    return 10;
                case 2:
                    return 100;
                case 3:
                    return 1000;
                case 4:
                    return 10000;
                case 5:
                    return 100000;
                case 6:
                    return 1000000;
                case 7:
                    return 10000000;
                case 8:
                    return 100000000;
                case 9:
                    return 1000000000;
                case 10:
                    return 10000000000;
                default:
                    return (long)System.Math.Pow(10, baseNumber); 
            }
        }

        public ILongIndexNode Has0 { get; set; }
        public ILongIndexNode Has1 { get; set; }
        public ILongIndexNode Has2 { get; set; }
        public ILongIndexNode Has3 { get; set; }
        public ILongIndexNode Has4 { get; set; }
        public ILongIndexNode Has5 { get; set; }
        public ILongIndexNode Has6 { get; set; }
        public ILongIndexNode Has7 { get; set; }
        public ILongIndexNode Has8 { get; set; }
        public ILongIndexNode Has9 { get; set; }

        public long CalculateDigit(long number)
        {
            long baseNumberPlus = LongIndexNode.CalculateBaseNumber((short)(this.Base + 1));
            long baseNumber = LongIndexNode.CalculateBaseNumber(this.Base);
            return (number - ((number / baseNumberPlus) * baseNumberPlus)) / baseNumber;
        }

        #region ILongIndexNode Members

        public bool Contains(long number)
        {
            switch (this.CalculateDigit(number))
            {
                case 0:
                    if (this.Has0 == null)
                    {
                        return false;
                    }
                    return this.Has0.Contains(number);
                case 1:
                    if (this.Has1 == null)
                    {
                        return false;
                    }
                    return this.Has1.Contains(number);
                case 2:
                    if (this.Has2 == null)
                    {
                        return false;
                    }
                    return this.Has2.Contains(number);
                case 3:
                    if (this.Has3 == null)
                    {
                        return false;
                    }
                    return this.Has3.Contains(number);
                case 4:
                    if (this.Has4 == null)
                    {
                        return false;
                    }
                    return this.Has4.Contains(number);
                case 5:
                    if (this.Has5 == null)
                    {
                        return false;
                    }
                    return this.Has5.Contains(number);
                case 6:
                    if (this.Has6 == null)
                    {
                        return false;
                    }
                    return this.Has6.Contains(number);
                case 7:
                    if (this.Has7 == null)
                    {
                        return false;
                    }
                    return this.Has7.Contains(number);
                case 8:
                    if (this.Has8 == null)
                    {
                        return false;
                    }
                    return this.Has8.Contains(number);
                case 9:
                    if (this.Has9 == null)
                    {
                        return false;
                    }
                    return this.Has9.Contains(number);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Add(long number)
        {

            switch (this.CalculateDigit(number))
            {
                case 0:
                    if (this.Has0 == null)
                    {
                        this.Has0 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.Has0.Add(number);
                    break;
                case 1:
                    if (this.Has1 == null)
                    {
                        this.Has1 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.Has1.Add(number);
                    break;
                case 2:
                    if (this.Has2 == null)
                    {
                        this.Has2 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.Has2.Add(number);
                    break;
                case 3:
                    if (this.Has3 == null)
                    {
                        this.Has3 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.Has3.Add(number);
                    break;
                case 4:
                    if (this.Has4 == null)
                    {
                        this.Has4 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.Has4.Add(number);
                    break;
                case 5:
                    if (this.Has5 == null)
                    {
                        this.Has5 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.Has5.Add(number);
                    break;
                case 6:
                    if (this.Has6 == null)
                    {
                        this.Has6 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.Has6.Add(number);
                    break;
                case 7:
                    if (this.Has7 == null)
                    {
                        this.Has7 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.Has7.Add(number);
                    break;
                case 8:
                    if (this.Has8 == null)
                    {
                        this.Has8 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.Has8.Add(number);
                    break;
                case 9:
                    if (this.Has9 == null)
                    {
                        this.Has9 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.Has9.Add(number);
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
                    if (this.Has0 != null)
                    {
                        this.Has0.Remove(number);
                    }
                    break;
                case 1:
                    if (this.Has1 != null)
                    {
                        this.Has1.Remove(number);
                    }
                    break;
                case 2:
                    if (this.Has2 != null)
                    {
                        this.Has2.Remove(number);
                    }
                    break;
                case 3:
                    if (this.Has3 != null)
                    {
                        this.Has3.Remove(number);
                    }
                    break;
                case 4:
                    if (this.Has4 != null)
                    {
                        this.Has4.Remove(number);
                    }
                    break;
                case 5:
                    if (this.Has5 != null)
                    {
                        this.Has5.Remove(number);
                    }
                    break;
                case 6:
                    if (this.Has6 != null)
                    {
                        this.Has6.Remove(number);
                    }
                    break;
                case 7:
                    if (this.Has7 != null)
                    {
                        this.Has7.Remove(number);
                    }
                    break;
                case 8:
                    if (this.Has8 != null)
                    {
                        this.Has8.Remove(number);
                    }
                    break;
                case 9:
                    if (this.Has9 != null)
                    {
                        this.Has9.Remove(number);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public short Base
        {
            get                 
            {
                return _baseNumber;
            }
        }

        #endregion
    }
}