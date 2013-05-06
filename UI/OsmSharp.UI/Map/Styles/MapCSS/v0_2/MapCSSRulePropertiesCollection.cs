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
                        rule = _properties[idx];
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
        /// Returns the ranged properties.
        /// </summary>
        /// <returns></returns>
        public List<MapCSSRuleProperties> GetRanges()
        {
            var rules = new List<MapCSSRuleProperties>();
            MapCSSRuleProperties previousRule = null;
            int zoomLevel = 0;
            while(zoomLevel < 25)
            {
                MapCSSRuleProperties rule = this.GetRulesForZoom(zoomLevel);
                if (rule != null)
                { // the current rule was found.
                    // add the rule.
                    rules.Add(rule);
                    // set the new maximum zoom.
                    zoomLevel = rule.MaxZoom;
                }
                else
                { // manually increase the zoom.
                    zoomLevel++;
                }
            }
            return rules;
        }
    }
}