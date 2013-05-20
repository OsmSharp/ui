using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives.Storage
{
    /// <summary>
    /// Represents an entry or object in a Scene2D.
    /// </summary>
    [ProtoContract]
    internal class Scene2DEntry
    {
        /// <summary>
        /// Gets or sets the id of this object.
        /// </summary>
        [ProtoMember(1)]
        public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        [ProtoMember(2)]
        public int Layer { get; set; }

        /// <summary>
        /// Gets or sets the primitive.
        /// </summary>
        [ProtoMember(3)]
        public IScene2DPrimitive Scene2DPrimitive { get; set; }
    }
}
