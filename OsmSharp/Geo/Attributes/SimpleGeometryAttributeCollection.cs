using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Geo.Attributes
{
    /// <summary>
    /// Represents a simple tags collection based on a list.
    /// </summary>
    public class SimpleGeometryAttributeCollection : GeometryAttributeCollection
    {
        /// <summary>
        /// Holds the attributes.
        /// </summary>
        private readonly List<GeometryAttribute> _attributes;

        /// <summary>
        /// Creates a new attributes collection.
        /// </summary>
        public SimpleGeometryAttributeCollection()
        {
            _attributes = new List<GeometryAttribute>();
        }

        /// <summary>
        /// Creates a new attributes collection initialized with the given existing attributes.
        /// </summary>
        /// <param name="attributes"></param>
        public SimpleGeometryAttributeCollection(IEnumerable<GeometryAttribute> attributes)
        {
            _attributes = new List<GeometryAttribute>();
            _attributes.AddRange(attributes);
        }

        /// <summary>
        /// Creates a new attributes collection initialized with the given existing key-value tags.
        /// </summary>
        /// <param name="tags"></param>
        public SimpleGeometryAttributeCollection(IEnumerable<OsmSharp.Collections.Tags.Tag> tags)
        {
            _attributes = new List<GeometryAttribute>();
            if (tags != null)
            {
                foreach (OsmSharp.Collections.Tags.Tag tag in tags)
                {
                    _attributes.Add(new GeometryAttribute()
                    {
                        Key = tag.Key,
                        Value = tag.Value
                    });
                }
            }
        }

        /// <summary>
        /// Returns the number of attributes in this collection.
        /// </summary>
        public override int Count
        {
            get { return _attributes.Count; }
        }

        /// <summary>
        /// Adds a new attribute (key-value pair) to this attributes collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void Add(string key, object value)
        {
            _attributes.Add(new GeometryAttribute()
            {
                Key = key,
                Value = value
            });
        }

        /// <summary>
        /// Adds a new attribute to this collection.
        /// </summary>
        /// <param name="attribute"></param>
        public override void Add(GeometryAttribute attribute)
        {
            _attributes.Add(attribute);
        }

        /// <summary>
        /// Adds a new attribute (key-value pair) to this attributes collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void AddOrReplace(string key, object value)
        {
            for (int idx = 0; idx < _attributes.Count; idx++)
            {
                GeometryAttribute attribute = _attributes[idx];
                if (attribute.Key.Equals(key))
                {
                    attribute.Value = value;
                    _attributes[idx] = attribute;
                    return;
                }
            }
            this.Add(key, value);
        }

        /// <summary>
        /// Adds a new tag to this collection.
        /// </summary>
        /// <param name="tag"></param>
        public override void AddOrReplace(GeometryAttribute tag)
        {
            this.AddOrReplace(tag.Key, tag.Value);
        }

        /// <summary>
        /// Returns true if the given key is found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool ContainsKey(string key)
        {
            return this.Any(tag => tag.Key.Equals(key));
        }

        /// <summary>
        /// Returns true if the given key exists and gets the value parameter.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetValue(string key, out object value)
        {
            foreach (var tag in this.Where(tag => tag.Key.Equals(key)))
            {
                value = tag.Value;
                return true;
            }
            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Returns true if the given key-value pair is found in this attributes collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool ContainsKeyValue(string key, object value)
        {
            return this.Any(tag => tag.Key.Equals(key) && tag.Value.Equals(value));
        }

        /// <summary>
        /// Clears all attributes.
        /// </summary>
        public override void Clear()
        {
            _attributes.Clear();
        }

        /// <summary>
        /// Returns the enumerator for this attributes collection.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<GeometryAttribute> GetEnumerator()
        {
            return _attributes.GetEnumerator();
        }
    }
}
