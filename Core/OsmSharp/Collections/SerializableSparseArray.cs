//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using OsmSharp.IO.StreamCache;
//using ProtoBuf;
//using ProtoBuf.Meta;

//namespace OsmSharp.Collections
//{
//    /// <summary>
//    /// A serializable sparse array.
//    /// </summary>
//    public class SerializableSparseArray<T>
//    {
//        /// <summary>
//        /// Holds the serialized version of the array.
//        /// </summary>
//        private readonly IStreamCache _streamCache;
        
//        /// <summary>
//        /// Holds the block size.
//        /// </summary>
//        private readonly int _blockSize;

//        /// <summary>
//        /// Holds the virtual size.
//        /// </summary>
//        private int _virtualSize;

//        /// <summary>
//        /// Holds the array blocks.
//        /// </summary>
//        private readonly Dictionary<long, ArrayBlock> _arrayBlocks;

//        /// <summary>
//        /// Holds the header string.
//        /// </summary>
//        private const string SerializableSparseArrayHeader = "SparseArrayHeader.v1";

//        /// <summary>
//        /// Creates a new serializable sparce array.
//        /// </summary>
//        /// <param name="size"></param>
//        /// <param name="streamCache"></param>
//        public SerializableSparseArray(int size, IStreamCache streamCache)
//        {
//            _streamCache = streamCache;

//            _virtualSize = size;
//            _blockSize = 256;

//            _arrayBlocks = new Dictionary<long, ArrayBlock>();
//            _lastAccessedBlock = null;
//        }

//        /// <summary>
//        /// Holds the last accessed block to exploit locality of access.
//        /// </summary>
//        private ArrayBlock _lastAccessedBlock; 

//        /// <summary>
//        /// Gets/sets a value.
//        /// </summary>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        public T this[long index]
//        {
//            get
//            {
//                if (index >= _virtualSize)
//                { // index out of range!
//                    throw new IndexOutOfRangeException();
//                }
//                if (_lastAccessedBlock != null &&
//                    _lastAccessedBlock.Index <= index && (_lastAccessedBlock.Index + _blockSize) > index)
//                { // the last accessed block is contains the requested value.
//                    return _lastAccessedBlock.Data[index - _lastAccessedBlock.Index];
//                }
//                // calculate block index.
//                long blockIndex = index / _blockSize;
                
//                // get block.
//                ArrayBlock block;
//                if (_arrayBlocks.TryGetValue(blockIndex, out block))
//                { // return the value from this block.
//                    _lastAccessedBlock = block; // set last accessed block.
//                    return _lastAccessedBlock.Data[index - _lastAccessedBlock.Index]; 
//                }
//                return default(T); // no block was found!
//            }
//            set
//            {
//                if (index >= _virtualSize)
//                { // index out of range!
//                    throw new IndexOutOfRangeException();
//                }
//                if (_lastAccessedBlock != null &&
//                    _lastAccessedBlock.Index <= index && (_lastAccessedBlock.Index + _blockSize) > index)
//                { // the last accessed block is contains the requested value.
//                    _lastAccessedBlock.Data[index - _lastAccessedBlock.Index] = value;
//                    return;
//                }
//                // calculate block index.
//                long blockIndex = index / _blockSize;

//                // get block.
//                ArrayBlock block;
//                if (!_arrayBlocks.TryGetValue(blockIndex, out block))
//                { // return the value from this block.
//                    block = new ArrayBlock(blockIndex*_blockSize, _blockSize);
//                    _arrayBlocks.Add(blockIndex, block);
//                }
//                _lastAccessedBlock = block; // set last accessed block.
//                _lastAccessedBlock.Data[index - _lastAccessedBlock.Index] = value;
//            }
//        }

//        /// <summary>
//        /// Resizes this array.
//        /// </summary>
//        /// <param name="size">The new size.</param>
//        public void Resize(int size)
//        {
//            if (size >= _virtualSize)
//            { // increasing the size is easy!
//                _virtualSize = size;
//            }
//            else
//            { // decreasing the size; harder!
//                _virtualSize = size;

//                // remove unneeded blocks.
//                var unneededBlocks =
//                    new List<KeyValuePair<long, ArrayBlock>>();
//                foreach (var block in _arrayBlocks)
//                {
//                    if (block.Value.Index > _virtualSize)
//                    {
//                        unneededBlocks.Add(block);
//                    }
//                }
//                foreach (var unneededBlock in unneededBlocks)
//                {
//                    _arrayBlocks.Remove(unneededBlock.Key);
//                }
//            }
//        }

//        /// <summary>
//        /// Gets the length of this array.
//        /// </summary>
//        public int Length { get { return _virtualSize; }}

//        /// <summary>
//        /// Flushes the sparse array.
//        /// </summary>
//        public void Flush()
//        {
//            // move to the beginning of the stream.
//            _stream.Seek(0, SeekOrigin.Begin);

//            // create the array block header.
//            var arrayBlockHeader = new ArrayBlockHeader();
//            arrayBlockHeader.BlockId = new long[_arrayBlocks.Count];
//            arrayBlockHeader.BlockLenghts = new int[_arrayBlocks.Count];
//            arrayBlockHeader.Positions = new long[_arrayBlocks.Count];

