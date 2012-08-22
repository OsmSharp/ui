using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Tools.Math.Geo;
using Osm.Core.Filters;
using Osm.Core.Sources;

namespace Osm.Data
{
    /// <summary>
    /// Represents a generic data source.
    /// 
    /// This datasource can create elements.
    /// 
    /// Used For: OSM Files
    /// </summary>
    public interface IDataSource : IDataSourceReadOnly
    {
        //#region Events

        ///// <summary>
        ///// Delegates used to delegate changes.
        ///// </summary>
        ///// <param name="changes"></param>
        //public delegate void OsmChangesDelegate(IList<Change> changes);

        ///// <summary>
        ///// Event throw when data was changed.
        ///// </summary>
        //public event OsmChangesDelegate DataChanged;

        //#endregion

        #region Persist

        /// <summary>
        /// Persists all the data to the underlying source.
        /// </summary>
        void Persist();

        #endregion

        #region Features

        /// <summary>
        /// Returns true if this datasource is bounded.
        /// </summary>
        bool HasBoundinBox { get; }

        /// <summary>
        /// Returns true if this datasource can generate final id's.
        /// </summary>
        bool IsBaseIdGenerator { get; }

        /// <summary>
        /// Returns true if this datasource can create new objects.
        /// </summary>
        bool IsCreator { get; }

        /// <summary>
        /// Returns true if this datasource is readonly.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Returns true if this data source does not have to be saved.
        /// 
        /// The data is persisted live or not while adding/removing data.
        /// </summary>
        bool IsLive { get; }

        #endregion

        #region Nodes

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <returns></returns>
        Node CreateNode();

        #endregion

        #region Relation

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <returns></returns>
        Relation CreateRelation();

        #endregion

        #region Way

        /// <summary>
        /// Creates a new way.
        /// </summary>
        /// <returns></returns>
        Way CreateWay();

        #endregion

        #region Changes
        
        /// <summary>
        /// Applies the given changeset to the data in this datasource.
        /// </summary>
        /// <param name="change_set"></param>
        void ApplyChangeSet(ChangeSet change_set);

        /// <summary>
        /// Creates a new changeset.
        /// </summary>
        /// <returns></returns>
        ChangeSet CreateChangeSet();

        #endregion
    }
}
