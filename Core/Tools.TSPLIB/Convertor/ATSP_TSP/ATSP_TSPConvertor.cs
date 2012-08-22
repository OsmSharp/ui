using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.TSP;
using Tools.TSPLIB.Problems;
using Tools.Math.TSP.Problems;

namespace Tools.TSPLIB.Convertor.ATSP_TSP
{
    public static class ATSP_TSPConvertor
    {
        public static TSPLIBProblem Convert(IProblem atsp, string name, string comment)
        {
            // convert the problem to a symetric one.
            IProblem symetric = atsp.ConvertToSymmetric();

            return new TSPLIBProblem(name, comment, symetric.Size, symetric.WeightMatrix,
                TSPLIBProblemWeightTypeEnum.Explicit, TSPLIBProblemTypeEnum.TSP);
        }

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
                TSPLIBProblemWeightTypeEnum.Explicit, TSPLIBProblemTypeEnum.TSP);
        }
    }
}
