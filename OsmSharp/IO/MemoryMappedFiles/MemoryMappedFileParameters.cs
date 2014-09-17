using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.IO.MemoryMappedFiles
{
    /// <summary>
    /// Parameters to configure the creation of memory mapped files.
    /// </summary>
    public class MemoryMappedFileParameters
    {
        /// <summary>
        /// Get or sets the in-memory flag.
        /// </summary>
        public bool InMemory { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public MemoryMappedFileType Type { get; set; }

        /// <summary>
        /// Creates parameters for a native directory factory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static MemoryMappedFileParameters NativeDirectory(string path)
        {
            return new MemoryMappedFileParameters()
            {
                InMemory = false,
                Path = path,
                Type = MemoryMappedFileType.Native
            };
        }
    }

    /// <summary>
    /// An enumeration of types of memory mapped files.
    /// </summary>
    public enum MemoryMappedFileType
    {
        /// <summary>
        /// Native type.
        /// </summary>
        Native,
        /// <summary>
        /// Generic type.
        /// </summary>
        Generic
    }
}
