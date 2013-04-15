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
using OsmSharp.Tools.Math.TSP;
using OsmSharp.Tools.TSPLIB.Problems;
using OsmSharp.Tools.Math.TSP.Problems;

namespace OsmSharp.Tools.TSPLIB.Convertor.ATSP_TSP
{
    /// <summary>
    /// ATSP-TSP convertor.
    /// </summary>
    public static class ATSP_TSPConvertor
    {
        /// <summary>
        /// Converts a problem.
        /// </summary>
        /// <param name="atsp"></param>
        /// <param name="name"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static TSPLIBProblem Convert(IProblem atsp, string name, string comment)
        {
            // convert the problem to a symetric one.
            IProblem symetric = atsp.ConvertToSymmetric();

            return new TSPLIBProblem(name, comment, symetric.Size, symetric.WeightMatrix,
                TSPLIBProblemWeightTypeEnum.Explicit, TSPLIBProblemTypeEnum.TSP, 0, 0);
        }

        /// <summary>
        /// Converts problem.
        /// </summary>
        /// <param name="atsp"></param>
        /// <returns></returns>
        public static TSPLIBProblem Convert(TSPLIBProblem atsp)
        {
            // check if the problem is not already symmetric.
            if (atsp.Symmetric)
            {
                return atsp;
            }
            
            // the problem is not symmetric, convert it.
            string name = atsp.Name + "(SYM)";
            string comment = atsp.Comment;

            // convert the problem to a symetric one.
            IProblem symetric = atsp.ConvertToSymmetric();

            return new TSPLIBProblem(name, comment, symetric.Size, symetric.WeightMatrix,
                TSPLIBProblemWeightTypeEnum.Explicit, TSPLIBProblemTypeEnum.TSP, 0, 0);
        }
    }
}
