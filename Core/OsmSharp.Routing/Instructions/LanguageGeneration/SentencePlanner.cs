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
using OsmSharp.Tools.Math.Geo.Meta;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.ArcAggregation.Output;

namespace OsmSharp.Routing.Instructions.LanguageGeneration
{
    internal class SentencePlanner
    {
        private List<Instruction> _instructions;

        private ILanguageGenerator _generator;

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

        internal void GenerateTurn(GeoCoordinateBox box,
            RelativeDirection direction, 
            int street_count_turn,
            int street_count_before_turn, 
            List<KeyValuePair<string, string>> street_from,
            List<KeyValuePair<string, string>> street_to, 
            List<PointPoi> list)
        {
            // create a new instruction first.
            Instruction instruction = new Instruction(box, list);

            // pass the instruction to the languate generator.
            // test if the street is the same but a turn needs to be taken anyway.
            if (street_from == street_to)
            {
                if (street_count_turn == 0)
                {// there are no other streets between the one being turned into and the street coming from in the same
                    // direction as the turn.
                    instruction = _generator.GenerateDirectFollowTurn(
                        instruction, street_count_before_turn, street_to, direction.Direction, list);
                }
                else
                { // there is another street; this is tricky to explain.
                    instruction = _generator.GenerateIndirectFollowTurn(
                        instruction, street_count_turn, street_count_before_turn, street_to, direction.Direction, list);
                }
            }
            else
            {
                if (street_count_turn == 0)
                { // there are no other streets between the one being turned into and the street coming from in the same
                    // direction as the turn.
                    instruction = _generator.GenerateDirectTurn(
                        instruction, street_count_before_turn, street_to, direction.Direction, list);
                }
                else
                { // there is another street; this is tricky to explain.
                    instruction = _generator.GenerateIndirectTurn(
                        instruction, street_count_turn, street_count_before_turn, street_to, direction.Direction, list);
                }
            }

            // add the instruction to the instructions list.
            _instructions.Add(instruction);
        }

        internal void GeneratePoi(GeoCoordinateBox box, 
            List<PointPoi> list,
            RelativeDirection direction)
        {
            // create a new instruction first.
            Instruction instruction = new Instruction(box, list);
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
                    direction_instruction = new Instruction(box, list);
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

        internal void GenerateImmidiateTurn(GeoCoordinateBox box,
            List<KeyValuePair<string, string>> before_name, RelativeDirection first_direction, int first_street_count_to,
            RelativeDirection second_direction, List<KeyValuePair<string, string>> first_street_to, List<KeyValuePair<string, string>> second_street_to, List<PointPoi> list)
        {
            // create a new instruction first.
            Instruction instruction = new Instruction(box);

            // pass the instruction to the language generator.
            instruction = _generator.GenerateImmidiateTurn(
                instruction, first_street_count_to, first_street_to, first_direction, second_street_to, second_direction);

            // add the instruction to the instructions list.
            _instructions.Add(instruction);
        }

        internal void GenerateRoundabout(GeoCoordinateBox box, int count, List<KeyValuePair<string, string>> next_street)
        {
            // create a new instruction first.
            Instruction instruction = new Instruction(box);

            // pass the instruction to the language generator.
            instruction = _generator.GenerateRoundabout(instruction, count, next_street);

            // add the instruction to the instructions list.
            _instructions.Add(instruction);
        }
    }
}
