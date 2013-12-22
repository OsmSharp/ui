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
            int minZoom = 0, maxZoom = 20;
            for (int zoomLevel = 0; zoomLevel < 20; zoomLevel++)
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
                        currentRule = null;
                    }

                    if (!string.IsNullOrWhiteSpace(currentRuleString))
                    { // only do this part when string is not empty.
                        minZoom = zoomLevel; // set the min zoom.
                        MapCSSRuleProperties props = this.GetRulesForZoom(zoomLevel);
                        if (props != null)
                        {
                            currentRule = new MapCSSRuleProperties(minZoom, 20);
                            currentRule = currentRule.Merge(props);
                        }
                    }
                    previousRuleString = currentRuleString;
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