using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor.ChangeSets
{
    /// <summary>
    /// A changeset target that acts as a source of osm data; 
    /// Input=One changeset stream
    /// </summary>
    public abstract class DataProcessorChangeSetTarget
    {
        /// <summary>
        /// The source this target get's it's data from.
        /// </summary>
        private DataProcessorChangeSetSource _source;

        /// <summary>
        /// Creates a new change set data source by applying the changes to the given source.
        /// </summary>
        /// <param name="source"></param>
        public DataProcessorChangeSetTarget()
        {

        }

        /// <summary>
        /// Applies a change to the target.
        /// </summary>
        /// <param name="change"></param>
        public abstract void ApplyChange(SimpleChangeSet change);

        /// <summary>
        /// Pulls the changes from the source to this target.
        /// </summary>
        public void Pull()
        {
            _source.Initialize();
            this.Initialize();
            while (_source.MoveNext())
            {
                SimpleChangeSet change_set = _source.Current();
                this.ApplyChange(change_set);
            }
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Closes the current target.
        /// </summary>
        public virtual void Close()
        {
            _source = null;
        }

        /// <summary>
        /// Registers the source for this target.
        /// </summary>
        /// <param name="source"></param>
        public void RegisterSource(DataProcessorChangeSetSource source)
        {
            _source = source;
        }
    }
}
