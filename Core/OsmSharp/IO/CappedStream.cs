using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.IO
{
    /// <summary>
    /// Represents a capped stream that can only be used along a given region.
    /// </summary>
    public class CappedStream : Stream
    {
        /// <summary>
        /// Holds the stream.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Holds the offset.
        /// </summary>
        private readonly long _offset;

        /// <summary>
        /// Holds the length.
        /// </summary>
        private readonly long _length;

        /// <summary>
        /// Creates a new capped stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public CappedStream(Stream stream, long offset, long length)
        {
            _stream = stream;
            _stream.Seek(offset, SeekOrigin.Begin);
            _offset = offset;
            _length = length;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            _stream.Flush();
        }

        /// <summary>
        /// Returns the current length of this stream.
        /// </summary>
        public override long Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Gets/sets the current position.
        /// </summary>
        public override long Position
        {
            get { return _stream.Position - _offset; }
            set { _stream.Position = value + _offset; }
        }

        /// <summary>
        /// Reads a sequence of bytes from the current
        ///     stream and advances the position within the stream by the number of bytes
        ///     read.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.Position + count >= _length)
            {
                count = (int)(_length - this.Position);
                return _stream.Read(buffer, offset, count);
            }
            return _stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Sets the position within the current
        ///     stream.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (offset >= _length)
            {
                throw new Exception();
            }
            return _stream.Seek(offset + _offset, origin);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            _stream.SetLength(value + _offset);
        }

        /// <summary>
        ///  Writes a sequence of bytes to the current
        ///     stream and advances the current position within this stream by the number
        ///     of bytes written.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
