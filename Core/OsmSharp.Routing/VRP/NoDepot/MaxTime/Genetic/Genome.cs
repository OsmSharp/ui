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
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic
{
    //internal class Genome : IEquatable<Genome>, ICloneable
    //{
    //    public int[] Sizes { get; set; }

    //    public int[] Customers { get; set; }

    //    public int Vehicles
    //    {
    //        get
    //        {
    //            return this.Sizes.Length;
    //        }
    //    }

    //    public int StartOf(int route_idx)
    //    {
    //        int start = 0;
    //        for (int idx = 0; idx < route_idx; idx++)
    //        {
    //            start = start + this.Sizes[idx];
    //        }
    //        return start;
    //    }

    //    public IRoute Route(int route_idx)
    //    {
    //        int start = this.StartOf(route_idx);
    //        int stop = -1;
    //        if (route_idx == this.Sizes.Length - 1)
    //        {
    //            stop = this.Customers.Length;
    //        }
    //        else
    //        {
    //            stop = this.StartOf(route_idx + 1);
    //        }
    //        List<int> part_of_orginal = new List<int>(stop - start + 1);
    //        //int idx = 0;
    //        for (int i = start; i < stop; i++)
    //        {
    //            int customer = this.Customers[i];
    //            part_of_orginal.Add(customer);
    //        }
    //        //foreach (int customer in this.Customers)
    //        //{
    //        //    if (idx >= start && idx < stop)
    //        //    {
    //        //        part_of_orginal.Add(customer);
    //        //    }
    //        //    idx++;
    //        //}
    //        return new SimpleAsymmetricRoute(part_of_orginal, true);
    //    }

    //    public static Genome CreateFrom(DynamicAsymmetricMultiRoute route)
    //    {
    //        Genome genome = new Genome();
    //        List<int> customers = new List<int>();
    //        List<int> sizes = new List<int>();
    //        for (int idx = 0; idx < route.Count; idx++)
    //        {
    //            int customer_idx = 0;
    //            foreach (int customer in route.Route(idx))
    //            {
    //                customer_idx++;
    //                customers.Add(customer);
    //            }

    //            sizes.Add(customer_idx);
    //        }

    //        genome.Customers = customers.ToArray();
    //        genome.Sizes = sizes.ToArray();

    //        return genome;
    //    }

    //    public bool Equals(Genome other)
    //    {
    //        if (this.Sizes.Length == other.Sizes.Length &&
    //            this.Customers.Length == other.Customers.Length)
    //        {
    //            for(int idx = 0; idx < this.Sizes.Length; idx++)
    //            {
    //                if(this.Sizes[idx] != other.Sizes[idx])                    
    //                {
    //                    return false;
    //                }
    //            }
    //            for (int idx = 0; idx < this.Customers.Length; idx++)
    //            {
    //                if (this.Customers[idx] != other.Customers[idx])
    //                {
    //                    return false;
    //                }
    //            }
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// Returns true if this route is valid.
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool IsValid()
    //    {
    //        //HashSet<int> unique_customers = new HashSet<int>(this.Customers);
    //        //unique_customers.Remove(-1);
    //        //int count = 0;
    //        //foreach (int customer in this.Customers)
    //        //{
    //        //    if (customer >= 0)
    //        //    {
    //        //        count++;
    //        //    }
    //        //}
    //        //if (unique_customers.Count != count)
    //        //{
    //        //    return false;
    //        //}
    //        //int total_size = 0;
    //        //foreach (int size in this.Sizes)
    //        //{
    //        //    total_size = total_size + size;
    //        //}
    //        //return total_size == unique_customers.Count;
    //        return true;
    //    }

    //    public object Clone()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string ToString()
    //    {
    //        StringBuilder builder = new StringBuilder();
    //        foreach (int size in Sizes)
    //        {
    //            builder.Append(size);
    //            builder.Append(" ");
    //        }
    //        return builder.ToString();
    //    }
    //}
}
