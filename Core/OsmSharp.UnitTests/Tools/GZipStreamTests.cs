using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zlib;
using NUnit.Framework;

namespace OsmSharp.UnitTests.Tools
{
    /// <summary>
    /// Contains some simple compression/decompression tests.
    /// </summary>
    [TestFixture]
    public class GZipStreamTests
    {
        /// <summary>
        /// Simple compression/decompression test.
        /// </summary>
        [Test]
        public void GZipStreamTest()
        {
            string testString = "Some testing string to compress/decompress using the GzipStream object!";

            // compress.
            var testStringBytes = ASCIIEncoding.ASCII.GetBytes(testString);
            var compressedStream = new MemoryStream();
            var stream = new GZipStream(compressedStream, CompressionMode.Compress);
            stream.Write(testStringBytes, 0, testStringBytes.Length);
            stream.Flush();
            byte[] compressedTestString = compressedStream.ToArray();

            // decompress.
            compressedStream = new MemoryStream(compressedTestString);
            var decompressiongStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            var decompressedStream = new MemoryStream();
            decompressiongStream.CopyTo(decompressedStream);
            var decompressedTestString = new byte[decompressedStream.Length];
            decompressedStream.Read(decompressedTestString, 0, decompressedTestString.Length);

            string resultingTestString = ASCIIEncoding.ASCII.GetString(decompressedTestString);
        }
    }
}
