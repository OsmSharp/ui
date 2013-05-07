using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2
{
    class MapCSSRulePropertiesCollection
    {
        /// <summary>
        /// Holds properties lists.
        /// </summary>
        private readonly List<MapCSSRuleProperties> _properties; 

        /// <summary>
        /// Creates a new collection of properties.
        /// </summary>
        public MapCSSRulePropertiesCollection()
        {
            _properties = new List<MapCSSRuleProperties>();    
        }

        /// <summary>
        /// Adds the given properties to the ones already here.
        /// </summary>
        /// <param name="properties"></param>
        public void AddProperties(MapCSSRuleProperties properties)
        {
            _properties.Add(properties);
        }

        /// <summary>
        /// Returns the rules for the given zoom.
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public MapCSSRuleProperties GetRulesForZoom(int zoom)
        {
            MapCSSRuleProperties rule = null;
            for (int idx = 0; idx < _properties.Count; idx++)
            {
                if (_properties[idx].IsForZoom(zoom))
                {
                    if (rule == null)
                    {
                        rule = new MapCSSRuleProperties(_properties[idx].MinZoom, 
                            _properties[idx].MaxZoom);
                        rule = rule.Merge(_properties[idx]);
                    }
                    else
                    {
                        rule = rule.Merge(_properties[idx]);
                    }
                }
            }
            return rule;
        }

        /// <summary>
        /// Returns the rule string for the given zoom.
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private string GetRuleStringForZoom(int zoom)
        {
            var ruleString = new StringBuilder();
            for (int idx = 0; idx < _properties.Count; idx++)
            {
                if (_properties[idx].IsForZoom(zoom))
                {
                    ruleString.Append(
                        idx.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    ruleString.Append('/');
                }
            }
            return ruleString.ToString();
        }

        /// <summary>
        /// Returns the ranged properties.
        /// </summary>
        /// <returns></returns>
        public List<MapCSSRuleProperties> GetRanges()
        {
            var rules = new List<MapCSSRuleProperties>();
            MapCSSRuleProperties currentRule = null;
            string previousRuleString = string.Empty;
            int minZoom = 0, maxZoom = 25;
            for (int zoomLevel = 0; zoomLevel < 25; zoomLevel++)
            {
                // get the current rule string.
                string currentRuleString = this.GetRuleStringForZoom(zoomLevel);

                if (previousRuleString != currentRuleString)
                { // there is a new rule.
                    // store the previous rule.
                    if (currentRule != null)
                    { // the current rule exists; store it.
                        currentRule.MinZoom = minZoom;
                        currentRule.MaxZoom = maxZoom + 1;

                        rules.Add(currentRule);
                    }

                    minZoom = zoomLevel; // set the min zoom.
                    MapCSSRuleProperties props = this.GetRulesForZoom(zoomLevel);
                    if (props != null)
                    {
                        currentRule = new MapCSSRuleProperties(minZoom, 25);
                        currentRule = currentRule.Merge(props);
                        previousRuleString = currentRuleString;
                    }
                }
                maxZoom = zoomLevel; // set the max zoom.
            }

            // store the previous rule.
            if (currentRule != null)
            { // the current rule exists; store it.
                currentRule.MinZoom = minZoom;
                currentRule.MaxZoom = maxZoom + 1;

                rules.Add(currentRule);
            }

            return rules;
        }
    }
}