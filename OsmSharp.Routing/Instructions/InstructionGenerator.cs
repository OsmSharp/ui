// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using System.Collections.Generic;
using OsmSharp.Routing.ArcAggregation.Output;
using OsmSharp.Routing.Instructions.LanguageGeneration;
using OsmSharp.Routing.Interpreter;

namespace OsmSharp.Routing.Instructions
{
    /// <summary>
    /// Instruction generator.
    /// </summary>
    public static class InstructionGenerator
    {
        /// <summary>
        /// Generates instructions.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public static List<Instruction> Generate(Route route, IRoutingInterpreter interpreter)
        {
            return InstructionGenerator.Generate(route, interpreter,
                new OsmSharp.Routing.Instructions.LanguageGeneration.Defaults.SimpleEnglishLanguageGenerator());
        }

        /// <summary>
        /// Generates instructions.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="interpreter"></param>
        /// <param name="languageGenerator"></param>
        /// <returns></returns>
        public static List<Instruction> Generate(Route route, IRoutingInterpreter interpreter, ILanguageGenerator languageGenerator)
        {
            OsmSharp.Routing.ArcAggregation.ArcAggregator aggregator = 
                new OsmSharp.Routing.ArcAggregation.ArcAggregator(interpreter);
            AggregatedPoint point = 
                aggregator.Aggregate(route);

			return InstructionGenerator.Generate(point, interpreter, languageGenerator);
        }

        /// <summary>
        /// Generates instructions.
        /// </summary>
        /// <param name="aggregatePoint"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public static List<Instruction> Generate(AggregatedPoint aggregatePoint, IRoutingInterpreter interpreter)
        {
			return InstructionGenerator.Generate(aggregatePoint, interpreter,
                new OsmSharp.Routing.Instructions.LanguageGeneration.Defaults.SimpleEnglishLanguageGenerator());
        }

        /// <summary>
        /// Generates instructions.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="interpreter"></param>
        /// <param name="languagGenerator"></param>
        /// <returns></returns>
        public static List<Instruction> Generate(AggregatedPoint point, IRoutingInterpreter interpreter, ILanguageGenerator languagGenerator)
        {
            MicroPlanning.MicroPlanner planner = new MicroPlanning.MicroPlanner(languagGenerator, interpreter);
            return planner.Plan(point);
        }
    }
}