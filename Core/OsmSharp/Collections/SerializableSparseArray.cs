using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ProtoBuf.Meta;

namespace OsmSharp.Collections
{
    /// <summary>
    /// A serializable sparse array.
    /// </summary>
    public class SerializableSparseArray<T>
    {
        /// <summary>
        /// Holds the serialized version of the array.
        /// </summary>
        private Stream _stream;
        
        /// <summary>
        /// Holds the block size.
        /// </summary>
        private readonly int _blockSize;

        /// <summary>
        /// Holds the virtual size.
        /// </summary>
        private int _virtualSize;

        /// <summary>
        /// Holds the array blocks.
        /// </summary>
        private readonly Dictionary<long, ArrayBlock> _arrayBlocks;

        /// <summary>
        /// Holds the header string.
        /// </summary>
        private const string SerializableSparseArrayHeader = "SparseArrayHeader.v1";

        /// <summary>
        /// Creates a new serializable sparce array.
        /// </summary>
        /// <param name="blockSize"></param>
        /// <param name="stream"></param>
        public SerializableSparseArray(int blockSize, Stream stream)
        {
            _stream = stream;

            _virtualSize = blockSize;
            _blockSize = 256;

            _arrayBlocks = new Dictionary<long, ArrayBlock>();
            _lastAccessedBlock = null;
        }

        /// <summary>
        /// Holds the last accessed block to exploit locality of access.
        /// </summary>
        private ArrayBlock _lastAccessedBlock; 

        /// <summary>
        /// Gets/sets a value.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[long index]
        {
            get
            {
                if (index >= _virtualSize)
                { // index out of range!
                    throw new IndexOutOfRangeException();
                }
                if (_lastAccessedBlock != null &&
                    _lastAccessedBlock.Index <= index && (_lastAccessedBlock.Index + _blockSize) > index)
                { // the last accessed block is contains the requested value.
                    return _lastAccessedBlock.Data[index - _lastAccessedBlock.Index];
                }
                // calculate block index.
                long blockIndex = index / _blockSize;
                
                // get block.
                ArrayBlock block;
                if (_arrayBlocks.TryGetValue(blockIndex, out block))
                { // return the value from this block.
                    _lastAccessedBlock = block; // set last accessed block.
                    return _lastAccessedBlock.Data[index - _lastAccessedBlock.Index]; 
                }
                return default(T); // no block was found!
            }
            set
            {
                if (index >= _virtualSize)
                { // index out of range!
                    throw new IndexOutOfRangeException();
                }
                if (_lastAccessedBlock != null &&
                    _lastAccessedBlock.Index <= index && (_lastAccessedBlock.Index + _blockSize) > index)
                { // the last accessed block is contains the requested value.
                    _lastAccessedBlock.Data[index - _lastAccessedBlock.Index] = value;
                }
                // calculate block index.
                long blockIndex = index / _blockSize;

                // get block.
                ArrayBlock block;
                if (!_arrayBlocks.TryGetValue(blockIndex, out block))
                { // return the value from this block.
                    block = new ArrayBlock(blockIndex*_blockSize, _blockSize);
                    _arrayBlocks.Add(blockIndex, block);
                }
                _lastAccessedBlock = block; // set last accessed block.
                _lastAccessedBlock.Data[index - _lastAccessedBlock.Index] = value;
            }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size">The new size.</param>
        public void Resize(int size)
        {
            if (size >= _virtualSize)
            { // increasing the size is easy!
                _virtualSize = size;
            }
            else
            { // decreasing the size harder.
                _virtualSize = size;

                // remove unneeded blocks.
                var unneededBlocks =
                    new List<KeyValuePair<long, ArrayBlock>>();
                foreach (var block in _arrayBlocks)
                {
                    if (block.Value.Index > _virtualSize)
                    {
                        unneededBlocks.Add(block);
                    }
                }
                foreach (var unneededBlock in unneededBlocks)
                {
                    _arrayBlocks.Remove(unneededBlock.Key);
                }
            }
        }

        /// <summary>
        /// Gets the length of this array.
        /// </summary>
        public int Length { get { return _virtualSize; }}

        /// <summary>
        /// Flushes the sparse array.
        /// </summary>
        public void Flush()
        {
            // move to the beginning of the stream.
            _stream.Seek(0, SeekOrigin.Begin);

            // create the array block header.
            var arrayBlockHeader = new ArrayBlockHeader();
            arrayBlockHeader.BlockId = new long[_arrayBlocks.Count];
            arrayBlockHeader.BlockLenghts = new int[_arrayBlocks.Count];
            arrayBlockHeader.Positions = new long[_arrayBlocks.Count];

            int idx = 0;
            var streams = new Stream[_arrayBlocks.Count];
            int position = 0;
            foreach (var pair in _arrayBlocks)
            {
                // serialize the given block.
                Stream stream = this.SerializeArrayBlock(position, pair.Value);
                streams[idx] = stream;
                int length = streams.Length;

                arrayBlockHeader.BlockId[idx] = pair.Key;
                arrayBlockHeader.Positions[idx] = position;
                arrayBlockHeader.BlockLenghts[idx] = length;

                position = position + length;
            }

            // build the run time type model.
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(ArrayBlockHeader), true);
            typeModel.Add(typeof(ArrayBlock), true); // the tile metadata.
            
            // serialize the array block header.
            var headerStream = new MemoryStream();
            typeModel.Serialize(headerStream, arrayBlockHeader);

            // serialize the header.
            byte[] header = ASCIIEncoding.ASCII.GetBytes(SerializableSparseArrayHeader);
            _stream.Write(header, 0, header.Length);
            byte[] headerSize = BitConverter.GetBytes(headerStream.Length);
            _stream.Write(headerSize, 0, 4);

            // write the header.
            headerStream.CopyTo(_stream);
            headerStream.Dispose();

            // write blocks.
            for (int idx = 0; idx < streams.Length; idx++)
            {
                streams[idx].CopyTo(_stream);
            }
            _stream.Flush();
        }

        /// <summary>
        /// Deserialize the array block.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        private ArrayBlock DeserializeArrayBlock(long position, int lenght)
        {
            return null;
        }

        /// <summary>
        /// Serializes the array block.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="block"></param>
        private Stream SerializeArrayBlock(long position, ArrayBlock block)
        {
            return null;
        }

        /// <summary>
        /// Represents an array block.
        /// </summary>
        private class ArrayBlock
        {
            /// <summary>
            /// Creates a new array block.
            /// </summary>
            /// <param name="index"></param>
            /// <param name="size"></param>
            public ArrayBlock(long index, int size)
            {
                this.Index = index;
                this.Data = new T[size];
            }

            /// <summary>
            /// The starting index of this block.
            /// </summary>
            public long Index { get; private set; }

            /// <summary>
            /// The actual data.
            /// </summary>
            public T[] Data { get; private set; }
        }

        /// <summary>
        /// Represents the header of serialized sparse array.
        /// </summary>
        private class ArrayBlockHeader
        {
            /// <summary>
            /// Gets or sets the block ids.
            /// </summary>
            public long[] BlockId { get; set; }

            /// <summary>
            /// Gets or sets the block positions.
            /// </summary>
            public long[] Positions { get; set; }

            /// <summary>
            /// Gets or sets the block lengths.
            /// </summary>
            public int[] BlockLenghts { get; set; }
        }
    }
}