//            int blockIdx = 0;
//            var streams = new Stream[_arrayBlocks.Count];
//            long position = 0;
//            foreach (var pair in _arrayBlocks)
//            {
//                // serialize the given block.
//                Stream stream = this.SerializeArrayBlock(pair.Value);
//                streams[blockIdx] = stream;
//                long length = stream.Length;

//                arrayBlockHeader.BlockId[blockIdx] = pair.Key;
//                arrayBlockHeader.Positions[blockIdx] = position;
//                arrayBlockHeader.BlockLenghts[blockIdx] = (int)length;

//                position = position + length;
//                blockIdx++;
//            }

//            // get the type model.
//            RuntimeTypeModel typeModel = this.GetTypeModel();

//            // serialize the array block header.
//            var headerStream = new MemoryStream();
//            typeModel.Serialize(headerStream, arrayBlockHeader);

//            // serialize the header.
//            byte[] header = ASCIIEncoding.ASCII.GetBytes(SerializableSparseArrayHeader);
//            _stream.Write(header, 0, header.Length);
//            byte[] headerSize = BitConverter.GetBytes(headerStream.Length);
//            _stream.Write(headerSize, 0, 4);

//            // write the header.
//            headerStream.CopyTo(_stream);
//            headerStream.Dispose();

//            // write blocks.
//            for (int idx = 0; idx < streams.Length; idx++)
//            {
//                streams[idx].CopyTo(_stream);
//            }
//            _stream.Flush();
//        }

//        /// <summary>
//        /// Deserialize the array block.
//        /// </summary>
//        /// <param name="stream"></param>
//        /// <param name="position"></param>
//        /// <param name="lenght"></param>
//        /// <returns></returns>
//        private ArrayBlock DeserializeArrayBlock(Stream stream, long position, int lenght)
//        {
//            // get bytes.
//            var blokBytes = new byte[lenght];
//            _stream.Seek(position, SeekOrigin.Begin);
//            _stream.Read(blokBytes, 0, lenght);

//            // deserialize.
//            RuntimeTypeModel runtimeTypeModel = this.GetTypeModel();
//            return runtimeTypeModel.Deserialize(new MemoryStream(blokBytes), null, typeof(ArrayBlock)) 
//                as ArrayBlock;
//        }

//        /// <summary>
//        /// Serializes the array block.
//        /// </summary>
//        /// <param name="block"></param>
//        private Stream SerializeArrayBlock(ArrayBlock block)
//        {
//            Stream stream = new MemoryStream();

//            RuntimeTypeModel runtimeTypeModel = this.GetTypeModel();
//            runtimeTypeModel.Serialize(stream, block);
//            return stream;
//        }

//        /// <summary>
//        /// Holds the type model when created.
//        /// </summary>
//        private RuntimeTypeModel _typeModel;

//        /// <summary>
//        /// Creates or gets the type model.
//        /// </summary>
//        /// <returns></returns>
//        private RuntimeTypeModel GetTypeModel()
//        {
//            if (_typeModel == null)
//            {
//                // build the run time type model.
//                _typeModel = TypeModel.Create();
//                _typeModel.Add(typeof(ArrayBlockHeader), true);
//                _typeModel.Add(typeof(ArrayBlock), true); // the tile metadata.
//                //_typeModel.Add(typeof(T), true); // the tile metadata.

//                _typeModel[typeof (ArrayBlock)][2].SupportNull = true;
//                //RuntimeTypeModel.Default[typeof(YourType)][3].SupportNull = true;
//            }
//            return _typeModel;
//        }

//        /// <summary>
//        /// Represents an array block.
//        /// </summary>
//        [ProtoContract]
//        protected class ArrayBlock
//        {
//            /// <summary>
//            /// Creates a new array block.
//            /// </summary>
//            /// <param name="index"></param>
//            /// <param name="size"></param>
//            public ArrayBlock(long index, int size)
//            {
//                this.Index = index;
//                this.Data = new T[size];
//            }

//            /// <summary>
//            /// The starting index of this block.
//            /// </summary>
//            [ProtoMember(1)]
//            public long Index { get; private set; }

//            /// <summary>
//            /// The actual data.
//            /// </summary>
//            [ProtoMember(2)]
//            public T[] Data { get; private set; }
//        }

//        /// <summary>
//        /// Represents the header of serialized sparse array.
//        /// </summary>
//        [ProtoContract]
//        private class ArrayBlockHeader
//        {
//            /// <summary>
//            /// Gets or sets the block ids.
//            /// </summary>
//            [ProtoMember(1)]
//            public long[] BlockId { get; set; }

//            /// <summary>
//            /// Gets or sets the block positions.
//            /// </summary>
//            [ProtoMember(2)]
//            public long[] Positions { get; set; }

//            /// <summary>
//            /// Gets or sets the block lengths.
//            /// </summary>
//            [ProtoMember(3)]
//            public int[] BlockLenghts { get; set; }
//        }
//    }
//}