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

using System;
using System.IO;

namespace OsmSharp.IO.LZ4.Seekable
{
    /// <summary>
    /// A write-only random access compressed stream writer.
    /// </summary>
    public class LZ4SeekableStreamWriter : Stream
    {
        // RANDOM ACCESS STRUCTURE: very simple basic block-based definition.
        //
        // conditions:  - good compression ratio is important but not critical.
        //              - decompression performance is the most important part.
        // structure:   [BUFFER_SIZE][NEXT_LOCATION1|DATA][NEXT_LOCATION2|DATA][NEXT_LOCATION3|DATA][NEXT_LOCATION4|DATA][NEXT_LOCATION5|DATA]...
        // when reading: 
        //               Build index    [0] = NEXT_LOCATION1
        //                              [1] = NEXT_LOCATION2
        //                              [2] = NEXT_LOCATION3
        //                              [3] = NEXT_LOCATION4
        //                              [4] = NEXT_LOCATION5
        //

        /// <summary>
        /// Holds the writable stream.
        /// </summary>
        private Stream _stream = null;

        /// <summary>
        /// Holds the buffer.
        /// </summary>
        private byte[] _buffer = null;

        /// <summary>
        /// Holds the current position in buffer.
        /// </summary>
        private ushort _bufferPosition = 0;

        /// <summary>
        /// Creates a compressed seekable stream that is writeonly.
        /// </summary>
        /// <param name="stream">The random-access stream.</param>
        /// <param name="bufferSize">The buffer size.</param>
        public LZ4SeekableStreamWriter(Stream stream, ushort bufferSize)
        {
            _stream = stream;
            _buffer = new byte[bufferSize];

            // write buffer size.
            _stream.Write(BitConverter.GetBytes(bufferSize), 0, 2);
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the length in bytes of this stream.
        /// </summary>
        public override long Length
        {
            get { return _stream.Length; }
        }

        /// <summary>
        /// Gets or sets the stream position.
        /// </summary>
        public override long Position
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            for(int idx = offset; idx < offset + count; idx++)
            {
                if(_buffer.Length - 1 == _bufferPosition)
                { // the current buffer needs to be flushed.
                    this.FlushBuffer();
                }
                _buffer[_bufferPosition] = buffer[offset];
                _bufferPosition++;
            }
        }

        /// <summary>
        /// Flushes the current buffer.
        /// </summary>
        private void FlushBuffer()
        {
            _stream.Write(BitConverter.GetBytes(_bufferPosition), 0, 2);
            _stream.Write(_buffer, 0, _bufferPosition);
            _bufferPosition = 0;
        }
    }
}