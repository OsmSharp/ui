using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.HH.Core;

namespace Osm.Routing.HH
{
    /// <summary>
    /// Interface to give progress.
    /// </summary>
    public interface IHighwayHierarchyProgressReporter
    {
        /// <summary>
        /// Notifies listeners that a new level has been started.
        /// </summary>
        /// <param name="level"></param>
        void NewLevel(int level);

        ///// <summary>
        ///// Notifies listeners that a next vertex has been selected from processing.
        ///// </summary>
        ///// <param name="vertex"></param>
        //void StartedVertex(GraphVertex vertex);

        ///// <summary>
        ///// Notifies listeners that a list of new edges was found.
        ///// </summary>
        ///// <param name="edges"></param>
        //void HighwayEdge(HighwayEdge edges);

        /// <summary>
        /// Notifies listeners that the core calculation has been started.
        /// </summary>
        void StartCore();

        ///// <summary>
        ///// Notifies listeners that edges have been removed.
        ///// </summary>
        ///// <param name="edges"></param>
        //void RemovedFromCore(HighwayEdge edge);

        ///// <summary>
        ///// Notifies listeners that edges have been added.
        ///// </summary>
        ///// <param name="edges"></param>
        //void AddedToCore(HighwayEdge edge);
    }
}
