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
using Tools.Math.Geo.Meta;

namespace Routing.Instructions.LanguageGeneration
{
    public interface ILanguageGenerator
    {
        Instruction GenerateDirectTurn(Instruction instruction, int street_count_before_turn, List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum direction, List<Routing.Core.ArcAggregation.Output.PointPoi> list);

        Instruction GenerateIndirectTurn(Instruction instruction, int street_count_turn, int street_count_before_turn, List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum direction, List<Routing.Core.ArcAggregation.Output.PointPoi> list);

        Instruction GeneratePoi(Instruction instruction, List<Routing.Core.ArcAggregation.Output.PointPoi> list, RelativeDirectionEnum? direction);

        Instruction GenerateDirectFollowTurn(Instruction instruction, int street_count_before_turn, List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum relativeDirectionEnum, List<Routing.Core.ArcAggregation.Output.PointPoi> list);

        Instruction GenerateIndirectFollowTurn(Instruction instruction, int street_count_turn, int street_count_before_turn, List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum relativeDirectionEnum, List<Routing.Core.ArcAggregation.Output.PointPoi> list);

        Instruction GenerateImmidiateTurn(Instruction instruction, int first_street_count_to, List<KeyValuePair<string, string>> first_street_to, RelativeDirection first_direction, List<KeyValuePair<string, string>> second_street_to, RelativeDirection second_direction);

        Instruction GenerateRoundabout(Instruction instruction, int count, List<KeyValuePair<string, string>> next_street);

        Instruction GenerateSimpleTurn(Instruction direction_instruction, RelativeDirectionEnum direction);
    }
}
