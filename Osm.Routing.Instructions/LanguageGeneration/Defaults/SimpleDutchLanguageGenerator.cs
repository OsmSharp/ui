using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo.Meta;
using Osm.Routing.Core.ArcAggregation.Output;

namespace Osm.Routing.Instructions.LanguageGeneration.Defaults
{
    internal class SimpleDutchLanguageGenerator : ILanguageGenerator
    {
        private string TurnDirection(RelativeDirectionEnum direction)
        {
            switch (direction)
            {
                case RelativeDirectionEnum.Right:
                case RelativeDirectionEnum.SharpRight:
                case RelativeDirectionEnum.SlightlyRight:
                    return "rechts";
                case RelativeDirectionEnum.Left:
                case RelativeDirectionEnum.SharpLeft:
                case RelativeDirectionEnum.SlightlyLeft:
                    return "links";
                case RelativeDirectionEnum.TurnBack:
                    return "terug";
            }
            return string.Empty;
        }

        #region ILanguageGenerator Members

        public Instruction GenerateDirectTurn(Instruction instruction, int street_count_before_turn,
            List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum direction, List<PointPoi> list)
        {
            if (street_count_before_turn == 1)
            {
                instruction.Text = string.Format("Neem de 1ste afslag {0}, de {1} op.",
                    TurnDirection(direction),
                    this.GetName("nl", street_to));
            }
            else
            {
                instruction.Text = string.Format("Neem de {0}de afslag {1}, de {2} op.",
                    street_count_before_turn,
                    TurnDirection(direction),
                    this.GetName("nl", street_to));
            }

            // returns the instruction with text.
            return instruction;
        }

        public Instruction GenerateIndirectTurn(Instruction instruction, int street_count_turn, int street_count_before_turn,
            List<KeyValuePair<string, string>> street_to, RelativeDirectionEnum direction, List<PointPoi> list)
        {
            instruction.Text = string.Format("Neem de {0}de afslag {1}, de {2} op.",
                street_count_before_turn,
                TurnDirection(direction),
                this.GetName("nl", street_to));

            // returns the instruction with text.
            return instruction;
        }

        public Instruction GeneratePoi(Instruction instruction, List<PointPoi> list, RelativeDirectionEnum? direction)
        {
            if (direction == null)
            {
                instruction.Text = string.Format("Poi");
            }
            else
            {
                instruction.Text = string.Format("Poi:{0}", direction);
            }

            // returns the instruction with text.
            return instruction;
        }

        public Instruction GenerateDirectFollowTurn(Instruction instruction, int street_count_before_turn, List<KeyValuePair<string, string>> street_to, 
            RelativeDirectionEnum direction, List<PointPoi> list)
        {
            if (street_count_before_turn == 1)
            {
                instruction.Text = string.Format("Sla {1}af om op {0} te blijven.",
                    this.GetName("nl", street_to),
                    TurnDirection(direction));
            }
            else
            {
                instruction.Text = string.Format("Neem de {1}de straat {2} om op {0} te blijven.",
                    this.GetName("nl", street_to),
                    street_count_before_turn,
                    TurnDirection(direction));
            }

            // returns the instruction with text.
            return instruction;
        }

        public Instruction GenerateIndirectFollowTurn(Instruction instruction, int street_count_turn, int street_count_before_turn, List<KeyValuePair<string, string>> street_to, 
            RelativeDirectionEnum direction, List<PointPoi> list)
        {
            if (street_count_before_turn == 1)
            {
                instruction.Text = string.Format("Sla {1}af om op {0} te blijven.",
                    this.GetName("nl",  street_to),
                    TurnDirection(direction));
            }
            else
            {
                instruction.Text = string.Format("Neem de {1}de straat {2} om op {0} te blijven.",
                    this.GetName("nl", street_to),
                    street_count_before_turn,
                    TurnDirection(direction));
            }

            // returns the instruction with text.
            return instruction;
        }

        public Instruction GenerateImmidiateTurn(Instruction instruction, int first_street_count_to, List<KeyValuePair<string, string>> first_street_to,
            RelativeDirection first_direction, List<KeyValuePair<string, string>> second_street_to, RelativeDirection second_direction)
        {
            if (first_street_count_to == 1)
            {
                instruction.Text = string.Format("Neem de 1ste afslag {0}, de {1} op, en ga onmiddellijk {2} op de {3}.",
                    TurnDirection(first_direction.Direction),
                    this.GetName("nl", first_street_to),
                    TurnDirection(second_direction.Direction),
                    this.GetName("nl", second_street_to));
            }
            else
            {
                instruction.Text = string.Format("Neem de {4}de afslag {0}, de {1} op, en ga onmiddellijk {2} op de {3}.",
                    TurnDirection(first_direction.Direction),
                    this.GetName("nl", first_street_to),
                    TurnDirection(second_direction.Direction),
                    this.GetName("nl", second_street_to),
                    first_street_count_to);
            }

            // returns the instruction with text.
            return instruction;
        }

        public Instruction GenerateRoundabout(Instruction instruction, int count, List<KeyValuePair<string, string>> next_street)
        {
            instruction.Text = string.Format("Neem de {0}de afslag op het volgende rondpunt naar de {1}.",
                count,
                this.GetName("nl", next_street));

            // returns the instruction with text.
            return instruction;
        }

        public Instruction GenerateSimpleTurn(Instruction instruction, RelativeDirectionEnum direction)
        {
            instruction.Text = string.Format("Draai {0}", this.TurnDirection(direction));

            return instruction;
        }

        #endregion

        private string GetName(string language_key, List<KeyValuePair<string, string>> tags)
        {
            language_key = language_key.ToLower();

            string name = string.Empty;
            foreach (KeyValuePair<string, string> tag in tags)
            {
                if (tag.Key != null && tag.Key.ToLower() == string.Format("name:{0}", language_key))
                {
                    return tag.Value;
                }
                if (tag.Key != null && tag.Key.ToLower() == "name")
                {
                    name = tag.Key;
                }
            }
            return name;
        }
    }
}
