using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Interpreter.Features;
using Osm.Map.Layers;
using Osm.Core;

namespace Osm.Map.Styles
{
    /// <summary>
    /// Represents a style of a datasource.
    /// 
    /// A style is a way of transforming features (for osm objects) into map elements.
    /// </summary>
    public class OsmStyleInterpreter : IFeatureClient
    {
        /// <summary>
        /// Holds the layer this style if for.
        /// </summary>
        private DataSourceLayer _layer;

        /// <summary>
        /// Holds the style information.
        /// </summary>
        private OsmStyle _style;

        /// <summary>
        /// Creates a new style for a datasoruce layer.
        /// </summary>
        /// <param name="layer"></param>
        public OsmStyleInterpreter(DataSourceLayer layer)
        {
            _layer = layer;
        }


        #region IFeatureClient Members

        /// <summary>
        /// Before interpretation; remove the object.
        /// </summary>
        /// <param name="obj"></param>
        public void OnBeforeInterpretation(OsmGeo obj)
        {
            _layer.RemoveElements(obj);
        }

        /// <summary>
        /// Interpretation of the given object succeeded!
        /// </summary>
        /// <param name="features_path"></param>
        /// <param name="obj"></param>
        /// <param name="meta"></param>
        public void InterpretationSucces(
            IList<Feature> features_path, 
            OsmGeo obj, 
            IDictionary<string, string> meta)
        {

        }

        /// <summary>
        /// After interpretation; if not succesfull report!
        /// </summary>
        /// <param name="succes"></param>
        /// <param name="obj"></param>
        public void OnAfterInterpretation(bool succes, OsmGeo obj)
        {
            // TODO: report unsuccesfull interpretation.
        }

        #endregion
    }
}
