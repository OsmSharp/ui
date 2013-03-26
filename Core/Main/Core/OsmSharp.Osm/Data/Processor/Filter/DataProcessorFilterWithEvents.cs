using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Data.Core.Processor;

namespace OsmSharp.Osm.Data.Processor.Filter
{
    /// <summary>
    /// A filter that raizes events for each object.
    /// </summary>
    public class DataProcessorFilterWithEvents : DataProcessorFilter
    {
        /// <summary>
        /// An empty delegate.
        /// </summary>
        public delegate void EmptyDelegate();

        /// <summary>
        /// Event raised when filter is initialized.
        /// </summary>
        public event EmptyDelegate InitializeEvent;

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public override void Initialize()
        {
            if (this.InitializeEvent != null)
            {
                this.InitializeEvent();
            }

            this.Source.Initialize();
        }

        /// <summary>
        /// A delegate with a simple osm geo object as parameter.
        /// </summary>
        /// <param name="simple_osm_geo"></param>
        public delegate void SimpleOsmGeoDelegate(Simple.SimpleOsmGeo simple_osm_geo);

        /// <summary>
        /// Event raised when the move is made to the next object.
        /// </summary>
        public event SimpleOsmGeoDelegate MovedToNextEvent;

        /// <summary>
        /// Moves this filter to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            if (this.Source.MoveNext())
            {
                if (this.MovedToNextEvent != null)
                {
                    this.MovedToNextEvent(this.Source.Current());
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override Simple.SimpleOsmGeo Current()
        {
            return this.Source.Current();
        }

        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            this.Source.Reset();
        }

        /// <summary>
        /// Returns true if this filter can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return this.Source.CanReset; }
        }
    }
}
