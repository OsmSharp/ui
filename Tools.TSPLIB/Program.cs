using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.TSPLIB.Problems;
using System.IO;

namespace Tools.TSPLIB
{
    class Program
    {
        static void Main(string[] args)
        {
            string file_name = "ft70.atsp";
            TSPLIBProblem problem = 
                TSPLIB.Parser.TSPLIBProblemParser.ParseFrom(file_name);
            TSPLIBProblem atsp = Convertor.ATSP_TSP.ATSP_TSPConvertor.Convert(problem);
            Parser.TSPLIBProblemGenerator.Generate(new FileInfo("output.tsp"),
                atsp);
        }
    }
}
