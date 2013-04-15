// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Simple;

namespace OsmSharp.Osm.Data.Core.Processor.SimpleSource
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
        /// <param name="way_id"></param>
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
        /// <param name="relation_id"></param>
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
