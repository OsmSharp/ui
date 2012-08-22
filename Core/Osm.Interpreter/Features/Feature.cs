using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Filters;
using Osm.Core;

namespace Osm.Interpreter.Features
{
    /// <summary>
    /// Represents a feature of osm.
    /// </summary>
    public abstract class Feature
    {
        /// <summary>
        /// The parent of this feature.
        /// </summary>
        private Feature _parent;

        /// <summary>
        /// The filter for this feature.
        /// </summary>
        private Filter _filter;

        /// <summary>
        /// The path to get to this feature.
        /// </summary>
        private IList<Feature> _path;

        /// <summary>
        /// The tags to use to build the meta data.
        /// </summary>
        private IDictionary<string,IList<string>> _meta;

        /// <summary>
        /// The unique name of the feature.
        /// </summary>
        private string _name;

        /// <summary>
        /// Creates a new feature.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="filter"></param>
        /// <param name="meta"></param>
        protected Feature(
            string name,
            Filter filter,
            IDictionary<string, IList<string>> meta)
        {
            _name = name;
            _filter = filter;
            _meta = meta;
        }

        #region Properties

        /// <summary>
        /// Returns the unique name of this feature.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #endregion

        #region ChildFeatures

        /// <summary>
        /// Holds all the child features.
        /// </summary>
        private IList<Feature> _child_features;

        /// <summary>
        /// Returns all the child features.
        /// </summary>
        internal IList<Feature> ChildFeatures
        {
            get
            {
                if (_child_features == null)
                {
                    _child_features = this.GetChildFeatures();   

                    // set the parent for each child feature.
                    foreach (Feature feature in _child_features)
                    {
                        feature.SetParent(this);
                    }
                }
                return _child_features;
            }
        }

        /// <summary>
        /// Sets this features parent.
        /// </summary>
        /// <param name="feature"></param>
        private void SetParent(Feature feature)
        {
            _parent = feature;
        }

        /// <summary>
        /// Returns all the child features.
        /// </summary>
        /// <returns></returns>
        internal abstract IList<Feature> GetChildFeatures();

        #endregion

        #region Interpretation

        /// <summary>
        /// Interprets a feature and reports to the client if the feature matches the filter.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool Interpret(
            OsmGeo obj,
            IFeatureClient client)
        {
            if (_filter.Evaluate(obj))
            {
                // add the meta tags.
                Dictionary<string,string> meta = new Dictionary<string,string>();

                foreach(string value in _meta.Keys)
                {
                    meta.Add(value,string.Empty);

                    // check all the tags in the list.
                    foreach(string tag in _meta[value])
                    {
                        if(obj.Tags.ContainsKey(value))
                        {
                            string tag_value = obj.Tags["tag"];

                            if(tag_value != null && tag_value.Length > 0)
                            {
                                meta[value] = tag_value;
                            }
                        }
                    }
                }

                // report the interpretation succes.
                client.InterpretationSucces(_path, obj, meta);

                return true;
            }
            return false;
        }

        #endregion
    }
}
