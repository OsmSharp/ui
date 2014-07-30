// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

namespace OsmSharp.IO.MemoryMappedFiles
{
    /// <summary>
    /// Native image cache factory.
    /// 
    /// Uses dependency injection to build native images.
    /// </summary>
    public static class NativeMemoryMappedFileFactory
    {
        /// <summary>
        /// Delegate to create a native MemoryMappedFile.
        /// </summary>
        /// <param name="path">The path to file to map.</param>
        public delegate IMemoryMappedFile NativeMemoryMappedFileCreate(string path);

        /// <summary>
        /// The native MemoryMappedFile create delegate.
        /// </summary>
        private static NativeMemoryMappedFileCreate _nativeMemoryMappedFileDelegate;

        /// <summary>
        /// Delegate to create a memory-mapped file that has the specified capacity in system memory.
        /// </summary>
        /// <param name="mapName">A name to assign to the memory-mapped file.</param>
        /// <param name="capacity">The maximum size, in bytes, to allocate to the memory-mapped file.</param>
        public delegate IMemoryMappedFile NativeMemoryMappedFileSharedCreate(string mapName, long capacity);

        /// <summary>
        /// The native MemoryMappedFile shared create delegate.
        /// </summary>
        private static NativeMemoryMappedFileSharedCreate _nativeMemoryMappedFileSharedDelegate;

        /// <summary>
        /// Creates a native MemoryMappedFile.
        /// </summary>
        /// <param name="path">The path to file to map.</param>
        public static IMemoryMappedFile Create(string path)
        {
            if (_nativeMemoryMappedFileDelegate == null)
            { // oeps, not initialized.
                throw new InvalidOperationException("MemoryMappedFile creating delegate not initialized, call OsmSharp.{Platform).UI.Native.Initialize() in the native code.");
            }
            return _nativeMemoryMappedFileDelegate.Invoke(path);
        }

        /// <summary>
        /// Creates a new memory-mapped file that has the specified capacity in system memory.
        /// </summary>
        /// <param name="mapName">A name to assign to the memory-mapped file.</param>
        /// <param name="capacity">The maximum size, in bytes, to allocate to the memory-mapped file.</param>
        public static IMemoryMappedFile Create(string mapName, long capacity)
        {
            if (_nativeMemoryMappedFileSharedDelegate == null)
            { // oeps, not initialized.
                throw new InvalidOperationException("MemoryMappedFile creating delegate not initialized, call OsmSharp.{Platform).UI.Native.Initialize() in the native code.");
            }
            return _nativeMemoryMappedFileSharedDelegate.Invoke(mapName, capacity);
        }

        /// <summary>
        /// Sets the delegate.
        /// </summary>
        /// <param name="createMemoryMappedFile">Creates a memory mapped file delegate.</param>
        /// <param name="createMemoryMappedSharedFile">Creates a shared memory mapped file delegate.</param>
        public static void SetDelegates(NativeMemoryMappedFileCreate createMemoryMappedFile, NativeMemoryMappedFileSharedCreate createMemoryMappedSharedFile)
        {
            _nativeMemoryMappedFileDelegate = createMemoryMappedFile;
            _nativeMemoryMappedFileSharedDelegate = createMemoryMappedSharedFile;
        }
    }
}