using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;

namespace Osm.Interpreter.Features
{
    /// <summary>
    /// Represents a feature client.
    /// </summary>
    public interface IFeatureClient
    {
        /// <summary>
        /// Called when interpretation is about to start.
        /// </summary>
        /// <param name="obj"></param>
        void OnBeforeInterpretation(OsmGeo obj);

        /// <summary>
        /// Called when a feature interprets an object succesfully.
        /// </summary>
        /// <param name="features_path"></param>
        /// <param name="obj"></param>
        /// <param name="meta">Some extra info the feature might want to include.</param>
        void InterpretationSucces(
            IList<Feature> features_path,
            OsmGeo obj,
            IDictionary<string,string> meta);

        /// <summary>
        /// Called when interpretation is finished.
        /// </summary>
        /// <param name="obj"></param>
        void OnAfterInterpretation(bool succes,OsmGeo obj);
    }
}
