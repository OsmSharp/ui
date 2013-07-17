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
using OsmSharp.Routing.Route;
using OsmSharp.Routing.ArcAggregation.Output;
using OsmSharp.Routing.Instructions.LanguageGeneration;
using OsmSharp.Routing.Interpreter;

namespace OsmSharp.Routing.Instructions
{
    /// <summary>
    /// Instruction generator.
    /// </summary>
    public class InstructionGenerator
    {
        /// <summary>
        /// Generates instructions.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public List<Instruction> Generate(OsmSharpRoute route, IRoutingInterpreter interpreter)
        {
            return this.Generate(route, interpreter,
                new OsmSharp.Routing.Instructions.LanguageGeneration.Defaults.SimpleEnglishLanguageGenerator());
        }

        /// <summary>
        /// Generates instructions.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="interpreter"></param>
        /// <param name="language_generator"></param>
        /// <returns></returns>
        public List<Instruction> Generate(OsmSharpRoute route, IRoutingInterpreter interpreter, ILanguageGenerator language_generator)
        {
            OsmSharp.Routing.ArcAggregation.ArcAggregator aggregator = 
                new OsmSharp.Routing.ArcAggregation.ArcAggregator(interpreter);
            AggregatedPoint point = 
                aggregator.Aggregate(route);

            return this.Generate(point, interpreter, language_generator);
        }

        /// <summary>
        /// Generates instructions.
        /// </summary>
        /// <param name="aggregate_point"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public List<Instruction> Generate(AggregatedPoint aggregate_point, IRoutingInterpreter interpreter)
        {
            return this.Generate(aggregate_point, interpreter,
                new OsmSharp.Routing.Instructions.LanguageGeneration.Defaults.SimpleEnglishLanguageGenerator());
        }

        /// <summary>
        /// Generates instructions.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="interpreter"></param>
        /// <param name="language_generator"></param>
        /// <returns></returns>
        public List<Instruction> Generate(AggregatedPoint point, IRoutingInterpreter interpreter, ILanguageGenerator language_generator)
        {
            MicroPlanning.MicroPlanner planner = new MicroPlanning.MicroPlanner(language_generator, interpreter);
            return planner.Plan(point);
        }
    }
}
