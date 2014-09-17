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

namespace OsmSharp.IO.MemoryMappedFiles.Native
{
    /// <summary>
    /// Native image cache factory.
    /// 
    /// Uses dependency injection to build native images.
    /// </summary>
    public class NativeMemoryMappedFileFactory
    {
        /// <summary>
        /// Holds the path.
        /// </summary>
        private string _path;

        /// <summary>
        /// Creates a new memory mapped file factory.
        /// </summary>
        public NativeMemoryMappedFileFactory()
        {
            _path = null;
        }

        /// <summary>
        /// Creates a new memory mapped file factory.
        /// </summary>
        /// <param name="path"></param>
        public NativeMemoryMappedFileFactory(string path)
        {
            _path = path;
        }

        /// <summary>
        /// Delegate to create a native MemoryMappedFile.
        /// </summary>
        /// <param name="path">The path to file to map.</param>
        /// <param name="capacity">The maximum size, in bytes, to allocate to the memory-mapped file.</param>
        public delegate INativeMemoryMappedFile NativeMemoryMappedFileCreate(string path, long capacity);

        /// <summary>
        /// The native MemoryMappedFile create delegate.
        /// </summary>
        private static NativeMemoryMappedFileCreate _nativeMemoryMappedFileDelegate;

        /// <summary>
        /// Creates a native MemoryMappedFile.
        /// </summary>
        /// <param name="path">The path to file to map.</param>
        /// <param name="capacity">The maximum size, in bytes, to allocate to the memory-mapped file.</param>
        public static INativeMemoryMappedFile CreateFromFile(string path, long capacity)
        {
            if (_nativeMemoryMappedFileDelegate == null)
            { // oeps, not initialized.
                throw new InvalidOperationException("MemoryMappedFile creating delegate not initialized, call OsmSharp.{Platform).UI.Native.Initialize() in the native code.");
            }
            return _nativeMemoryMappedFileDelegate.Invoke(path, capacity);
        }

        /// <summary>
        /// Delegate to create a memory-mapped file that has the specified capacity in system memory.
        /// </summary>
        /// <param name="mapName">A name to assign to the memory-mapped file.</param>
        /// <param name="capacity">The maximum size, in bytes, to allocate to the memory-mapped file.</param>
        public delegate INativeMemoryMappedFile NativeMemoryMappedFileSharedCreate(string mapName, long capacity);

        /// <summary>
        /// The native MemoryMappedFile shared create delegate.
        /// </summary>
        private static NativeMemoryMappedFileSharedCreate _nativeMemoryMappedFileSharedDelegate;

        /// <summary>
        /// Creates a new memory-mapped file that has the specified capacity in system memory.
        /// </summary>
        /// <param name="mapName">A name to assign to the memory-mapped file.</param>
        /// <param name="capacity">The maximum size, in bytes, to allocate to the memory-mapped file.</param>
        public static INativeMemoryMappedFile CreateNew(string mapName, long capacity)
        {
            if (_nativeMemoryMappedFileSharedDelegate == null)
            { // oeps, not initialized.
                throw new InvalidOperationException("MemoryMappedFile creating delegate not initialized, call OsmSharp.{Platform).UI.Native.Initialize() in the native code.");
            }
            return _nativeMemoryMappedFileSharedDelegate.Invoke(mapName, capacity);
        }

        /// <summary>
        /// Delegate to calculate the size in-memory of the given type.
        /// </summary>
        /// <param name="type">A name to assign to the memory-mapped file.</param>
        public delegate int SizeDelegate(Type type);

        /// <summary>
        /// The native get size delegate.
        /// </summary>
        private static SizeDelegate _getSizeDelegate;

        /// <summary>
        /// Calculates the size in-memory of the given type.
        /// </summary>
        /// <param name="type">A name to assign to the memory-mapped file.</param>
        public static int GetSize(Type type)
        {
            if (_getSizeDelegate == null)
            { // oeps, not initialized.
                throw new InvalidOperationException("MemoryMappedFile get size not initialized, call OsmSharp.{Platform).UI.Native.Initialize() in the native code.");
            }
            return _getSizeDelegate.Invoke(type);
        }

        /// <summary>
        /// Sets the delegate.
        /// </summary>
        /// <param name="createMemoryMappedFile">Creates a memory mapped file delegate.</param>
        /// <param name="createMemoryMappedSharedFile">Creates a shared memory mapped file delegate.</param>
        /// <param name="getSizeDelegate">Delegate to calculate the size in-memory of the given type.</param>
        public static void SetDelegates(NativeMemoryMappedFileCreate createMemoryMappedFile, NativeMemoryMappedFileSharedCreate createMemoryMappedSharedFile, SizeDelegate getSizeDelegate)
        {
            _nativeMemoryMappedFileDelegate = createMemoryMappedFile;
            _nativeMemoryMappedFileSharedDelegate = createMemoryMappedSharedFile;
            _getSizeDelegate = getSizeDelegate;
        }

        /// <summary>
        /// Returns the size of the given structure on disk.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int SizeOf<T>()
            where T : struct
        {
            return NativeMemoryMappedFileFactory.GetSize(typeof(T));
        }

        /// <summary>
        /// Creates a new memory mapped file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sizeInBytes"></param>
        /// <returns></returns>
        public IMemoryMappedFile<T> DoCreateNew<T>(long sizeInBytes)
            where T : struct
        {
            IMemoryMappedFile<T> newFile = null;
            if (!string.IsNullOrEmpty(_path))
            {
                newFile = new NativeMemoryMappedFileWrapper<T>(
                    NativeMemoryMappedFileFactory.CreateFromFile(_path + System.Guid.NewGuid().ToString(), sizeInBytes));
            }
            else
            {
                newFile = new NativeMemoryMappedFileWrapper<T>(
                    NativeMemoryMappedFileFactory.CreateNew(System.Guid.NewGuid().ToString(), sizeInBytes));
            }
            return newFile;
        }
    }
}