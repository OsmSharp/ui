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
using System.IO;
using System.Linq;
using System.Reflection;
using OsmSharp.Tools.Math.TSP;
using OsmSharp.Tools.Math.TSP.ArbitraryInsertion;
using OsmSharp.Tools.Math.TSP.CheapestInsertion;
using OsmSharp.Tools.Math.TSP.EdgeAssemblyGenetic;
using OsmSharp.Tools.Math.TSP.Genetic;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.CrossOver;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Generation;
using OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.Mutation;
using OsmSharp.Tools.Math.TSP.LocalSearch.HillClimbing3Opt;
using OsmSharp.Tools.TSPLIB.Parser;
using OsmSharp.Tools.TSPLIB.Problems;
using System.Diagnostics;

namespace OsmSharp.Tools.Benchmark.TSPLIB
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DoTests();

            OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Finished!");
            Console.ReadLine();
        }

        static void DoTests()
        {
            //bool ga = false;
            //bool eax = true;
            OsmSharp.Tools.Output.OutputStreamHost.RegisterOutputStream(
                new OsmSharp.Tools.Output.ConsoleOutputStream());
            OsmSharp.Tools.Output.OutputStreamHost.RegisterOutputStream(
                new OsmSharp.Tools.Output.FileOutputStream(@"c:\temp\results_x64.txt"));

            List<TSPLIBProblem> problems = new List<TSPLIBProblem>();

            //problems.Add(Program.CreateProblem(@"\Problems\DM\{0}", "031_K1040-06.atsp", 341));
            //problems.Add(Program.CreateProblem(@"\Problems\DM\{0}", "036_K1210-01.atsp", 720));
            //problems.Add(Program.CreateProblem(@"\Problems\DM\{0}", "061_K3511.atsp", 281));
            //problems.Add(Program.CreateProblem(@"\Problems\DM\{0}", "072_K3510.atsp", 186));
            //problems.Add(Program.CreateProblem(@"\Problems\DM\{0}", "098_K3089.atsp", 403));
            //problems.Add(Program.CreateProblem(@"\Problems\DM\{0}", "122_K4052.atsp", 463));
            //problems.Add(Program.CreateProblem(@"\Problems\DM\{0}", "151_K7537.atsp", 1489));
            //problems.Add(Program.CreateProblem(@"\Problems\DM\{0}", "168_K2160.atsp", 2232));
            //problems.Add(Program.CreateProblem(@"\Problems\DM\{0}", "181_K4207.atsp", 1234));
            //problems.Add(Program.CreateProblem(@"\Problems\DM\{0}", "209_K2125.atsp", 1284));
            //problems.Add(Program.CreateProblem(@"\Problems\DM\{0}", "254_K3504.atsp", 568));

            //problems.Add(Program.CreateProblem(@"\Problems\TSP\{0}", "a280.tsp", 2579));

            //// ATSP instances.
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "br17.atsp", 39));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ftv33.atsp", 1286));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ftv35.atsp", 1473));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ftv38.atsp", 1530));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "p43.atsp", 5620));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ftv44.atsp", 1613));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ftv47.atsp", 1776));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ry48p.atsp", 14422));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ft53.atsp", 6905));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ftv55.atsp", 1608));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ftv64.atsp", 1839));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ft70.atsp", 38673));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ftv70.atsp", 1950));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "kro124p.atsp", 36230));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "ftv170.atsp", 2755));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "rbg323.atsp", 1326));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "rbg403.atsp", 2465));
            //problems.Add(Program.CreateProblem(@"\Problems\ATSP\{0}", "rbg443.atsp", 2720));

            List<ISolver> solvers = new List<ISolver>();
            solvers.Add(new HillClimbing3OptSolver(true, true));
            solvers.Add(new CheapestInsertionSolver());
            solvers.Add(new ArbitraryInsertionSolver());
            //Program.DoAddSolvers(solvers, false, true, 100, 20);
            //Program.DoAddSolvers(solvers, false, true, 300, 20);

            TSPLIBTester tester = new TSPLIBTester("log", problems, solvers, 100);
            tester.StartTests();

            //    DoTest(name, "br17.atsp", false, 100000000, log_stream);
            //    DoTest(name, "ftv33.atsp", false, 100000000, log_stream);
            //    DoTest(name, "ftv35.atsp", false, 100000000, log_stream);
            //    DoTest(name, "ftv38.atsp", false, 100000000, log_stream);
            //    DoTest(name, "p43.atsp", false, 100000000, log_stream);
            //    DoTest(name, "ftv44.atsp", false, 100000000, log_stream);
            //    DoTest(name, "ftv47.atsp", false, 100000000, log_stream);
            //    DoTest(name, "ry48p.atsp", false, 100000000, log_stream);
            //    DoTest(name, "ftv55.atsp", false, 100000000, log_stream);
            //    DoTest(name, "ftv64.atsp", false, 100000000, log_stream);
            //    DoTest(name, "ft70.atsp", false, 100000000, log_stream);
            //    DoTest(name, "ftv70.atsp", false, 100000000, log_stream);
            //    DoTest(name, "kro124p.atsp", false, 100000000, log_stream);
            //    DoTest(name, "ftv170.atsp", false, 100000000, log_stream);
            //    DoTest(name, "rbg323.atsp", false, 100000000, log_stream);
            //    DoTest(name, "rbg358.atsp", false, 100000000, log_stream);
            //    DoTest(name, "rbg403.atsp", false, 100000000, log_stream);
            //    DoTest(name, "rbg443.atsp", false, 100000000, log_stream);
        }

        static void DoAddSolvers( List<ISolver> solvers, bool ga, bool eax, int population, int stagnation)
        {
            //int population = 100;
            //int stagnation = 20;

            if (ga)
            {
                //solvers.Add(new GeneticSolver(population, stagnation, 0,
                //        new BestPlacementMutationOperation(), 10,
                //        new EdgeAssemblyCrossover(5,
                //            EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
                //            false), 90,
                //        new BestPlacementGenerationOperation()));
                //solvers.Add(new GeneticSolver(population, stagnation, 0,
                //        new BestPlacementMutationOperation(), 10,
                //        new EdgeAssemblyCrossover(30,
                //            EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
                //            false), 90,
                //        new BestPlacementGenerationOperation()));
                solvers.Add(new GeneticSolver(population, stagnation, 0,
                          new BestPlacementMutationOperation(), 10,
                          new EdgeAssemblyCrossover(30,
                              EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
                              true), 90,
                          new BestPlacementGenerationOperation()));
                //solvers.Add(new GeneticSolver(population, stagnation, 0,
                //        new BestPlacementMutationOperation(), 10,
                //        new EdgeAssemblyCrossover(30,
                //            EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.MultipleRandom,
                //            false), 90,
                //        new BestPlacementGenerationOperation()));
                //solvers.Add(new GeneticSolver(population, stagnation, 0,
                //        new BestPlacementMutationOperation(), 10,
                //        new EdgeAssemblyCrossover(30,
                //            EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.MultipleRandom,
                //            false), 90,
                //        new BestPlacementGenerationOperation()));
            }
            if (eax)
            {
                //solvers.Add(new EdgeAssemblyCrossOverSolver(population, stagnation,
                //    new BestPlacementGenerationOperation(),
                //     new EdgeAssemblyCrossover(1,
                //            EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.MultipleRandom,
                //            true)));
                //solvers.Add(new EdgeAssemblyCrossOverSolver(population, stagnation,
                //     new BestPlacementGenerationOperation(),
                //      new EdgeAssemblyCrossover(5,
                //             EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.MultipleRandom,
                //             true)));
                //solvers.Add(new EdgeAssemblyCrossOverSolver(population, stagnation,
                //     new BestPlacementGenerationOperation(),
                //      new EdgeAssemblyCrossover(10,
                //             EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.MultipleRandom,
                //             true)));
                //solvers.Add(new EdgeAssemblyCrossOverSolver(population, stagnation,
                //     new BestPlacementGenerationOperation(),
                //      new EdgeAssemblyCrossover(30,
                //             EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.MultipleRandom,
                //             true)));
                //solvers.Add(new EdgeAssemblyCrossOverSolver(population, stagnation,
                //     new _3OptGenerationOperation(),
                //      new EdgeAssemblyCrossover(30,
                //             EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.MultipleRandom,
                //             true)));
                //solvers.Add(new EdgeAssemblyCrossOverSolver(population, stagnation,
                //    new BestPlacementGenerationOperation(),
                //     new EdgeAssemblyCrossover(1,
                //            EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
                //            true)));
                //solvers.Add(new EdgeAssemblyCrossOverSolver(population, stagnation,
                //     new BestPlacementGenerationOperation(),
                //      new EdgeAssemblyCrossover(5,
                //             EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
                //             true)));
                //solvers.Add(new EdgeAssemblyCrossOverSolver(population, stagnation,
                //     new BestPlacementGenerationOperation(),
                //      new EdgeAssemblyCrossover(30,
                //             EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
                //             true)));
                //solvers.Add(new EdgeAssemblyCrossOverSolver(population, stagnation,
                //     new _3OptGenerationOperation(),
                //      new EdgeAssemblyCrossover(30,
                //             EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
                //             false)));
                solvers.Add(new EdgeAssemblyCrossOverSolver(population, stagnation,
                     new _3OptGenerationOperation(),
                      new EdgeAssemblyCrossover(30,
                             EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
                             true)));
                //solvers.Add(new EdgeAssemblyCrossOverSolver(population, stagnation,
                //     new _3OptGenerationOperation(),
                //      new EdgeAssemblyCrossover(30,
                //             EdgeAssemblyCrossover.EdgeAssemblyCrossoverSelectionStrategyEnum.SingleRandom,
                //             true)));
            }
        }

        static TSPLIBProblem CreateProblem(string path, string file, float best)
        {
            TSPLIBProblem problem = TSPLIBProblemParser.ParseFrom(new FileInfo(new FileInfo(
                    Assembly.GetExecutingAssembly().Location).DirectoryName + string.Format(path, file)));
            problem.Best = best;
            return problem;
        }

        static TSPLIBProblem CreateProblem(string path, string file, float best, int? first, int? last)
        {
            TSPLIBProblem problem = TSPLIBProblemParser.ParseFrom(new FileInfo(new FileInfo(
                    Assembly.GetExecutingAssembly().Location).DirectoryName + string.Format(path, file)),
                    first, last);
            problem.Best = best;
            return problem;
        }
    }
}
