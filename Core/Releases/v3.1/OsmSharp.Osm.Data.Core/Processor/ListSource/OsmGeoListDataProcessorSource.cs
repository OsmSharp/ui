using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Core.Simple;

namespace OsmSharp.Osm.Data.Core.Processor.ListSource
{
    /// <summary>
    /// A data processor source of regular OsmBase objects.
    /// </summary>
    public class OsmGeoListDataProcessorSource : DataProcessorSource
    {
        /// <summary>
        /// Holds the list of OsmBase objects.
        /// </summary>
        private IList<OsmGeo> _base_objects;

        /// <summary>
        /// Holds the current object index.
        /// </summary>
        private int _current;

        /// <summary>
        /// Creates a new OsmBase source.
        /// </summary>
        /// <param name="base_objects"></param>
        public OsmGeoListDataProcessorSource(IList<OsmGeo> base_objects)
        {
            _base_objects = base_objects;
            _current = int.MinValue;
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            this.Reset();
        }

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            _current++;
            return (_current >= 0 &&
                _current < _base_objects.Count);
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override SimpleOsmGeo Current()
        {
            // get the current object.
            OsmGeo osm_object = _base_objects[_current];

            // convert the object.
            return osm_object.ToSimple();
        }

        /// <summary>
        /// Resets this data source.
        /// </summary>
        public override void Reset()
        {
            _current = -1;
        }

        /// <summary>
        /// Returns true, this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return true; }
        }
    }
}
