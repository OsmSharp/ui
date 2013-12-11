//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;

//namespace OsmSharp.IO.ByteCache
//{
//    /// <summary>
//    /// An on-disk cache storing small amounts of data.
//    /// </summary>
//    public class DiskByteCache : IByteCache
//    {        
//        /// <summary>
//        /// Holds the next id.
//        /// </summary>
//        private uint _nextId = 0;

//        /// <summary>
//        /// Holds the caching directory.
//        /// </summary>
//        private readonly string _cacheDirectory;

//        /// <summary>
//        /// Creates a new disk cache.
//        /// </summary>
//        public DiskByteCache()
//        {
//            string temp = System.IO.Path.GetTempPath() + Guid.NewGuid();
//            while (Directory.Exists(temp))
//            { // make sure the directory does not exist already by some amazing coincidence.
//                temp = System.IO.Path.GetTempPath() + Guid.NewGuid();
//            }
//            var path = new DirectoryInfo(temp);
//            path.Create();
//            _cacheDirectory = temp;
//        }

//        /// <summary>
//        /// Creates a fileinfo object.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        private FileInfo BuildFileInfo(uint id)
//        {
//            var builder = new StringBuilder();
//            builder.Append(_cacheDirectory);
//            builder.Append(Path.DirectorySeparatorChar);
//            builder.Append(id);
//            builder.Append(".cache");

//            return new FileInfo(builder.ToString());
//        }

//        /// <summary>
//        /// Returns the size of this cache.
//        /// </summary>
//        public int Size
//        {
//            get {
//                var path = new DirectoryInfo(_cacheDirectory);
//                return path.GetFiles().Length;
//            }
//        }

//        /// <summary>
//        /// Adds new data.
//        /// </summary>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        public uint Add(byte[] data)
//        {
//            _nextId++;
//            FileInfo currentFile = this.BuildFileInfo(_nextId);
//            while (currentFile.Exists)
//            { // no check for full cache but cache can still get HUGE.
//                // this is to prevent long-running processes from stopping.
//                if (_nextId == uint.MaxValue)
//                {
//                    _nextId = 0;
//                }
//                else
//                {
//                    _nextId++;
//                }
//                currentFile = this.BuildFileInfo(_nextId);
//            }

//            // write the data.
//            FileStream fileStream = currentFile.Open(FileMode.CreateNew);
//            fileStream.Write(data, 0, data.Length);
//            fileStream.Flush();
//            fileStream.Close();

//            return _nextId;
//        }

//        /// <summary>
//        /// Gets the data associated with the given id.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public byte[] Get(uint id)
//        {
//            FileInfo file = this.BuildFileInfo(id);
//            if (file.Exists)
//            {
//                FileStream fileStream = file.OpenRead();
//                var data = new byte[fileStream.Length];
//                fileStream.Read(data, 0, data.Length);
//                fileStream.Close();
//                return data;
//            }
//            throw new IndexOutOfRangeException();
//        }

//        /// <summary>
//        /// Removes all associated data.
//        /// </summary>
//        /// <param name="id"></param>
//        public void Remove(uint id)
//        {
//            FileInfo file = this.BuildFileInfo(id);
//            if (file.Exists)
//            {
//                file.Delete();
//            }
//        }

//        /// <summary>
//        /// Disposes of all resource associated with all files in this cache.
//        /// </summary>
//        public void Dispose()
//        {
//            var path = new DirectoryInfo(_cacheDirectory);
//            path.Delete(true);
//        }
//    }
//}
