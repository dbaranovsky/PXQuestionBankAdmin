using System;
using System.Linq;
using System.Text;
using System.IO;


namespace Bfw.PX.Notes
{
    /// <summary>
    /// Static content filter.
    /// </summary>
    public class StaticContentFilter : Stream
    {
        /// <summary>
        /// Base tag text.
        /// </summary>
        private readonly static char[] BASE_TAG = "base".ToCharArray();

        /// <summary>
        /// Img tag text.
        /// </summary>
        private readonly static char[] IMG_TAG = "img".ToCharArray();

        /// <summary>
        /// inline image tag
        /// </summary>
        private readonly static char[] INLINE_IMG_TAG = "data:image".ToCharArray();

        /// <summary>
        /// Anchor tag text.
        /// </summary>
        private readonly static char[] ANCHOR_TAG = "a".ToCharArray();

        /// <summary>
        /// Script tag text.
        /// </summary>
        private readonly static char[] SCRIPT_TAG = "script".ToCharArray();

        /// <summary>
        /// Link tag text.
        /// </summary>
        private readonly static char[] LINK_TAG = "link".ToCharArray();

        /// <summary>
        /// Src attribute text.
        /// </summary>
        private readonly static char[] SRC_ATTRIBUTE = "src".ToCharArray();

        /// <summary>
        /// Href attribute text.
        /// </summary>
        private readonly static char[] HREF_ATTRIBUTE = "href".ToCharArray();

        /// <summary>
        /// Style attribute text.
        /// </summary>
        private readonly static char[] STYLE_ATTRIBUTE = "style".ToCharArray();

        /// <summary>
        /// Http prefix text.
        /// </summary>
        private readonly static char[] HTTP_PREFIX = "http://".ToCharArray();

        /// <summary>
        /// Javascript prefix text.
        /// </summary>
        private readonly static char[] JAVASCRIPT_PREFIX = "javascript:".ToCharArray();

        /// <summary>
        /// Mailto prefix text.
        /// </summary>
        private readonly static char[] MAILTO_PREFIX = "mailto:".ToCharArray();

        /// <summary>
        /// Relative slash text.
        /// </summary>
        private readonly static char[] RELATIVE_SLASH = "/".ToCharArray();

        /// <summary>
        /// Empty text.
        /// </summary>
        private readonly static char[] EMPTY_VALUE = "".ToCharArray();

        /// <summary>
        /// Url style text.
        /// </summary>
        private readonly static char[] URL_STYLE = "url(".ToCharArray();

        /// <summary>
        /// Parent directory text.
        /// </summary>
        private readonly static char[] PARENT_DIRECTORY = "../".ToCharArray();

        /// <summary>
        /// The image prefix.
        /// </summary>
        private byte[] ImagePrefix;

        /// <summary>
        /// The Javascript prefix.
        /// </summary>
        private byte[] JavascriptPrefix;

        /// <summary>
        /// The CSS prefix.
        /// </summary>
        private byte[] CssPrefix;

        /// <summary>
        /// The URL prefix.
        /// </summary>
        private byte[] UrlPrefix;

        /// <summary>
        /// The response stream.
        /// </summary>
        Stream ResponseStream;

        /// <summary>
        /// The encoding/
        /// </summary>
        Encoding Encoding;

        /// <summary>
        /// Holds characters from last Write(...) call where the start tag did not
        /// end and thus the remaining characters need to be preserved in a buffer so 
        /// that a complete tag can be parsed
        /// </summary>
        char[] PendingBuffer = null;

        /// <summary>
        /// A stringbuilder for debugging.
        /// </summary>
        StringBuilder Debug = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticContentFilter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="domainPrefix">The domain prefix.</param>
        /// <param name="urlPrefix">The URL prefix.</param>
        public StaticContentFilter(Stream stream, System.Text.Encoding encoding, string domainPrefix, string urlPrefix)
        {
            Encoding = encoding;
            ResponseStream = new MemoryStream();

            domainPrefix = domainPrefix.Replace(" ", "%20");
            urlPrefix = urlPrefix.Replace(" ", "%20");
            ImagePrefix = Encoding.GetBytes(domainPrefix);
            JavascriptPrefix = Encoding.GetBytes(domainPrefix);
            CssPrefix = Encoding.GetBytes(domainPrefix);
            UrlPrefix = Encoding.GetBytes(urlPrefix);
        }

        #region Filter overrides

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>true if the stream supports reading; otherwise, false.
        ///   </returns>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>true if the stream supports seeking; otherwise, false.
        ///   </returns>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>true if the stream supports writing; otherwise, false.
        ///   </returns>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
        /// </summary>
        public override void Close()
        {
            FlushPendingBuffer();
            ResponseStream.Close();
        }

