using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor.Filter
{
    public class DataProcessorFilterBoundingBox : DataProcessorFilter
    {
        private SimpleOsmGeoType _current_type = SimpleOsmGeoType.Node;

        private bool _include_extra_mode = false;

        private GeoCoordinateBox _box;

        private LongIndex.LongIndex _nodes_in = new LongIndex.LongIndex();
        private HashSet<long> _ways_in = new HashSet<long>();
        private HashSet<long> _relation_in = new HashSet<long>();

        private LongIndex.LongIndex _nodes_to_include = new LongIndex.LongIndex();
        private HashSet<long> _relations_to_include = new HashSet<long>();
        private HashSet<long> _ways_to_include = new HashSet<long>();

        public DataProcessorFilterBoundingBox(GeoCoordinateBox box)
            : base()
        {
            _box = box;
        }

        public override void Initialize()
        {
            this.Source.Initialize();
        }

        public override bool MoveNext()
        {
            if (!_include_extra_mode)
            { // just go over all nodes and ways.
                if (this.Source.MoveNext())
                {
                    bool finished = false;
                    bool is_in = false;

                    // move to the next object of the current type.
                    while (this.Current().Type != _current_type)
                    {
                        if (!this.Source.MoveNext())
                        {
                            finished = true;
                            break;
                        }
                    }
                    while (this.Current().Type == _current_type && !is_in)
                    {
                        SimpleOsmGeo current = this.Source.Current();
                        is_in = this.IsInBB(current);
                        if (is_in)
                        {
                            switch (current.Type)
                            {
                                case SimpleOsmGeoType.Node:
                                    _nodes_in.Add(current.Id.Value);
                                    break;
                                case SimpleOsmGeoType.Way:
                                    _ways_in.Add(current.Id.Value);
                                    break;
                                case SimpleOsmGeoType.Relation:
                                    _relation_in.Add(current.Id.Value);
                                    break;
                            }
                            break;
                        }

                        // move to the next object of the current type.
                        if (!this.Source.MoveNext())
                        {
                            finished = true;
                            break;
                        }
                        while (this.Current().Type != _current_type)
                        {
                            if (!this.Source.MoveNext())
                            {
                                finished = true;
                                break;
                            }
                        }
                        if (finished)
                        {
                            break;
                        }
                    }

                    if (!finished && this.Current().Type == _current_type)
                    { // nothing was finished and the types match.
                        return true;
                    }
                }

                switch (_current_type)
                {
                    case SimpleOsmGeoType.Node:
                        this.Source.Reset();
                        _current_type = SimpleOsmGeoType.Way;
                        return this.MoveNext();
                    case SimpleOsmGeoType.Way:
                        this.Source.Reset();
                        _current_type = SimpleOsmGeoType.Relation;
                        return this.MoveNext();
                    case SimpleOsmGeoType.Relation:
                        this.Source.Reset();
                        _include_extra_mode = true;
                        return this.MoveNext();
                }
                throw new InvalidOperationException("Unkown SimpleOsmGeoType");
            }
            else
            {
                while (this.Source.MoveNext())
                {
                    switch (this.Source.Current().Type)
                    {
                        case SimpleOsmGeoType.Node:
                            if (_nodes_to_include.Contains(this.Source.Current().Id.Value))
                            {
                                if (!_nodes_in.Contains(this.Source.Current().Id.Value))
                                {
                                    return true;
                                }
                            }
                            break;
                        case SimpleOsmGeoType.Way:
                            if (_ways_to_include.Contains(this.Source.Current().Id.Value))
                            {
                                if (!_ways_in.Contains(this.Source.Current().Id.Value))
                                {
                                    return true;
                                }
                            }
                            break;
                        case SimpleOsmGeoType.Relation:
                            if (_relations_to_include.Contains(this.Source.Current().Id.Value))
                            {
                                if (!_relation_in.Contains(this.Source.Current().Id.Value))
                                {
                                    return true;
                                }
                            }
                            break;
                    }
                    this.Source.MoveNext();
                }
                return false;
            }
        }

        public override SimpleOsmGeo Current()
        {
            return this.Source.Current();
        }

        public override void Reset()
        {
            _ways_in.Clear();
            _nodes_in.Clear();
            _current_type = SimpleOsmGeoType.Node;
            _include_extra_mode = false;
            this.Source.Reset();
        }

        HashSet<long> _relations_considered = new HashSet<long>();

        private bool IsInBB(SimpleOsmGeo osm_geo)
        {
            bool is_in = false;
            switch (osm_geo.Type)
            {
                case SimpleOsmGeoType.Node:
                    is_in = _box.IsInside(new GeoCoordinate(
                        (osm_geo as SimpleNode).Latitude.Value,
                        (osm_geo as SimpleNode).Longitude.Value));
                    break;
                case SimpleOsmGeoType.Way:
                    foreach (long node_id in (osm_geo as SimpleWay).Nodes)
                    {
                        if (_nodes_in.Contains(node_id))
                        {
                            is_in = true;
                            break;
                        }
                    }
                    if (is_in)
                    {                   
                        foreach (long node_id in (osm_geo as SimpleWay).Nodes)
                        {
                            _nodes_to_include.Add(node_id);
                        }
                    }
                    break;
                case SimpleOsmGeoType.Relation:
                    if (!_relations_considered.Contains(osm_geo.Id.Value))
                    {
                        foreach (SimpleRelationMember member in (osm_geo as SimpleRelation).Members)
                        {
                            switch (member.MemberType.Value)
                            {
                                case SimpleRelationMemberType.Node:
                                    if (_nodes_in.Contains(member.MemberId.Value))
                                    {
                                        is_in = true;
                                        break;
                                    }
                                    break;
                                case SimpleRelationMemberType.Way:
                                    if (_ways_in.Contains(member.MemberId.Value))
                                    {
                                        is_in = true;
                                        break;
                                    }
                                    break;
                                case SimpleRelationMemberType.Relation:
                                    if (_relation_in.Contains(member.MemberId.Value))
                                    {
                                        is_in = true;
                                        break;
                                    }
                                    break;
                            }
                        }

                        if (is_in)
                        {
                            foreach (SimpleRelationMember member in (osm_geo as SimpleRelation).Members)
                            {
                                switch (member.MemberType.Value)
                                {
                                    case SimpleRelationMemberType.Node:
                                        _nodes_to_include.Add(member.MemberId.Value);
                                        break;
                                    case SimpleRelationMemberType.Way:
                                        _ways_to_include.Add(member.MemberId.Value);
                                        break;
                                    case SimpleRelationMemberType.Relation:
                                        _relations_to_include.Add(member.MemberId.Value);
                                        break;
                                }
                            }
                        }
                    }
                    break;
            }
            return is_in;
        }
    }
}
