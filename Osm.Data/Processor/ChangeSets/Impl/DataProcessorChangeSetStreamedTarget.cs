using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor.ChangeSets.Impl
{
    /// <summary>
    /// Class accepting a stream of changesets and merging it with a stream of OsmGeo objects to produce a changed stream source of OsmGeo data.
    /// </summary>
    public class DataProcessorChangeSetStreamedTarget : DataProcessorSource
    {
        /// <summary>
        /// Creates a new change set data source by applying the changes to the given source.
        /// </summary>
        /// <param name="source"></param>
        public DataProcessorChangeSetStreamedTarget()
        {

        }

        /// <summary>
        /// Applies a change to the target.
        /// </summary>
        /// <param name="change"></param>
        public void ApplyChange(SimpleChangeSet change)
        {
            
        }

        /// <summary>
        /// Pulls the changes from the source to this target.
        /// </summary>
        public void Pull(DataProcessorChangeSetSource source)
        {

        }

        /// <summary>
        /// Closes the current target.
        /// </summary>
        public virtual void Close()
        {

        }

        public void RegisterSource(DataProcessorChangeSetTarget target)
        {

        }

        #region OsmGeo Source Implementation

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public override SimpleOsmGeo Current()
        {
            throw new NotImplementedException();
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
