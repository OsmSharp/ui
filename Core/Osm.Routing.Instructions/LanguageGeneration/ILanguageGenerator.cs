using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo.Meta;

namespace Osm.Routing.Instructions.LanguageGeneration
{
    public interface ILanguageGenerator
    {
        Instruction GenerateDirectTurn(Instruction instruction, int street_count_before_turn, List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum direction, List<Osm.Routing.Core.ArcAggregation.Output.PointPoi> list);

        Instruction GenerateIndirectTurn(Instruction instruction, int street_count_turn, int street_count_before_turn, List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum direction, List<Osm.Routing.Core.ArcAggregation.Output.PointPoi> list);

        Instruction GeneratePoi(Instruction instruction, List<Osm.Routing.Core.ArcAggregation.Output.PointPoi> list, RelativeDirectionEnum? direction);

        Instruction GenerateDirectFollowTurn(Instruction instruction, int street_count_before_turn, List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum relativeDirectionEnum, List<Osm.Routing.Core.ArcAggregation.Output.PointPoi> list);

        Instruction GenerateIndirectFollowTurn(Instruction instruction, int street_count_turn, int street_count_before_turn, List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum relativeDirectionEnum, List<Osm.Routing.Core.ArcAggregation.Output.PointPoi> list);

        Instruction GenerateImmidiateTurn(Instruction instruction, int first_street_count_to, List<KeyValuePair<string, string>> first_street_to, RelativeDirection first_direction, List<KeyValuePair<string, string>> second_street_to, RelativeDirection second_direction);

        Instruction GenerateRoundabout(Instruction instruction, int count, List<KeyValuePair<string, string>> next_street);

        Instruction GenerateSimpleTurn(Instruction direction_instruction, RelativeDirectionEnum direction);
    }
}
