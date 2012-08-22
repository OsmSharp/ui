using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core
{
    /// <summary>
    /// Represents a change in a change set.
    /// </summary>
    [Serializable]
    public class Change
    {
        /// <summary>
        /// Contains the type of change.
        /// </summary>
        private ChangeType _type;

        /// <summary>
        /// Contains the object to change.
        /// </summary>
        private OsmGeo _obj;

        /// <summary>
        /// Creates a new change object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        public Change(ChangeType type, OsmGeo obj)
        {
            _type = type;
            _obj = obj;
        }

        /// <summary>
        /// The type of this change.
        /// </summary>
        public ChangeType Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// The object this change is for.
        /// </summary>
        public OsmGeo Object
        {
            get
            {
                return _obj;
            }
        }
    }

    public enum ChangeType
    {
        Create,
        Delete,
        Modify
    }
}
