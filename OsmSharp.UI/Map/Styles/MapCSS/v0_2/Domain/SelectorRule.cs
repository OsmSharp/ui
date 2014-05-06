using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using OsmSharp.Geo.Geometries;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain
{
    /// <summary>
    /// Represents a selector rule for a MapCSS v0.2 Selector class.
    /// </summary>
    public abstract class SelectorRule
    {
        /// <summary>
        /// Invert this rule or not.
        /// </summary>
        public bool Invert { get; set; }

        /// <summary>
        /// Overloads operator && creating a SelectorRuleCombined.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SelectorRule operator &(SelectorRule left, SelectorRule right)
        {
            return new SelectorRuleCombined()
                       {
                           Left = left,
                           Operator = SelectorRuleOperator.And,
                           Right = right
                       };
        }

        /// <summary>
        /// Overloads operator || creating a SelectorRuleCombined.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SelectorRuleCombined operator |(SelectorRule left, SelectorRule right)
        {
            return new SelectorRuleCombined()
            {
                Left = left,
                Operator = SelectorRuleOperator.Or,
                Right = right
            };
        }

        /// <summary>
        /// Returns true if the given object is selected by this selector rule.
        /// </summary>
        /// <param name="mapCSSObject"></param>
        /// <returns></returns>
        internal abstract bool Selects(MapCSSObject mapCSSObject);

        /// <summary>
        /// Adds all relevant keys to the given collection.
        /// </summary>
        /// <param name="relevantKeys"></param>
        internal abstract void AddRelevantKeysTo(ICollection<string> relevantKeys);
    }

    /// <summary>
    /// Combination operators.
    /// </summary>
    public enum SelectorRuleOperator
    {
        /// <summary>
        /// Combine two rules using AND.
        /// </summary>
        And,
        /// <summary>
        /// Combine two rules using OR.
        /// </summary>
        Or
    }

    /// <summary>
    /// Represents a selector rule combining two other rules.
    /// </summary>
    public class SelectorRuleCombined : SelectorRule
    {
        /// <summary>
        /// The left rule.
        /// </summary>
        public SelectorRule Left { get; set; }

        /// <summary>
        /// The operator.
        /// </summary>
        public SelectorRuleOperator Operator { get; set; }

        /// <summary>
        /// The right rule.
        /// </summary>
        public SelectorRule Right { get; set; }
        
        /// <summary>
        /// Returns true if the given object is selected by this selector rule.
        /// </summary>
        /// <param name="mapCSSObject"></param>
        /// <returns></returns>
        internal override bool Selects(MapCSSObject mapCSSObject)
        {
            switch (this.Operator)
            {
                case SelectorRuleOperator.And:
                    return this.Left.Selects(mapCSSObject) && this.Right.Selects(mapCSSObject);
                case SelectorRuleOperator.Or:
                    return this.Left.Selects(mapCSSObject) || this.Right.Selects(mapCSSObject);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Adds all relevant keys to the given collections.
        /// </summary>
        /// <param name="relevantKeys"></param>
        internal override void AddRelevantKeysTo(ICollection<string> relevantKeys)
        {
            this.Right.AddRelevantKeysTo(relevantKeys);
            this.Left.AddRelevantKeysTo(relevantKeys);
        }

        /// <summary>
        /// Returns a description of the selector rule.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}{1}{2}",
                                 this.Left.ToString(),
                                 this.Operator == SelectorRuleOperator.Or ? "||" : string.Empty,
                                 this.Right.ToString());
        }
    }

    /// <summary>
    /// Represents a selector rule that checks the presence of a tag.
    /// </summary>
    public class SelectorRuleTag : SelectorRule
    {
        /// <summary>
        /// The tag that has to be present.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Returns true if the given object is selected by this selector rule.
        /// </summary>
        /// <param name="mapCSSObject"></param>
        /// <returns></returns>
        internal override bool Selects(MapCSSObject mapCSSObject)
        {
            return mapCSSObject.ContainsKey(this.Tag);
        }

        /// <summary>
        /// Adds all relevant keys to the given collections.
        /// </summary>
        /// <param name="relevantKeys"></param>
        internal override void AddRelevantKeysTo(ICollection<string> relevantKeys)
        {
            relevantKeys.Add(this.Tag);
        }

        /// <summary>
        /// Returns a description of the selector rule.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}{1}]",
                                 this.Invert ? "!" : "",
                                 this.Tag);
        }
    }

    /// <summary>
    /// Represents a selector rule that checks the value of a tag.
    /// </summary>
    public class SelectorRuleTagValueComparison : SelectorRuleTag
    {
        /// <summary>
        /// The value of the tag or the regular expression matching it.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The value of the tag or the regular expression matching it.
        /// </summary>
        public SelectorRuleTagValueComparisonEnum Comparator { get; set; }

        /// <summary>
        /// Returns true if the given object is selected by this selector rule.
        /// </summary>
        /// <param name="mapCSSObject"></param>
        /// <returns></returns>
        internal override bool Selects(MapCSSObject mapCSSObject)
        {
            string tagValue;
            if (mapCSSObject.TryGetValue(this.Tag, out tagValue))
            {
                double valueDouble;
                double tagValueDouble;
                switch (this.Comparator)
                {
                    case SelectorRuleTagValueComparisonEnum.Equal:
                        return tagValue == this.Value;
                    case SelectorRuleTagValueComparisonEnum.NotEqual:
                        return tagValue != this.Value;
                    case SelectorRuleTagValueComparisonEnum.GreaterThan:
                        if (double.TryParse(this.Value, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueDouble) &&
                            (double.TryParse(tagValue, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tagValueDouble)))
                        {
                            return tagValueDouble > valueDouble;
                        }
                        break;
                    case SelectorRuleTagValueComparisonEnum.GreaterThanOrEqual:
                        if (double.TryParse(this.Value, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueDouble) &&
                            (double.TryParse(tagValue, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tagValueDouble)))
                        {
                            return tagValueDouble >= valueDouble;
                        }
                        break;
                    case SelectorRuleTagValueComparisonEnum.SmallerThan:
                        if (double.TryParse(this.Value, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueDouble) &&
                            (double.TryParse(tagValue, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tagValueDouble)))
                        {
                            return tagValueDouble < valueDouble;
                        }
                        break;
                    case SelectorRuleTagValueComparisonEnum.SmallerThanOrEqual:
                        if (double.TryParse(this.Value, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valueDouble) &&
                            (double.TryParse(tagValue, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tagValueDouble)))
                        {
                            return tagValueDouble <= valueDouble;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return mapCSSObject.ContainsKey(this.Tag);
            }
            return false;
        }

        /// <summary>
        /// Adds all relevant keys to the given collections.
        /// </summary>
        /// <param name="relevantKeys"></param>
        internal override void AddRelevantKeysTo(ICollection<string> relevantKeys)
        {
            relevantKeys.Add(this.Tag);
        }

        /// <summary>
        /// Returns a description of the selector rule.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string comparator;
            switch (this.Comparator)
            {
                case SelectorRuleTagValueComparisonEnum.GreaterThan:
                    comparator = ">";
                    break;
                case SelectorRuleTagValueComparisonEnum.GreaterThanOrEqual:
                    comparator = ">=";
                    break;
                case SelectorRuleTagValueComparisonEnum.SmallerThan:
                    comparator = "<";
                    break;
                case SelectorRuleTagValueComparisonEnum.SmallerThanOrEqual:
                    comparator = "<=";
                    break;
                case SelectorRuleTagValueComparisonEnum.Equal:
                    comparator = "=";
                    break;
                case SelectorRuleTagValueComparisonEnum.NotEqual:
                    comparator = "!=";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return string.Format("[{0}{1}{2}{3}]",
                                 this.Invert ? "!" : "",
                                 this.Tag,
                                 comparator,
                                 this.Value);
        }

        /// <summary>
        /// Comparator enumeration.
        /// </summary>
        public enum SelectorRuleTagValueComparisonEnum
        {
            Equal,
            NotEqual,
            GreaterThan,
            GreaterThanOrEqual,
            SmallerThan,
            SmallerThanOrEqual
        }
    }
}
