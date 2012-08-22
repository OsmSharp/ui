using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;

namespace Osm.Core
{
    /// <summary>
    /// Represents a changeset.
    /// </summary>
    [Serializable]
    public class ChangeSet : OsmBase
    {
        /// <summary>
        /// Holds all the changes in this changeset.
        /// </summary>
        private IList<Change> _changes;

        /// <summary>
        /// Creates a new changeset.
        /// </summary>
        /// <param name="id"></param>
        internal ChangeSet(long id)
            :base(id)
        {
            _changes = new List<Change>();
        }

        #region Properties

        /// <summary>
        /// Returns an ordered list of all changes in this changeset.
        /// </summary>
        public IList<Change> Changes
        {
            get
            {
                return _changes;
            }
        }

        /// <summary>
        /// Returns the list of objects that this changeset applies to.
        /// </summary>
        public IList<OsmGeo> Objects
        {
            get
            {
                IList<OsmGeo> objs = new List<OsmGeo>();

                foreach (Change change in this.Changes)
                {
                    objs.Add(change.Object);
                }

                return objs;
            }
        }

        /// <summary>
        /// Returns the bounding box of this changeset.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get 
            {
                if (this.Objects.Count > 0)
                {
                    GeoCoordinateBox box = this.Objects[0].BoundingBox;

                    for (int idx = 1; idx < this.Objects.Count; idx++)
                    {
                        box = box + this.Objects[idx].BoundingBox;
                    }

                    return box;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the changeset type.
        /// </summary>
        public override OsmType Type
        {
            get { return OsmType.ChangeSet; }
        }

        #endregion
    }
}
