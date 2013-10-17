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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo.Meta;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.ArcAggregation.Output;

namespace OsmSharp.Routing.Instructions.LanguageGeneration
{
    /// <summary>
    /// Scentence planner for routing instructions.
    /// </summary>
    internal class SentencePlanner
    {
        /// <summary>
        /// Holds the list of planned instructions.
        /// </summary>
        private List<Instruction> _instructions;

        /// <summary>
        /// Holds the language-specific generator.
        /// </summary>
        private ILanguageGenerator _generator;

        /// <summary>
        /// Creates a new scentence planner.
        /// </summary>
        /// <param name="generator"></param>
        public SentencePlanner(ILanguageGenerator generator)
        {
            _generator = generator;
            _instructions = new List<Instruction>();
        }

        /// <summary>
        /// Not sure this should be here?
        /// </summary>
        /// <returns></returns>
        public List<Instruction> Instructions
        {
            get
            {
                return _instructions;
            }
        }

        /// <summary>
        /// Generates a turn instruction.
        /// </summary>
        /// <param name="entryIdx"></param>
        /// <param name="box"></param>
        /// <param name="direction"></param>
        /// <param name="streetCountTurn"></param>
        /// <param name="streetCountBeforeTurn"></param>
        /// <param name="streetFrom"></param>
        /// <param name="streetTo"></param>
        /// <param name="list"></param>
        internal void GenerateTurn(int entryIdx, GeoCoordinateBox box,
            RelativeDirection direction, int streetCountTurn, int streetCountBeforeTurn,
            TagsCollection streetFrom, TagsCollection streetTo, List<PointPoi> list)
        {
            // create a new instruction first.
            Instruction instruction = new Instruction(entryIdx, box, list);

            // pass the instruction to the languate generator.
            // test if the street is the same but a turn needs to be taken anyway.
            if (streetFrom == streetTo)
            {
                if (streetCountTurn == 0)
                {// there are no other streets between the one being turned into and the street coming from in the same
                    // direction as the turn.
                    instruction = _generator.GenerateDirectFollowTurn(
                        instruction, streetCountBeforeTurn, streetTo, direction.Direction, list);
                }
                else
                { // there is another street; this is tricky to explain.
                    instruction = _generator.GenerateIndirectFollowTurn(
                        instruction, streetCountTurn, streetCountBeforeTurn, streetTo, direction.Direction, list);
                }
            }
            else
            {
                if (streetCountTurn == 0)
                { // there are no other streets between the one being turned into and the street coming from in the same
                    // direction as the turn.
                    instruction = _generator.GenerateDirectTurn(
                        instruction, streetCountBeforeTurn, streetTo, direction.Direction, list);
                }
                else
                { // there is another street; this is tricky to explain.
                    instruction = _generator.GenerateIndirectTurn(
                        instruction, streetCountTurn, streetCountBeforeTurn, streetTo, direction.Direction, list);
                }
            }

            // add the instruction to the instructions list.
            _instructions.Add(instruction);
        }

        /// <summary>
        /// Generates a poi instruction.
        /// </summary>
        /// <param name="entryIdx"></param>
        /// <param name="box"></param>
        /// <param name="list"></param>
        /// <param name="direction"></param>
        internal void GeneratePoi(int entryIdx, GeoCoordinateBox box, 
            List<PointPoi> list, RelativeDirection direction)
        {
            // create a new instruction first.
            Instruction instruction = new Instruction(entryIdx, box, list);
            Instruction direction_instruction = null;

            // pass the instruction to the languate generator.
            if (direction == null)
            {
                instruction = _generator.GeneratePoi(instruction, list, null);
            }
            else
            {
                // create a direction instruction.
                if (direction.Direction == RelativeDirectionEnum.TurnBack)
                {
                    direction_instruction = new Instruction(entryIdx, box, list);
                    direction_instruction = _generator.GenerateSimpleTurn(direction_instruction, direction.Direction);
                }

                // generates the instructions.
                instruction = _generator.GeneratePoi(instruction, list, null);
            }

            // add the instruction to the instructions list.
            _instructions.Add(instruction);

            // add the direction instruction.
            if (direction_instruction != null)
            {
                _instructions.Add(direction_instruction);
            }
        }

        /// <summary>
        /// Generates an immidiate turn instruction.
        /// </summary>
        /// <param name="entryIdx"></param>
        /// <param name="box"></param>
        /// <param name="before_name"></param>
        /// <param name="first_direction"></param>
        /// <param name="first_street_count_to"></param>
        /// <param name="second_direction"></param>
        /// <param name="first_street_to"></param>
        /// <param name="second_street_to"></param>
        /// <param name="list"></param>
        internal void GenerateImmidiateTurn(int entryIdx, GeoCoordinateBox box,
            TagsCollection before_name, RelativeDirection first_direction, int first_street_count_to,
            RelativeDirection second_direction, TagsCollection first_street_to, TagsCollection second_street_to, List<PointPoi> list)
        {
            // create a new instruction first.
            Instruction instruction = new Instruction(entryIdx, box);

            // pass the instruction to the language generator.
            instruction = _generator.GenerateImmidiateTurn(
                instruction, first_street_count_to, first_street_to, first_direction, second_street_to, second_direction);

            // add the instruction to the instructions list.
            _instructions.Add(instruction);
        }

        /// <summary>
        /// Generates a roudabout instruction.
        /// </summary>
        /// <param name="entryIdx"></param>
        /// <param name="box"></param>
        /// <param name="count"></param>
        /// <param name="next_street"></param>
        internal void GenerateRoundabout(int entryIdx, GeoCoordinateBox box, int count, TagsCollection next_street)
        {
            // create a new instruction first.
            Instruction instruction = new Instruction(entryIdx, box);

            // pass the instruction to the language generator.
            instruction = _generator.GenerateRoundabout(instruction, count, next_street);

            // add the instruction to the instructions list.
            _instructions.Add(instruction);
        }
    }
}
