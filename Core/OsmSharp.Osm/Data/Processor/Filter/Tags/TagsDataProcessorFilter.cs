using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Simple;

namespace OsmSharp.Osm.Data.Processor.Filter.Tags
{
    /// <summary>
    /// A data processor filter that filters objects by their tags.
    /// </summary>
    public class TagsDataProcessorFilter : DataProcessorFilter
    {
        /// <summary>
        /// Holds the nodes filter.
        /// </summary>
        private Filters.Filter _nodesFilter;

        /// <summary>
        /// Holds the ways filter.
        /// </summary>
        private Filters.Filter _waysFilter;

        /// <summary>
        /// Keeps the nodes in the ways also.
        /// </summary>
        private readonly bool _wayKeepNodes;

        /// <summary>
        /// Holds the relations filter.
        /// </summary>
        private Filters.Filter _relationsFilter;

        /// <summary>
        /// Keeps the objects in the relations also.
        /// </summary>
        private readonly bool _relationKeepObjects;

        /// <summary>
        /// Filters data according to the given filters.
        /// </summary>
        /// <param name="nodesFilter"></param>
        /// <param name="waysFilter"></param>
        /// <param name="relationsFilter"></param>
        public TagsDataProcessorFilter(Filters.Filter nodesFilter, Filters.Filter waysFilter,
                                       Filters.Filter relationsFilter)
        {
            _nodesFilter = nodesFilter;
            _waysFilter = waysFilter;
            _relationsFilter = relationsFilter;

            _wayKeepNodes = false;
            _relationKeepObjects = false;
        }

        ///// <summary>
        ///// Filters data according to the given filters.
        ///// </summary>
        ///// <param name="nodesFilter"></param>
        ///// <param name="waysFilter"></param>
        ///// <param name="relationsFilter"></param>
        ///// <param name="wayKeepNodes"></param>
        ///// <param name="relationKeepObjects"></param>
        //public TagsDataProcessorFilter(Filters.Filter nodesFilter, Filters.Filter waysFilter,
        //                               Filters.Filter relationsFilter, bool wayKeepNodes, bool relationKeepObjects)
        //{
        //    _nodesFilter = nodesFilter;
        //    _waysFilter = waysFilter;
        //    _relationsFilter = relationsFilter;

        //    _wayKeepNodes = wayKeepNodes;
        //    _relationKeepObjects = relationKeepObjects;
        //}

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// Holds the current object.
        /// </summary>
        private SimpleOsmGeo _current;

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            if (!_relationKeepObjects && !_wayKeepNodes)
            { // simple here just filter!
                bool filter_ok = false;
                while (!filter_ok)
                {
                    if (this.Source.MoveNext())
                    {
                        SimpleOsmGeo current = this.Source.Current();

                        switch (current.Type)
                        {
                            case SimpleOsmGeoType.Node:
                                if (_nodesFilter == null ||
                                    _nodesFilter.Evaluate(current))
                                {
                                    _current = current;
                                    return true;
                                }
                                break;
                            case SimpleOsmGeoType.Way:
                                if (_waysFilter == null ||
                                    _waysFilter.Evaluate(current))
                                {
                                    _current = current;
                                    return true;
                                }
                                break;
                            case SimpleOsmGeoType.Relation:
                                if (_relationsFilter == null ||
                                    _relationsFilter.Evaluate(current))
                                {
                                    _current = current;
                                    return true;
                                }
                                break;
                        }
                    }
                    else
                    { // there is no more data in the source!
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override SimpleOsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            _current = null;

            this.Source.Reset();
        }

        /// <summary>
        /// Returns true if this filter can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return this.Source.CanReset; }
        }

        /// <summary>
        /// Registers the source of this filter.
        /// </summary>
        /// <param name="source"></param>
        public override void RegisterSource(DataProcessorSource source)
        {
            if (_wayKeepNodes || _relationKeepObjects)
            {
                if (!source.CanReset)
                {
                    throw new ArgumentException("The tags data processor source cannot be reset!",
                        "source");
                }
            }

            base.RegisterSource(source);
        }
    }
}
