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
using OsmSharp.Routing.ArcAggregation.Output;
using OsmSharp.Routing.Instructions;
using OsmSharp.Routing.Instructions.LanguageGeneration;
using OsmSharp.Math.Geo.Meta;
using OsmSharp.Collections.Tags;

namespace OsmSharp.UnitTests.Routing.Instructions
{
    /// <summary>
    /// Language test generator.
    /// </summary>
    public class LanguageTestGenerator : ILanguageGenerator
    {
        /// <summary>
        /// Direct turn instruction.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="streetCountBeforeTurn"></param>
        /// <param name="streetTo"></param>
        /// <param name="direction"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Instruction GenerateDirectTurn(Instruction instruction,
            int streetCountBeforeTurn,
            TagsCollection streetTo,
            RelativeDirectionEnum direction,
            List<PointPoi> list)
        {
            instruction.Text = string.Format("GenerateDirectTurn:{0}_{1}_{2}",
                                             streetCountBeforeTurn, direction.ToString(), list.Count);

            instruction.Extras = new Dictionary<string, object>();
            instruction.Extras.Add("streetCountBeforeTurn", streetCountBeforeTurn);
            instruction.Extras.Add("streetTo", streetTo);
            instruction.Extras.Add("direction", direction);
            instruction.Extras.Add("list", list);

            return instruction;
        }

        /// <summary>
        /// Generates an indirect turn.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="streetCountTurn"></param>
        /// <param name="streetCountBeforeTurn"></param>
        /// <param name="streetTo"></param>
        /// <param name="direction"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Instruction GenerateIndirectTurn(Instruction instruction,
            int streetCountTurn,
            int streetCountBeforeTurn,
            TagsCollection streetTo,
            RelativeDirectionEnum direction,
            List<PointPoi> list)
        {
            instruction.Text = string.Format("GenerateIndirectTurn:{0}_{1}_{2}_{3}",
                                             streetCountTurn, streetCountBeforeTurn,
                                             direction.ToString(), list.Count);

            instruction.Extras = new Dictionary<string, object>();
            instruction.Extras.Add("streetCountTurn", streetCountTurn);
            instruction.Extras.Add("streetCountBeforeTurn", streetCountBeforeTurn);
            instruction.Extras.Add("streetTo", streetTo);
            instruction.Extras.Add("direction", direction);
            instruction.Extras.Add("list", list);

            return instruction;
        }

        /// <summary>
        /// Generates poi instruction.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="list"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Instruction GeneratePoi(Instruction instruction, List<PointPoi> list,
            RelativeDirectionEnum? direction)
        {
            if (direction.HasValue)
            {
                instruction.Text = string.Format("GeneratePoi:{0}_{1}",
                    list.Count, direction.Value.ToString());
            }
            else
            {
                instruction.Text = string.Format("GeneratePoi:{0}",
                                                 list.Count);
            }

            instruction.Extras = new Dictionary<string, object>();
            instruction.Extras.Add("direction", direction);
            instruction.Extras.Add("list", list);

            return instruction;
        }

        /// <summary>
        /// Generates a direct follow turn.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="streetCountBeforeTurn"></param>
        /// <param name="streetTo"></param>
        /// <param name="relativeDirectionEnum"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Instruction GenerateDirectFollowTurn(Instruction instruction,
            int streetCountBeforeTurn,
            TagsCollection streetTo,
            RelativeDirectionEnum direction,
            List<PointPoi> list)
        {
            instruction.Text = string.Format("GenerateDirectFollowTurn:{0}_{1}_{2}",
                                             streetCountBeforeTurn, direction.ToString(), list.Count);

            instruction.Extras = new Dictionary<string, object>();
            instruction.Extras.Add("streetCountBeforeTurn", streetCountBeforeTurn);
            instruction.Extras.Add("streetTo", streetTo);
            instruction.Extras.Add("direction", direction);
            instruction.Extras.Add("list", list);

            return instruction;
        }

        /// <summary>
        /// Generates an indirect follow turn.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="streetCountTurn"></param>
        /// <param name="streetCountBeforeTurn"></param>
        /// <param name="streetTo"></param>
        /// <param name="relativeDirectionEnum"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public Instruction GenerateIndirectFollowTurn(Instruction instruction,
            int streetCountTurn,
            int streetCountBeforeTurn,
            TagsCollection streetTo,
            RelativeDirectionEnum direction,
            List<PointPoi> list)
        {
            instruction.Text = string.Format("GenerateDirectFollowTurn:{0}_{1}_{2}",
                                             streetCountBeforeTurn, streetCountBeforeTurn,
                                             direction.ToString(), list.Count);

            instruction.Extras = new Dictionary<string, object>();
            instruction.Extras.Add("streetCountTurn", streetCountTurn);
            instruction.Extras.Add("streetCountBeforeTurn", streetCountBeforeTurn);
            instruction.Extras.Add("streetTo", streetTo);
            instruction.Extras.Add("direction", direction);
            instruction.Extras.Add("list", list);

            return instruction;
        }

        /// <summary>
        /// Generates an immidiate turn.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="firstStreetCountTo"></param>
        /// <param name="firstStreetTo"></param>
        /// <param name="firstDirection"></param>
        /// <param name="secondStreetTo"></param>
        /// <param name="secondDirection"></param>
        /// <returns></returns>
        public Instruction GenerateImmidiateTurn(Instruction instruction,
            int firstStreetCountTo,
            TagsCollection firstStreetTo,
            OsmSharp.Math.Geo.Meta.RelativeDirection firstDirection,
            TagsCollection secondStreetTo,
            RelativeDirection secondDirection)
        {
            instruction.Text = string.Format("GenerateImmidiateTurn:{0}_{1}_{2}_{3}",
                                             firstStreetCountTo, firstDirection,
                                             firstDirection.ToString(),
                                             secondDirection.ToString());

            instruction.Extras = new Dictionary<string, object>();
            instruction.Extras.Add("firstStreetCountTo", firstStreetCountTo);
            instruction.Extras.Add("firstStreetTo", firstStreetTo);
            instruction.Extras.Add("firstDirection", firstDirection);
            instruction.Extras.Add("secondStreetTo", secondStreetTo);
            instruction.Extras.Add("secondDirection", secondDirection);

            return instruction;
        }

        /// <summary>
        /// Generates a roundabout instruction.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="count"></param>
        /// <param name="nextStreet"></param>
        /// <returns></returns>
        public Instruction GenerateRoundabout(Instruction instruction,
            int count, TagsCollection nextStreet)
        {
            instruction.Text = string.Format("GenerateRoundabout:{0}",
                                             count);

            instruction.Extras = new Dictionary<string, object>();
            instruction.Extras.Add("count", count);
            instruction.Extras.Add("nextStreet", nextStreet);

            return instruction;
        }

        /// <summary>
        /// Generates a simple turn instruction.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Instruction GenerateSimpleTurn(Instruction instruction,
            RelativeDirectionEnum direction)
        {
            instruction.Text = string.Format("GenerateSimpleTurn:{0}",
                                             direction.ToString());

            instruction.Extras = new Dictionary<string, object>();
            instruction.Extras.Add("direction", direction);

            return instruction;
        }
    }
}
