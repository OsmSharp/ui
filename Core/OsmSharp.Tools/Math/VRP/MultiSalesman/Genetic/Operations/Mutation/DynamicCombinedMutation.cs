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
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.Random;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic;

namespace OsmSharp.Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class DynamicCombinedMutation : CombinedMutation<List<Genome>, Problem, Fitness>
    {
        private IList<double> _initial;
        private IList<double> _increase_rounds;
        private IList<double> _decrease_rounds;
        private IList<double> _decrease_time;

        private bool _regime_reached = false;

        public DynamicCombinedMutation(
            IList<IMutationOperation<List<Genome>, Problem, Fitness>> operations,
            IList<double> initial,
            IList<double> increase_rounds,
            IList<double> decrease_rounds,
            IList<double> decrease_time)
            :base(StaticRandomGenerator.Get(),
            operations,
            initial)
        {
            _initial = initial;
            _decrease_rounds = decrease_rounds;
            _increase_rounds = increase_rounds;
            _decrease_time = decrease_time;
        }

        public override Individual<List<Genome>, Problem, Fitness> Mutate(
            Solver<List<Genome>, Problem, Fitness> solver, Individual<List<Genome>, Problem, Fitness> mutating)
        {
            // determine correct probalities.
            if (mutating.Fitness.Feasable)
            { // decrease total time.
                if (_regime_reached == false)
                {
                    OsmSharp.Tools.Output.OutputStreamHost.WriteLine(
                        "Regime Reached!");
                }
                //Tools.Core.Output.OutputTextStreamHost.Write("DT");
                this.Probabilities = _decrease_time;
                _regime_reached = true;
                if (solver.Fittest.Fitness.LargestRoundCategory == 0
                    || solver.Fittest.Fitness.SmallestRoundCategory == 0)
                {
                    if (solver.Fittest.Fitness.LargestRoundCategory >
                        solver.Fittest.Fitness.SmallestRoundCategory)
                    { // reduce the number of rounds.
                        this.Probabilities = _decrease_rounds;
                    }
                    else
                    { // increase the number of rounds.
                        this.Probabilities = _increase_rounds;
                    }
                }
            }
            else
            {
                //Tools.Core.Output.OutputTextStreamHost.Write("N");
                this.Probabilities = _initial;
            }

            // actually select one.
 	        Individual<List<Genome>, Problem, Fitness> result = base.Mutate(solver, mutating);
            result.Validate(solver.Problem);
            return result;
        }
    }
}
