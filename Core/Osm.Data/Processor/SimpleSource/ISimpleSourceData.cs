using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor.SimpleSource
{
    /// <summary>
    /// Abstracts a simple data source.
    /// </summary>
    public interface ISimpleSourceData
    {
        #region Nodes

        /// <summary>
        /// Returns a simple node.
        /// </summary>
        /// <param name="node_id"></param>
        /// <returns></returns>
        SimpleNode GetNode(long node_id);

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node"></param>
        void AddNode(SimpleNode node);

        /// <summary>
        /// Updates the given node.
        /// </summary>
        /// <param name="node"></param>
        void UpdateNode(SimpleNode node);

        /// <summary>
        /// Deletes a node with the given id.
        /// </summary>
        /// <param name="node_id"></param>
        void DeleteNode(long node_id);

        #endregion

        #region Ways

        /// <summary>
        /// Returns a simple way.
        /// </summary>
        /// <param name="node_id"></param>
        /// <returns></returns>
        SimpleWay GetWay(long way_id);

        /// <summary>
        /// Adds a way.
        /// </summary>
        /// <param name="way"></param>
        void AddWay(SimpleWay way);

        /// <summary>
        /// Updates the given way.
        /// </summary>
        /// <param name="way"></param>
        void UpdateWay(SimpleWay way);

        /// <summary>
        /// Deletes a way with the given id.
        /// </summary>
        /// <param name="way_id"></param>
        void DeleteWay(long way_id);

        /// <summary>
        /// Returns a collection of ways for a given node.
        /// </summary>
        /// <param name="node_id"></param>
        /// <returns></returns>
        IEnumerable<SimpleWay> GetForNode(long node_id);

        #endregion

        #region Relations

        /// <summary>
        /// Returns a simple relation.
        /// </summary>
        /// <param name="node_id"></param>
        /// <returns></returns>
        SimpleRelation GetRelation(long relation_id);

        /// <summary>
        /// Adds a way.
        /// </summary>
        /// <param name="way"></param>
        void AddRelation(SimpleRelation way);

        /// <summary>
        /// Updates the given way.
        /// </summary>
        /// <param name="way"></param>
        void UpdateRelation(SimpleRelation way);

        /// <summary>
        /// Deletes a way with the given id.
        /// </summary>
        /// <param name="way_id"></param>
        void DeleteRelation(long way_id);

        #endregion
    }
}
