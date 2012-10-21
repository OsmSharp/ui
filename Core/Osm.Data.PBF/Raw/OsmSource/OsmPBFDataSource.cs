using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data;
using Osm.Core.Sources;
using System.IO;
using Tools.Math.Geo;
using Osm.Core.Filters;
using Osm.Core.Factory;

namespace Osm.Data.PBF.Raw.OsmSource
{
    /// <summary>
    /// A datasource accepting a PBF OSM formatted stream/file.
    /// </summary>
    public class OsmPBFDataSource : IDataSource,
        INodeSource,
        IWaySource,
        IRelationSource
    {
        /// <summary>
        /// The stream containing the data.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// The id of this data source.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Creates a new osm data source.
        /// </summary>
        /// <param name="stream"></param>
        public OsmPBFDataSource(Stream stream)
        {
            _stream = stream;
            _id = Guid.NewGuid();

            _read = false;
            _nodes = new Dictionary<long, Osm.Core.Node>();
            _ways = new Dictionary<long, Osm.Core.Way>();
            _relations = new Dictionary<long, Osm.Core.Relation>();

            _ways_per_node = new Dictionary<long, List<long>>();
            _relations_per_member = new Dictionary<long, List<long>>();
            _closed_change_set = new List<long>();
        }

        #region Write/Read functions

        // hold all node, ways, relations and changesets and their bounding box.
        private IDictionary<long, Osm.Core.Node> _nodes;
        private IDictionary<long, Osm.Core.Way> _ways;
        private IDictionary<long, Osm.Core.Relation> _relations;
        private IList<long> _closed_change_set;
        private IDictionary<long, List<long>> _ways_per_node;
        private IDictionary<long, List<long>> _relations_per_member;
        private GeoCoordinateBox _bb;

        private bool _read;

        /// <summary>
        /// Adds the node-way relations for the given way.
        /// </summary>
        /// <param name="way"></param>
        private void RegisterNodeWayRelation(Osm.Core.Way way)
        {
            foreach (Osm.Core.Node node in way.Nodes)
            {
                if (!_ways_per_node.ContainsKey(node.Id))
                {
                    _ways_per_node.Add(node.Id, new List<long>());
                }
                _ways_per_node[node.Id].Add(way.Id);
            }
        }

        /// <summary>
        /// Adds the member-relation for the given relation.
        /// </summary>
        /// <param name="relation"></param>
        private void RegisterRelationMemberRelation(Osm.Core.Relation relation)
        {
            foreach (Osm.Core.RelationMember member in relation.Members)
            {
                if (!_relations_per_member.ContainsKey(member.Member.Id))
                {
                    _relations_per_member.Add(member.Member.Id, new List<long>());
                }
                _relations_per_member[member.Member.Id].Add(relation.Id);
            }
        }

        /// <summary>
        /// Registers a closed change set id.
        /// </summary>
        /// <param name="change_set_id"></param>
        private void RegisterChangeSetId(long? change_set_id)
        {
            if (change_set_id.HasValue
                && !_closed_change_set.Contains(change_set_id.Value))
            {
                _closed_change_set.Add(change_set_id.Value);
            }
        }

        /// <summary>
        /// Reads all the data from the osm document if needed.
        /// </summary>
        private void ReadFromDocument()
        {
            if (!_read)
            {
                _read = true;

            }
        }

        /// <summary>
        /// Writes to the osm document.
        /// </summary>
        private void WriteToDocument()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDataSource Members

        /// <summary>
        /// Gets the bounding box around the data in this data source.
        /// </summary>
        public GeoCoordinateBox BoundingBox
        {
            get 
            {
                this.ReadFromDocument();

                if (_bb == null)
                { // calculate bounding box.
                    throw new NotSupportedException("There is no boundingbox in this datafile!");
                }
                return _bb;
            }
        }

        /// <summary>
        /// Returns the name of this document.
        /// </summary>
        public string Name
        {
            get 
            {
                return string.Format("PBF Source {0}", _id.ToString());
            }
        }

        /// <summary>
        /// Returns the id of this data source.
        /// </summary>
        public Guid Id
        {
            get 
            {
                return _id;
            }
        }

        /// <summary>
        /// Returns true; a bounding box can always be calculated.
        /// </summary>
        public bool HasBoundinBox
        {
            get 
            {
                this.ReadFromDocument();

                return _bb != null;
            }
        }

        /// <summary>
        /// Returns false; this source cannot generate native id's.
        /// </summary>
        public bool IsBaseIdGenerator
        {
            get 
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if this source can create new objects.
        /// </summary>
        public bool IsCreator
        {
            get 
            {
                return true;
            }
        }

        /// <summary>
        /// Returns true if this source is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get 
            {
                return true;
            }
        }

        /// <summary>
        /// Returns false; data is not persisted live.
        /// </summary>
        public bool IsLive
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Persists all the data in this source.
        /// </summary>
        public void Persist()
        {
            if (!this.IsReadOnly)
            { // persist the data.
                // generate and write to the xml document.
                this.WriteToDocument();
            }
            else
            {
                throw new InvalidOperationException("Cannot persist a readonly datasource!");
            }
        }

        /// <summary>
        /// Applies the given changeset to the data in this datasource.
        /// </summary>
        /// <param name="change_set"></param>
        public void ApplyChangeSet(Osm.Core.ChangeSet change_set)
        {
            // test if the changeset was not already applied.
            if (_closed_change_set.Contains(change_set.Id))
            {
                throw new InvalidOperationException("Cannot apply an already closed changeset!");
            }

            // change the objects in the changeset.
            foreach (Osm.Core.Change change in change_set.Changes)
            {
                switch (change.Type)
                {
                    case Osm.Core.ChangeType.Create:
                        // set the changeset and version field.
                        change.Object.ChangeSetId = change_set.Id;
                        change.Object.Version = 0;

                        switch (change.Object.Type)
                        {
                            case Osm.Core.OsmType.Node:
                                this.AddNode(change.Object as Osm.Core.Node);
                                break;
                            case Osm.Core.OsmType.Relation:
                                this.AddRelation(change.Object as Osm.Core.Relation);
                                break;
                            case Osm.Core.OsmType.Way:
                                this.AddWay(change.Object as Osm.Core.Way);
                                break;
                        }
                        break;
                    case Osm.Core.ChangeType.Delete:

                        switch (change.Object.Type)
                        {
                            case Osm.Core.OsmType.Node:
                                this.RemoveNode(change.Object as Osm.Core.Node);
                                break;
                            case Osm.Core.OsmType.Relation:
                                this.RemoveRelation(change.Object as Osm.Core.Relation);
                                break;
                            case Osm.Core.OsmType.Way:
                                this.RemoveWay(change.Object as Osm.Core.Way);
                                break;
                        }
                        this.RegisterChangeSetId(change_set.Id);
                        break;
                    case Osm.Core.ChangeType.Modify:

                        // update the changeset field and the version field.
                        change.Object.ChangeSetId = change_set.Id;
                        if (change.Object.Version.HasValue)
                        {
                            change.Object.Version =  change.Object.Version.Value + 1;
                        }

                        switch (change.Object.Type)
                        {
                            case Osm.Core.OsmType.Node:
                                _nodes[change.Object.Id] = (change.Object as Osm.Core.Node);
                                break;
                            case Osm.Core.OsmType.Relation:
                                _relations[change.Object.Id] = (change.Object as Osm.Core.Relation);

                                // update the relations data.
                                IList<long> old_members_to_remove = new List<long>();
                                foreach (KeyValuePair<long, List<long>> pair in _relations_per_member)
                                {
                                    if (pair.Value.Contains(change.Object.Id))
                                    {
                                        pair.Value.Remove(change.Object.Id);
                                        // remove the old members that are only used in this relation.
                                        if (pair.Value.Count == 0)
                                        {
                                            old_members_to_remove.Add(pair.Key);
                                        }
                                    }
                                }
                                foreach (int old_member in old_members_to_remove)
                                {
                                    _relations_per_member.Remove(old_member);
                                }

                                // re-index the relation.
                                this.RegisterRelationMemberRelation(change.Object as Osm.Core.Relation);
                                break;
                            case Osm.Core.OsmType.Way:
                                _ways[change.Object.Id] = (change.Object as Osm.Core.Way);

                                // update the way-node relation data.
                                IList<long> old_nodes_to_remove = new List<long>();
                                foreach (KeyValuePair<long, List<long>> pair in _ways_per_node)
                                {
                                    if (pair.Value.Contains(change.Object.Id))
                                    {
                                        pair.Value.Remove(change.Object.Id);

                                        // remove the old nodes that are only used in this way.
                                        if (pair.Value.Count == 0)
                                        {
                                            old_nodes_to_remove.Add(pair.Key);
                                        }
                                    }
                                }
                                foreach (long old_node in old_nodes_to_remove)
                                {
                                    _ways_per_node.Remove(old_node);
                                }

                                // re-index the relation.
                                this.RegisterNodeWayRelation(change.Object as Osm.Core.Way);

                                // remove unused nodes.
                                // => nodes that are not re-indexed!
                                foreach (long old_node in old_nodes_to_remove)
                                {
                                    if (!_ways_per_node.ContainsKey(old_node))
                                    {
                                        this.RemoveNode(this.GetNode(old_node));
                                    }
                                    _ways_per_node.Remove(old_node);
                                }
                                break;
                        }
                        this.RegisterChangeSetId(change_set.Id);
                        break;
                }
            }
        }

        /// <summary>
        /// Creates a new changeset.
        /// </summary>
        /// <returns></returns>
        public Osm.Core.ChangeSet CreateChangeSet()
        {
            return OsmBaseFactory.CreateChangeSet(KeyGenerator.GenerateNew());
        }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <returns></returns>
        public Osm.Core.Node CreateNode()
        {
            return OsmBaseFactory.CreateNode(KeyGenerator.GenerateNew());
        }

        /// <summary>
        /// Returns the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Osm.Core.Node GetNode(long id)
        {
            this.ReadFromDocument();

            if (_nodes.ContainsKey(id))
            {
                return _nodes[id];
            }
            return null;
        }

        /// <summary>
        /// Returns an enumerable of all nodes in the data source.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Osm.Core.Node> GetNodes()
        {
            this.ReadFromDocument();

            return _nodes.Values;
        }

        /// <summary>
        /// Returns the nodes with the id's in the ids list.
        /// 
        /// The returned list will have the same size as the original
        /// and the returned nodes will be in the same position as their id's.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Osm.Core.Node> GetNodes(IList<long> ids)
        {
            IList<Osm.Core.Node> ret_list = new List<Osm.Core.Node>(ids.Count);

            for(int idx = 0; idx < ids.Count; idx++)
            {
                long id = ids[idx];
                ret_list.Add(this.GetNode(id));
            }

            return ret_list;
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <returns></returns>
        public Osm.Core.Relation CreateRelation()
        {
            return OsmBaseFactory.CreateRelation(KeyGenerator.GenerateNew());
        }

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Osm.Core.Relation GetRelation(long id)
        {
            if (_relations.ContainsKey(id))
            {
                return _relations[id];
            }
            return null;
        }

        /// <summary>
        /// Returns the relations with the given id's.
        /// 
        /// The returned list will have the same size as the original
        /// and the returned relations will be in the same position as their id's.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Osm.Core.Relation> GetRelations(IList<long> ids)
        {
            IList<Osm.Core.Relation> ret_list = new List<Osm.Core.Relation>(ids.Count);

            for (int idx = 0; idx < ids.Count; idx++)
            {
                long id = ids[idx];
                ret_list.Add(this.GetRelation(id));
            }

            return ret_list;
        }

        /// <summary>
        /// Returns the relations for the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IList<Osm.Core.Relation> GetRelationsFor(Osm.Core.OsmBase obj)
        {
            if (_relations_per_member.ContainsKey(obj.Id))
            {
                return this.GetRelations(_relations_per_member[obj.Id]);
            }
            return null;
        }

        /// <summary>
        /// Creates a new way.
        /// </summary>
        /// <returns></returns>
        public Osm.Core.Way CreateWay()
        {
            return OsmBaseFactory.CreateWay(KeyGenerator.GenerateNew());
        }

        /// <summary>
        /// Returns the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Osm.Core.Way GetWay(long id)
        {
            if (_ways.ContainsKey(id))
            {
                return _ways[id];
            }
            return null;
        }

        /// <summary>
        /// Returns all the ways in this datasource.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Osm.Core.Way> GetWays()
        {
            return this._ways.Values;
        }

        /// <summary>
        /// Returns the ways with the id's in the ids list.
        /// 
        /// The returned list will have the same size as the original
        /// and the returned ways will be in the same position as their id's.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Osm.Core.Way> GetWays(IList<long> ids)
        {
            IList<Osm.Core.Way> ret_list = new List<Osm.Core.Way>(ids.Count);

            for (int idx = 0; idx < ids.Count; idx++)
            {
                long id = ids[idx];
                ret_list.Add(this.GetWay(id));
            }

            return ret_list;
        }

        /// <summary>
        /// Returns the way(s) for the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IList<Osm.Core.Way> GetWaysFor(Osm.Core.Node node)
        {
            this.ReadFromDocument();

            if (_ways_per_node.ContainsKey(node.Id))
            {
                return this.GetWays(_ways_per_node[node.Id]);
            }
            return null;
        }

        /// <summary>
        /// Returns the objects that evaluate the filter to true.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<Osm.Core.OsmBase> Get(Filter filter)
        {
            this.ReadFromDocument();

            IList<Osm.Core.OsmBase> res = new List<Osm.Core.OsmBase>();
            foreach (Osm.Core.Node node in _nodes.Values)
            {
                if (filter.Evaluate(node))
                {
                    res.Add(node);
                }
            }
            foreach (Osm.Core.Way way in _ways.Values)
            {
                if (filter.Evaluate(way))
                {
                    res.Add(way);
                }
            }
            foreach (Osm.Core.Relation relation in _relations.Values)
            {
                if (filter.Evaluate(relation))
                {
                    res.Add(relation);
                }
            }

            return res;
        }

        /// <summary>
        /// Returns the objects that exist withing the given box and evaluate the filter to true.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<Osm.Core.OsmBase> Get(GeoCoordinateBox box, Filter filter)
        {
            this.ReadFromDocument();

            IList<Osm.Core.OsmBase> res = new List<Osm.Core.OsmBase>();
            foreach (Osm.Core.Node node in _nodes.Values)
            {
                if (filter.Evaluate(node) && node.Shape.Inside(box))
                {
                    res.Add(node);
                }
            }
            foreach (Osm.Core.Way way in _ways.Values)
            {
                if (filter.Evaluate(way) && way.Shape.Inside(box))
                {
                    res.Add(way);
                }
            }
            foreach (Osm.Core.Relation relation in _relations.Values)
            {
                if (filter.Evaluate(relation) && relation.Shape.Inside(box))
                {
                    res.Add(relation);
                }
            }

            return res;
        }

        #endregion

        #region Private Functions (Add-Remove objects)

        /// <summary>
        /// Adds a list of osm objects.
        /// </summary>
        /// <param name="objs"></param>
        public void AddOsmBase(IList<Osm.Core.OsmBase> objs)
        {
            foreach (Osm.Core.OsmBase obj in objs)
            {
                this.AddOsmBase(obj);
            }
        }

        /// <summary>
        /// Adds an osm object.
        /// </summary>
        /// <param name="obj"></param>
        public void AddOsmBase(Osm.Core.OsmBase obj)
        {
            switch (obj.Type)
            {
                case Osm.Core.OsmType.Node:
                    this.AddNode(obj as Osm.Core.Node);
                    break;
                case Osm.Core.OsmType.Relation:
                    this.AddRelation(obj as Osm.Core.Relation);
                    break;
                case Osm.Core.OsmType.Way:
                    this.AddWay(obj as Osm.Core.Way);
                    break;
            }
        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(Osm.Core.Node node)
        {
            if (_nodes.ContainsKey(node.Id))
            {
                throw new InvalidOperationException("Cannot add an object that already exists in this source!" + Environment.NewLine +
                    "If there is a modification use a changeset!");
            }
            else
            {
                _nodes.Add(node.Id, node);
            }
            this.RegisterChangeSetId(node.ChangeSetId);
        }

        /// <summary>
        /// Adds a way.
        /// </summary>
        /// <param name="way"></param>
        public void AddWay(Osm.Core.Way way)
        {
            if (_ways.ContainsKey(way.Id))
            {
                throw new InvalidOperationException("Cannot add an object that already exists in this source!" + Environment.NewLine +
                    "If there is a modification use a changeset!");
            }
            else
            {
                _ways.Add(way.Id, way);

                foreach (Osm.Core.Node node in way.Nodes)
                {
                    if (this.GetNode(node.Id) == null)
                    {
                        this.AddNode(node);
                    }
                }

            }
            this.RegisterChangeSetId(way.ChangeSetId);
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="relation"></param>
        public void AddRelation(Osm.Core.Relation relation)
        {
            if (_relations.ContainsKey(relation.Id))
            {
                throw new InvalidOperationException("Cannot add an object that already exists in this source!" + Environment.NewLine +
                    "If there is a modification use a changeset!");
            }
            else
            {
                _relations.Add(relation.Id, relation);

                foreach (Osm.Core.RelationMember member in relation.Members)
                {
                    Osm.Core.OsmGeo member_already_in = null;
                    switch (member.Member.Type)
                    {
                        case Osm.Core.OsmType.Node:
                            member_already_in = this.GetNode(member.Member.Id);
                            break;
                        case Osm.Core.OsmType.Relation:
                            member_already_in = this.GetRelation(member.Member.Id);
                            break;
                        case Osm.Core.OsmType.Way:
                            member_already_in = this.GetWay(member.Member.Id);
                            break;
                    }
                    if (member_already_in == null)
                    {
                        this.AddOsmBase(member.Member);
                    }
                }
            }
            this.RegisterChangeSetId(relation.ChangeSetId);
        }

        /// <summary>
        /// Removes a list of objects from this data source.
        /// </summary>
        /// <param name="objs"></param>
        private void RemoveOsmBase(IList<Osm.Core.OsmBase> objs)
        {
            foreach (Osm.Core.OsmBase obj in objs)
            {
                this.RemoveOsmBase(obj);
            }
        }

        /// <summary>
        /// Removes an object from this data source.
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveOsmBase(Osm.Core.OsmBase obj)
        {
            switch (obj.Type)
            {
                case Osm.Core.OsmType.Node:
                    this.RemoveNode(obj as Osm.Core.Node);
                    break;
                case Osm.Core.OsmType.Relation:
                    this.RemoveRelation(obj as Osm.Core.Relation);
                    break;
                case Osm.Core.OsmType.Way:
                    this.RemoveWay(obj as Osm.Core.Way);
                    break;
            }
        }

        /// <summary>
        /// Removes a node from this source.
        /// </summary>
        /// <param name="node"></param>
        private void RemoveNode(Osm.Core.Node node)
        {
            // check if node can be removed.
            IList<Osm.Core.Way> ways = this.GetWaysFor(node);
            if (ways.Count > 0)
            { // cannot remove node; there is still a way for this node.
                throw new InvalidOperationException("Cannot remove node {0}; there exists al least one way that uses it!");
            }
            IList<Osm.Core.Relation> relations = this.GetRelationsFor(node);
            if (relations.Count > 0)
            { // cannot remove node; there is still a relation for this node.
                throw new InvalidOperationException("Cannot remove node {0}; there exists al least one relation that uses it!");
            }

            _nodes.Remove(node.Id);
        }

        /// <summary>
        /// Removes a way from this source and all the nodes in it that are used only by this way.
        /// </summary>
        /// <param name="way"></param>
        private void RemoveWay(Osm.Core.Way way)
        {
            // check if way can be removed.
            IList<Osm.Core.Relation> relations = this.GetRelationsFor(way);
            if (relations.Count > 0)
            { // cannot remove node; there is still a relation for this way.
                throw new InvalidOperationException("Cannot remove way {0}; there exists al least one relation that uses it!");
            }

            // remove the way and all the nodes that exist only for this way.
            _ways.Remove(way.Id);
            foreach (Osm.Core.Node node in way.Nodes)
            {
                // first remove the way from the ways per node data.
                _ways_per_node[node.Id].Remove(way.Id);

                // check if there are other ways that are using this node.
                if (_ways_per_node[node.Id].Count == 0)
                { // remove the node if if is not used in any way.
                    this.RemoveNode(node);
                }
            }
        }

        /// <summary>
        /// Removes a relation from this source but none of the sub-objects.
        /// </summary>
        /// <param name="relation"></param>
        private void RemoveRelation(Osm.Core.Relation relation)
        {
            // check if relation can be removed.
            IList<Osm.Core.Relation> relations = this.GetRelationsFor(relation);
            if (relations.Count > 0)
            { // cannot remove node; there is still a relation for this relation.
                throw new InvalidOperationException("Cannot remove relation {0}; there exists al least one relation that uses it!");
            }

            // remove all the relation for all it's members
            foreach (Osm.Core.RelationMember member in relation.Members)
            {
                _relations_per_member[member.Member.Id].Remove(relation.Id);

                // if there was only one relation for this member remove the member.
                if (_relations_per_member[member.Member.Id].Count == 0)
                {
                    _relations_per_member.Remove(member.Member.Id);
                }
            }

            // remove the relation.
            _relations.Remove(relation.Id);
        }

        #endregion

        #region INodeSource Members

        Osm.Core.Node INodeSource.GetNode(long id)
        {
            if (_nodes.ContainsKey(id))
            {
                return _nodes[id];
            }
            return null;
        }

        #endregion
    
        #region IWaySource Members

        Osm.Core.Way IWaySource.GetWay(long id)
        {
            if (_ways.ContainsKey(id))
            {
                return _ways[id];
            }
            return null;
        }

        #endregion

        #region IRelationSource Members

        Osm.Core.Relation IRelationSource.GetRelation(long id)
        {
            if (_relations.ContainsKey(id))
            {
                return _relations[id];
            }
            return null;
        }

#endregion
    }
}
