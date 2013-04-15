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
//using OsmSharp.Tools.Math.VRP.MultiSalesman.Problems;
using OsmSharp.Tools.Math.Units.Time;
using OsmSharp.Tools.Math.TSP.Problems;
using OsmSharp.Tools.Math.VRP.Core;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic
{
    ///// <summary>
    ///// The problem description for the genetic algorithm.
    ///// </summary>
    //internal class Problem : IProblemWeights, OsmSharp.Tools.Math.AI.Genetic.IProblem
    //{
    //    private IProblemWeights _weights;

    //    private float[] _placement_solutions;

    //    public Problem(Second max, IProblemWeights weights, float[] placement_solutions)
    //    {
    //        this.Max = max;

    //        _weights = weights;
    //        _placement_solutions = placement_solutions;
    //    }

    //    public float[] PlacementSolutions
    //    {
    //        get
    //        {
    //            return _placement_solutions;
    //        }
    //    }

    //    public Second Max { get; private set; }

    //    public int Size
    //    {
    //        get
    //        {
    //            return _weights.Size;
    //        }
    //    }

    //    public IProblemWeights Weights
    //    {
    //        get
    //        {
    //            return _weights;
    //        }
    //    }

    //    public float Weight(int from, int to)
    //    {
    //        return _weights.Weight(from, to);
    //    }

    //    public bool Symmetric
    //    {
    //        get
    //        {
    //            return false;
    //        }
    //    }

    //    public bool Euclidean
    //    {
    //        get 
    //        {
    //            return false;
    //        }
    //    }

    //    public float[][] WeightMatrix
    //    {
    //        get 
    //        { 
    //            return _weights.WeightMatrix; 
    //        }
    //    }
    //}
}
