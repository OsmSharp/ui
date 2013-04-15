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
using System.IO;
using System.Reflection;
using OsmSharp.Tools.Progress;
using OsmSharp.Tools.TSPLIB.Parser;
using OsmSharp.Tools.Math.TSP;
using OsmSharp.Tools.TSPLIB.Problems;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Tools.Benchmark.TSPLIB
{
    /// <summary>
    /// Class with facilities to test TSP solvers.
    /// </summary>
    internal class TSPLIBTester
    {
        /// <summary>
        /// Holds the test count.
        /// </summary>
        private int _test_count;

        /// <summary>
        /// Holds the name.
        /// </summary>
        private string _name;

        /// <summary>
        /// Holds the list of problems.
        /// </summary>
        private IList<TSPLIBProblem> _problems;

        /// <summary>
        /// Holds the list of solvers.
        /// </summary>
        private IList<ISolver> _solvers;

        /// <summary>
        /// Creates a new tester.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="problems"></param>
        /// <param name="solvers"></param>
        public TSPLIBTester(string name, IList<TSPLIBProblem> problems, IList<ISolver> solvers)
            : this(name, problems, solvers, 3) { }

        /// <summary>
        /// Creates a new tester.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="problems"></param>
        /// <param name="solvers"></param>
        /// <param name="test_count"></param>
        public TSPLIBTester(string name, IList<TSPLIBProblem> problems, IList<ISolver> solvers, int test_count)
        {
            _name = name;
            _test_count = test_count;

            _problems = problems;
            _solvers = solvers;
        }

        private static string PadRight(object obj)
        {
            return TSPLIBTester.PadRight(obj, 10);
        }

        private static string PadRight(object obj, int count)
        {
            string obj_str = string.Empty;
            if (obj != null)
            {
                obj_str = obj.ToString();
            }
            return obj_str.PadRight(count);
        }

        private static string ToStringEmptyWhenNull(object obj)
        {
            string obj_str = string.Empty;
            if (obj != null)
            {
                obj_str = obj.ToString();
            }
            return obj_str;
        }

        /// <summary>
        /// Does the testing.
        /// </summary>
        public void StartTests()
        {
            // open a writer to the output file.
            FileInfo output_file = new FileInfo(string.Format("{0}.log", _name));
            StreamWriter writer = output_file.AppendText();
            //Tools.Core.Output.OutputStreamHost.WriteLine("====== {0} started! ======", _name);
            //Tools.Core.Output.OutputStreamHost.WriteLine();
            //Tools.Core.Output.OutputStreamHost.WriteLine("Started: {0}", problem.Name);
            OsmSharp.Tools.Output.OutputStreamHost.WriteLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}",
                PadRight("Problem", 15),
                PadRight("Name", 40),
                PadRight("Time"),
                PadRight("Time (Avg)"),
                PadRight("# Runs", 6),
                PadRight("# Bests", 7),
                PadRight("Avg"),
                PadRight("Best"),
                PadRight("Worst"),
                PadRight("Optimal"),
                PadRight("%")));

            // test each combination of solver and problem test_count-times.
            for (int problem_idx = 0; problem_idx < _problems.Count; problem_idx++)
            { // get the problem instance.
                TSPLIBProblem problem = _problems[problem_idx];

                // loop over all solvers.
                for (int solver_idx = 0; solver_idx < _solvers.Count; solver_idx++)
                { // get the solver instance.
                    ISolver solver = _solvers[solver_idx];
                    //Tools.Core.Output.OutputStreamHost.WriteLine("Started: {0} -> {1}", problem.Name, solver.Name);

                    // do the tests.
                    double best = float.MaxValue;
                    double worst = float.MinValue;

                    int best_count = 0;
                    long start_ticks = DateTime.Now.Ticks;
                    double total = 0;
                    int test_count = _test_count;
                    while (test_count > 0)
                    {
                        // do the actual test.
                        IRoute route = solver.Solve(problem);
                        // calculate the weight.
                        double weight = this.CalculateWeight(route, problem);
                        total = total + weight;
                        if (best > weight)
                        {
                            best = weight;
                        }
                        if (worst < weight)
                        {
                            worst = weight;
                        }

                        if (weight == problem.Best)
                        {
                            best_count++;
                        }

                        test_count--; // a test has been done
                    }
                    long stop_ticks = DateTime.Now.Ticks;

                    // report the result.
                    TimeSpan time = new TimeSpan(stop_ticks - start_ticks);
                    string line = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};",
                        ToStringEmptyWhenNull(problem.Name),
                        ToStringEmptyWhenNull(solver.Name),
                        ToStringEmptyWhenNull(problem.Best),
                        ToStringEmptyWhenNull(_test_count),
                        ToStringEmptyWhenNull(System.Math.Round(time.TotalSeconds / (double)_test_count, 3)),
                        ToStringEmptyWhenNull(best_count),
                        ToStringEmptyWhenNull(System.Math.Round(total / (double)_test_count, 3)),
                        ToStringEmptyWhenNull(best),
                        ToStringEmptyWhenNull(worst));
                    OsmSharp.Tools.Output.OutputStreamHost.WriteLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}",
                        PadRight(problem.Name, 15),
                        PadRight(solver.Name, 40),
                        PadRight(System.Math.Round(time.TotalSeconds, 3)),
                        PadRight(System.Math.Round(time.TotalSeconds / (double)_test_count, 3)),
                        PadRight(_test_count, 6),
                        PadRight(best_count, 7),
                        PadRight(System.Math.Round(total /  (double)_test_count, 3)),
                        PadRight(best),
                        PadRight(worst),
                        PadRight(problem.Best),
                        PadRight(System.Math.Round(
                            (((total /  (double)_test_count) - problem.Best) / problem.Best) * 100.0, 3))));
                    writer.WriteLine(line);
                    writer.Flush();
                }
                OsmSharp.Tools.Output.OutputStreamHost.WriteLine();
            }
            writer.Flush();
            writer.Close();
        }

        private double CalculateWeight(IRoute route, TSPLIBProblem problem)
        {
            int previous = -1;
            int first = -1;
            double weight = 0;
            foreach (int customer in route)
            {
                if (first < 0)
                {
                    first = customer;
                }
                if (previous < 0) { }
                else
                {
                    weight = weight + problem.WeightMatrix[previous][customer];
                }
                previous = customer;
            }
            weight = weight + problem.WeightMatrix[previous][first];
            return weight;
        }

        //#region Report/Status keeping

        ///// <summary>
        ///// Holds the latest reported fitness.
        ///// </summary>
        //private float? _current_problem_last_fitness;

        ///// <summary>
        ///// Holds the total reported fitness.
        ///// </summary>
        //private float? _current_problem_total_fitness;

        ///// <summary>
        ///// Holds the best solution so far.
        ///// </summary>
        //private float _current_problem_best_solution;

        ///// <summary>
        ///// Holds the best value for the current solution.
        ///// </summary>
        //private float _current_problem_best;

        ///// <summary>
        ///// Holds the current optimal count.
        ///// </summary>
        //private int _current_problem_optimal_count;

        ///// <summary>
        ///// Holds the start ticks count.
        ///// </summary>
        //private long _current_problem_start_ticks;

        ///// <summary>
        ///// Holds the stop ticks count.
        ///// </summary>
        //private long _current_problem_stop_ticks;

        ///// <summary>
        ///// Resets everything before testing a new solver.
        ///// </summary>
        //private void ResetTest(float best)
        //{
        //    this.ResetLocalTest();

        //    _current_problem_best = best;
        //    _current_problem_optimal_count = 0;
        //    _current_problem_best_solution = float.MaxValue;
        //    _current_problem_total_fitness = 0;

        //    _current_problem_start_ticks = DateTime.Now.Ticks;
        //}

        ///// <summary>
        ///// Resets everything for a new test run.
        ///// </summary>
        //private void ResetLocalTest()
        //{
        //    _current_problem_last_fitness = float.MaxValue;
        //}

        ///// <summary>
        ///// The local test was finished.
        ///// </summary>
        //private void FinishedLocal()
        //{
        //    _current_problem_total_fitness = _current_problem_total_fitness + _current_problem_best;
        //}

        ///// <summary>
        ///// The test was finished.
        ///// </summary>
        //private void FinishedTest()
        //{
        //    _current_problem_stop_ticks = DateTime.Now.Ticks;
        //}

        //#endregion

    }
}
