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
    internal class LongIndexNode : ILongIndexNode
    {
        private short _base_number;

        public LongIndexNode(short base_number)
        {
            _base_number = base_number;
        }

        public static ILongIndexNode CreateForBase(short base_number)
        {
            if (base_number == 0)
            {
                return new LongIndexLeaf();
            }
            else
            {
                return new LongIndexNode(base_number);
            }
        }

        public static long CalculateBaseNumber(short base_number)
        {
            switch (base_number)
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
                    return (long)System.Math.Pow(10, base_number); 
            }
        }

        public ILongIndexNode has_0 { get; set; }
        public ILongIndexNode has_1 { get; set; }
        public ILongIndexNode has_2 { get; set; }
        public ILongIndexNode has_3 { get; set; }
        public ILongIndexNode has_4 { get; set; }
        public ILongIndexNode has_5 { get; set; }
        public ILongIndexNode has_6 { get; set; }
        public ILongIndexNode has_7 { get; set; }
        public ILongIndexNode has_8 { get; set; }
        public ILongIndexNode has_9 { get; set; }

        public long CalculateDigit(long number)
        {
            long base_number_plus = LongIndexNode.CalculateBaseNumber((short)(this.Base + 1));
            long base_number = LongIndexNode.CalculateBaseNumber(this.Base);
            return (number - ((number / base_number_plus) * base_number_plus)) / base_number;
        }

        #region ILongIndexNode Members

        public bool Contains(long number)
        {
            switch (this.CalculateDigit(number))
            {
                case 0:
                    if (this.has_0 == null)
                    {
                        return false;
                    }
                    return this.has_0.Contains(number);
                case 1:
                    if (this.has_1 == null)
                    {
                        return false;
                    }
                    return this.has_1.Contains(number);
                case 2:
                    if (this.has_2 == null)
                    {
                        return false;
                    }
                    return this.has_2.Contains(number);
                case 3:
                    if (this.has_3 == null)
                    {
                        return false;
                    }
                    return this.has_3.Contains(number);
                case 4:
                    if (this.has_4 == null)
                    {
                        return false;
                    }
                    return this.has_4.Contains(number);
                case 5:
                    if (this.has_5 == null)
                    {
                        return false;
                    }
                    return this.has_5.Contains(number);
                case 6:
                    if (this.has_6 == null)
                    {
                        return false;
                    }
                    return this.has_6.Contains(number);
                case 7:
                    if (this.has_7 == null)
                    {
                        return false;
                    }
                    return this.has_7.Contains(number);
                case 8:
                    if (this.has_8 == null)
                    {
                        return false;
                    }
                    return this.has_8.Contains(number);
                case 9:
                    if (this.has_9 == null)
                    {
                        return false;
                    }
                    return this.has_9.Contains(number);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Add(long number)
        {

            switch (this.CalculateDigit(number))
            {
                case 0:
                    if (this.has_0 == null)
                    {
                        this.has_0 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.has_0.Add(number);
                    break;
                case 1:
                    if (this.has_1 == null)
                    {
                        this.has_1 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.has_1.Add(number);
                    break;
                case 2:
                    if (this.has_2 == null)
                    {
                        this.has_2 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.has_2.Add(number);
                    break;
                case 3:
                    if (this.has_3 == null)
                    {
                        this.has_3 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.has_3.Add(number);
                    break;
                case 4:
                    if (this.has_4 == null)
                    {
                        this.has_4 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.has_4.Add(number);
                    break;
                case 5:
                    if (this.has_5 == null)
                    {
                        this.has_5 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.has_5.Add(number);
                    break;
                case 6:
                    if (this.has_6 == null)
                    {
                        this.has_6 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.has_6.Add(number);
                    break;
                case 7:
                    if (this.has_7 == null)
                    {
                        this.has_7 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.has_7.Add(number);
                    break;
                case 8:
                    if (this.has_8 == null)
                    {
                        this.has_8 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.has_8.Add(number);
                    break;
                case 9:
                    if (this.has_9 == null)
                    {
                        this.has_9 = LongIndexNode.CreateForBase((short)(this.Base - 1));
                    }
                    this.has_9.Add(number);
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
                    if (this.has_0 != null)
                    {
                        this.has_0.Remove(number);
                    }
                    break;
                case 1:
                    if (this.has_1 != null)
                    {
                        this.has_1.Remove(number);
                    }
                    break;
                case 2:
                    if (this.has_2 != null)
                    {
                        this.has_2.Remove(number);
                    }
                    break;
                case 3:
                    if (this.has_3 != null)
                    {
                        this.has_3.Remove(number);
                    }
                    break;
                case 4:
                    if (this.has_4 != null)
                    {
                        this.has_4.Remove(number);
                    }
                    break;
                case 5:
                    if (this.has_5 != null)
                    {
                        this.has_5.Remove(number);
                    }
                    break;
                case 6:
                    if (this.has_6 != null)
                    {
                        this.has_6.Remove(number);
                    }
                    break;
                case 7:
                    if (this.has_7 != null)
                    {
                        this.has_7.Remove(number);
                    }
                    break;
                case 8:
                    if (this.has_8 != null)
                    {
                        this.has_8.Remove(number);
                    }
                    break;
                case 9:
                    if (this.has_9 != null)
                    {
                        this.has_9.Remove(number);
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
                return _base_number;
            }
        }

        #endregion
    }
}
