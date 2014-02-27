using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bfw.Common
{
    /// <summary>
    /// Static helper functions for streams.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// The default buffer size.
        /// </summary>
        public const int DEFAULT_BUFFER_SIZE = 4096;

        /// <summary>
        /// Copies a stream.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dest">The destination.</param>
        public static void Copy(this Stream source, Stream dest)
        {
            Copy(source, dest, DEFAULT_BUFFER_SIZE);
        }

        /// <summary>
        /// Helper method to copy bytes from the source stream into the destination
        /// stream.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dest">The destination.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        public static void Copy(this Stream source, Stream dest, int bufferSize)
        {
            if (source == null)
            {
                return;
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentException("bufferSize must be greater than zero");
            }

            if (null == source)
            {
                return;
            }

            byte[] buff = new byte[bufferSize];
            int read = 0;            
            while ((read = source.Read(buff, 0, bufferSize)) > 0)
            {
                dest.Write(buff, 0, read);
            }
        }

        /// <summary>
        /// Helper method to extract content from the source stream.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <returns>
        /// Content from the stream as a string.
        /// </returns>
        public static string AsString(this Stream source)
        {
            string data = "";

            var sw = new System.IO.StreamReader(source);
            data = sw.ReadToEnd();

            return data;
        }
    }
}