        /// <summary>
        /// Flushes the pending buffer.
        /// </summary>
        private void FlushPendingBuffer()
        {
            /// Some characters were left in the buffer 
            if (null != PendingBuffer)
            {
                WriteOutput(PendingBuffer, 0, PendingBuffer.Length);
                PendingBuffer = null;
            }
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        ///   </exception>
        public override void Flush()
        {
            FlushPendingBuffer();
            ResponseStream.Flush();
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        ///   </returns>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// A class derived from Stream does not support seeking.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        ///   </exception>
        public override long Length
        {
            get { return 0; }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        ///   </returns>
        ///   
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support seeking.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        ///   </exception>
        ///   

        private long lngPostion = 0;
        public override long Position
        {
            // return a default value - returning the Position itself was causing a recusion error.
            get { return lngPostion; }
            set { lngPostion = value; }
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        ///   </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return ResponseStream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length.
        /// </summary>
        /// <param name="length">The length.</param>
        public override void SetLength(long length)
        {
            ResponseStream.SetLength(length);
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="buffer"/> is null.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="offset"/> or <paramref name="count"/> is negative.
        ///   </exception>
        ///   
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support reading.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        ///   </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return ResponseStream.Read(buffer, offset, count);
        }

        #endregion

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="T:System.ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the buffer length.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="buffer"/> is null.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="offset"/> or <paramref name="count"/> is negative.
        ///   </exception>
        ///   
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs.
        ///   </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support writing.
        ///   </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        ///   </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            char[] content;
            char[] charBuffer = Encoding.GetChars(buffer, offset, count);

            /// If some bytes were left for processing during last Write call
            /// then consider those into the current buffer
            if (null != PendingBuffer)
            {
                content = new char[charBuffer.Length + PendingBuffer.Length];
                Array.Copy(PendingBuffer, 0, content, 0, PendingBuffer.Length);
                Array.Copy(charBuffer, 0, content, PendingBuffer.Length, charBuffer.Length);
                PendingBuffer = null;
            }
            else
            {
                content = charBuffer;
            }

            String strContentArray = new String(content);

            int lastPosWritten = 0;
            for (int pos = 0; pos < content.Length; pos++)
            {
                // See if tag start
                char c = content[pos];
                if ('<' == c)
                {
                    pos++;
                    /* Make sure there are enough characters available in the buffer to finish 
                     * tag start. This will happen when a tag partially starts but does not end
                     * For example, a partial img tag like <img                    
                     * We need a complete tag upto the > character.
                    */
                    if (HasTagEnd(content, pos))
                    {
                        if ('/' == content[pos])
                        {

                        }
                        else
                        {
                            if (HasMatch(content, pos, BASE_TAG))
                            {
                                string href = FindBaseHref(HREF_ATTRIBUTE, content, pos, lastPosWritten);
                                href = ParentDir(href);
                                UrlPrefix = Encoding.GetBytes(href);
                                ParentDir(href);
                            }
                            else if (HasMatch(content, pos, IMG_TAG))
                            {
                                if (!HasStringMatch(content, pos, INLINE_IMG_TAG))
                                    lastPosWritten = WritePrefixIf(SRC_ATTRIBUTE, content, pos, lastPosWritten, ImagePrefix, UrlPrefix);
                            }
                            else if (HasMatch(content, pos, ANCHOR_TAG))
                            {
                                lastPosWritten = WritePrefixIf(HREF_ATTRIBUTE, content, pos, lastPosWritten, ImagePrefix, UrlPrefix);
                            }
                            else if (HasMatch(content, pos, SCRIPT_TAG))
                            {
                                lastPosWritten = WritePrefixIf(SRC_ATTRIBUTE, content, pos, lastPosWritten, JavascriptPrefix, UrlPrefix);
                            }
                            else if (HasMatch(content, pos, LINK_TAG))
                            {
                                lastPosWritten = WritePrefixIf(HREF_ATTRIBUTE, content, pos, lastPosWritten, CssPrefix, UrlPrefix);
                            }
                            else
                            {
                                lastPosWritten = WritePrefixIf(STYLE_ATTRIBUTE, content, pos, lastPosWritten, CssPrefix, UrlPrefix);
                            }

                            // If buffer was written beyond current position, skip
                            // upto the position that was written
                            if (lastPosWritten > pos)
                            {
                                pos = lastPosWritten;
                            }
                        }
                    }
                    else
                    {
                        pos--;
                        // A tag started but it did not end in this buffer. Preserve the content
                        // in a buffer. On next write call, we will take an attempt to check it again.
                        PendingBuffer = new char[content.Length - pos];
                        Array.Copy(content, pos, PendingBuffer, 0, content.Length - pos);

                        // Write from last write position upto pos. the rest is now in pending buffer 
                        // will be processed later.
                        WriteOutput(content, lastPosWritten, pos - lastPosWritten);

                        return;
                    }
                }
            }

            // Write whatever is left in the buffer from last write pos to the end of the buffer.
            WriteOutput(content, lastPosWritten, content.Length - lastPosWritten);
        }

        private int FindAttributeValuePos(char[] attributeName, char[] content, int pos)
        {
            for (int i = pos; i < content.Length - attributeName.Length; i++)
            {
                // Tag closing reached but the attribute was not found.
                if ('>' == content[i]) return -1;

                if (HasMatch(content, i, attributeName))
                {
                    pos = i + attributeName.Length;

                    // Find the position of the double quote from where value is started
                    // We won't allow value without double quote, not even single quote. 
                    // The content must be XHTML valid for now. 
                    while ((content.Length > pos))
                    {
                        Boolean notFound = (('"' != content[pos]) && ('\'' != content[pos]));
                        if (notFound)
                        {
                            pos++;
                        }
                        else
                        {
                            pos++;
                            break;
                        }
                    };

                    return pos;
                }
            }

            return -1;
        }

        private int FindUrlValuePos(char[] attributeName, char[] content, int pos)
        {
            for (int i = pos; i < content.Length - attributeName.Length; i++)
            {
                // Tag closing reached but the attribute was not found
                if ('"' == content[i]) return -1;
                if ('\'' == content[i]) return -1;

                if (HasMatch(content, i, attributeName))
                {
                    pos = i + attributeName.Length;

                    return pos;
                }
            }

            return -1;
        }

        /// <summary>
        /// Write the prefix if the specified attribute was found and the attribute has a value
        /// that does not start with http:// prefix.
        /// If atttribute is not found, it just returns the lastWritePos as it is
        /// If attribute was found but the attribute already has a fully qualified URL, then return lastWritePos as it is
        /// If attribute has relative URL, then lastWritePos is the starting position of the attribute value. However,
        /// content from lastWritePos to position of the attribute value will already be written to output
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="content"></param>
        /// <param name="pos"></param>
        /// <param name="lastWritePos"></param>
        /// <param name="prefix"></param>
        /// <returns>The last position upto which content was written.</returns>
        private int WritePrefixIf(char[] attributeName, char[] content, int pos, int lastWritePos, byte[] prefix, byte[] pathPrefix)
        {
            String strContentArray = new String(content);
            // Write upto the position where image source tag comes in.
            int attributeValuePos = FindAttributeValuePos(attributeName, content, pos);

            if (attributeName == STYLE_ATTRIBUTE)
            {
                if (attributeValuePos > 0)
                {
                    // Write upto the position where url(: comes in.
                    attributeValuePos = FindUrlValuePos(URL_STYLE, content, attributeValuePos);
                }
            }

            while ((content.Length > pos) && ('"' != content[pos++])) ;

            // Ensure attribute was found.
            if (attributeValuePos > 0)
            {
                string preview = strContentArray.Substring(attributeValuePos - 1);
                string test = string.Empty;
                if (preview.Contains("2_UN621")) test = "test";

                if (HasMatch(content, attributeValuePos, HTTP_PREFIX))
                {
                    // We already have an absolute URL. So, nothing to do.
                    return lastWritePos;
                }
                else if (HasMatch(content, attributeValuePos, JAVASCRIPT_PREFIX))
                {
                    // We already have an absolute URL. So, nothing to do.
                    return lastWritePos;
                }
                else if (HasMatch(content, attributeValuePos, MAILTO_PREFIX))
                {
                    // We already have an absolute URL. So, nothing to do.
                    return lastWritePos;
                }
                else if (HasMatch(content, attributeValuePos, RELATIVE_SLASH))
                {
                    // It's a relative URL. So, let's prefix the URL with the
                    // static domain name.

                    // First, write content upto this position.
                    WriteOutput(content, lastWritePos, attributeValuePos - lastWritePos);

                    // Now write the prefix.
                    WriteBytes(prefix, 0, prefix.Length);

                    // Ensure the attribute value does not start with a leading slash because the prefix
                    // is supposed to have a trailing slash. If value does start with a leading slash,
                    // skip it.
                    if ('/' == content[attributeValuePos]) attributeValuePos++;

                    return attributeValuePos;
                }
                else if (HasMatch(content, attributeValuePos, PARENT_DIRECTORY))
                {
                    // First, write content upto this position.
                    WriteOutput(content, lastWritePos, attributeValuePos - lastWritePos);
                    string strPrefix = Encoding.GetString(pathPrefix);

                    // It's a relative URL. So, let's prefix the URL with the
                    // static domain name.
                    while (HasMatch(content, attributeValuePos, PARENT_DIRECTORY))
                    {
                        strPrefix = ParentDir(strPrefix);
                        pathPrefix = Encoding.GetBytes(strPrefix);

                        attributeValuePos += 3;
                    }

                    // Now write the relative path prefix .
                    WriteBytes(pathPrefix, 0, pathPrefix.Length);

                    return attributeValuePos;
                }

                else
                {
                    // First, write content upto this position.
                    WriteOutput(content, lastWritePos, attributeValuePos - lastWritePos);

                    // Now write the relative path prefix.
                    WriteBytes(pathPrefix, 0, pathPrefix.Length);

                    // We have a local reference. So stitch on the full path.
                    return attributeValuePos; ;
                }
            }
            else
            {
                return lastWritePos;
            }
        }

        /// <summary>
        /// Determines whether the specified content has match.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="pos">The pos.</param>
        /// <param name="match">The match.</param>
        /// <returns>
        ///   <c>true</c> if the specified content has match; otherwise, <c>false</c>.
        /// </returns>
        private bool HasMatch(char[] content, int pos, char[] match)
        {
            for (int i = 0; i < match.Length; i++)
            {
                if ((content.Count() > pos)
                    && (content[pos + i] != match[i]
                    && content[pos + i] != char.ToUpper(match[i])))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified content has match.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="pos">The pos.</param>
        /// <param name="match">The match.</param>
        /// <returns>
        ///   <c>true</c> if the specified content has match; otherwise, <c>false</c>.
        /// </returns>
        private bool HasStringMatch(char[] content, int pos, char[] match)
        {
            string sContent = new string(content);
            string sMatch = new string(match);
            int iPos = sContent.IndexOf(">", pos);
            
            string sTag = sContent.Substring(pos, iPos - pos);
            if (iPos > 0)
            {
                if (sTag.IndexOf(sMatch) > 0)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Determines whether the specified content has a tag end.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="pos">The pos.</param>
        /// <returns>
        ///   <c>true</c> if the specified content has a tag end; otherwise, <c>false</c>.
        /// </returns>
        private bool HasTagEnd(char[] content, int pos)
        {
            for (; pos < content.Length; pos++)
            {
                if ('>' == content[pos])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Writes the output.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="pos">The pos.</param>
        /// <param name="length">The length.</param>
        private void WriteOutput(char[] content, int pos, int length)
        {
            if (length == 0)
            {
                return;
            }

            Debug.Append(content, pos, length);
            byte[] buffer = Encoding.GetBytes(content, pos, length);
            WriteBytes(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes the output.
        /// </summary>
        /// <param name="content">The content.</param>
        private void WriteOutput(string content)
        {
            Debug.Append(content);
            byte[] buffer = Encoding.GetBytes(content);
            WriteBytes(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Gets the parent of a path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private string ParentDir(string path)
        {
            Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
            return GetParentUriString(myUri);
        }

        /// <summary>
        /// Gets the parent URI string.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        private static string GetParentUriString(Uri uri)
        {
            return uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments[uri.Segments.Length - 1].Length - uri.Query.Length);
        }

        /// <summary>
        /// Writes the bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="pos">The pos.</param>
        /// <param name="length">The length.</param>
        private void WriteBytes(byte[] bytes, int pos, int length)
        {
            ResponseStream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Finds the base href.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="content">The content.</param>
        /// <param name="pos">The pos.</param>
        /// <param name="lastWritePos">The last write pos.</param>
        /// <returns></returns>
        private string FindBaseHref(char[] attributeName, char[] content, int pos, int lastWritePos)
        {
            // Write up to the position where image source tag comes in.
            int attributeValuePos = FindAttributeValuePos(attributeName, content, pos);
            string href = "";

            while ((content.Length > attributeValuePos))
            {
                Boolean notFound = (('"' != content[attributeValuePos]) && ('\'' != content[attributeValuePos]));
                if (notFound)
                {
                    href = href + content[attributeValuePos];
                    attributeValuePos++;
                }
                else
                {
                    break;
                }
            };

            return href;
        }      
    }
}