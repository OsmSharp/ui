// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.IO.MemoryMappedFiles;
using System.IO;

namespace OsmSharp.WinForms.UI.IO.MemoryMappedFiles
{
    /// <summary>
    /// A temporary memory mapped file.
    /// </summary>
    public class TempMemoryMappedFile : MemoryMappedStream
    {
        /// <summary>
        /// Holds the temporary file info.
        /// </summary>
        private FileInfo _tempFile;

        /// <summary>
        /// Holds the stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Creates a new temporary memory mapped file.
        /// </summary>
        public TempMemoryMappedFile()
            : this(new FileInfo(Path.GetTempFileName()))
        {

        }

        /// <summary>
        /// Creates a new temporary memory mapped file.
        /// </summary>
        /// <param name="tempFile"></param>
        public TempMemoryMappedFile(FileInfo tempFile)
            : this(tempFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None), tempFile)
        {

        }

        /// <summary>
        /// Creates a new temporary memory mapped file.
        /// </summary>
        private TempMemoryMappedFile(Stream stream, FileInfo tempFile)
        {
            _tempFile = tempFile;
            _stream = stream;
        }

        /// <summary>
        /// Disposes of all resources associated with this files.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            _stream.Dispose();
            _tempFile.Delete();
        }
    }
}