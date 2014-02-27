using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
//using System.Web.Security;
using System.Web.UI;
//using System.Web.UI.HtmlControls;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace Bfw.PX.Comments.Data
{
    public class StaticContentFilter : Stream
    {
        private readonly static char[] BASE_TAG = "base".ToCharArray();
        private readonly static char[] IMG_TAG = "img".ToCharArray();
        private readonly static char[] ANCHOR_TAG = "a".ToCharArray();
        private readonly static char[] SCRIPT_TAG = "script".ToCharArray();
        private readonly static char[] LINK_TAG = "link".ToCharArray();
        private readonly static char[] SRC_ATTRIBUTE = "src".ToCharArray();
        private readonly static char[] HREF_ATTRIBUTE = "href".ToCharArray();
        private readonly static char[] STYLE_ATTRIBUTE = "style".ToCharArray();
        private readonly static char[] HTTP_PREFIX = "http://".ToCharArray();
        private readonly static char[] JAVASCRIPT_PREFIX = "javascript:".ToCharArray();
        private readonly static char[] RELATIVE_SLASH = "/".ToCharArray();
        private readonly static char[] EMPTY_VALUE = "".ToCharArray();
        private readonly static char[] URL_STYLE = "url(".ToCharArray();

        private readonly static char[] PARENT_DIRECTORY = "../".ToCharArray();

        private byte[] _ImagePrefix;
        private byte[] _JavascriptPrefix;
        private byte[] _CssPrefix;

        private byte[] _urlPrefix;

        Stream _ResponseStream;
        long _Position;

        Encoding _Encoding;

        /// <summary>
        /// Holds characters from last Write(...) call where the start tag did not
        /// end and thus the remaining characters need to be preserved in a buffer so 
        /// that a complete tag can be parsed
        /// </summary>
        char[] _PendingBuffer = null;

        StringBuilder debug = new StringBuilder();

        //public StaticContentFilter(HttpResponse response, string imagePrefix, string javascriptPrefix, string cssPrefix)
        //{
        //    this._Encoding = response.Output.Encoding;
        //    this._ResponseStream = response.Filter;

        //    this._ImagePrefix = _Encoding.GetBytes(imagePrefix);
        //    this._JavascriptPrefix = _Encoding.GetBytes(javascriptPrefix);
        //    this._CssPrefix = _Encoding.GetBytes(cssPrefix);
        //}

        public StaticContentFilter(Stream stream, System.Text.Encoding encoding, string domainPrefix, string urlPrefix)
        {
            this._Encoding = encoding;
            this._ResponseStream = new MemoryStream();
        
            domainPrefix = domainPrefix.Replace(" ", "%20");
            urlPrefix = urlPrefix.Replace(" ", "%20");
            //domainPrefix = HttpUtility.UrlEncode(domainPrefix);
            //urlPrefix = HttpUtility.UrlEncode(urlPrefix);


            this._ImagePrefix = _Encoding.GetBytes(domainPrefix);
            this._JavascriptPrefix = _Encoding.GetBytes(domainPrefix);
            this._CssPrefix = _Encoding.GetBytes(domainPrefix);

            this._urlPrefix = _Encoding.GetBytes(urlPrefix);
        }

        #region Filter overrides
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Close()
        {
            this.FlushPendingBuffer();
            _ResponseStream.Close();
        }

        private void FlushPendingBuffer()
        {
            /// Some characters were left in the buffer 
            if (null != this._PendingBuffer)
            {
                this.WriteOutput(this._PendingBuffer, 0, this._PendingBuffer.Length);
                this._PendingBuffer = null;
            }
        }

        public override void Flush()
        {
            this.FlushPendingBuffer();
            _ResponseStream.Flush();
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _ResponseStream.Seek(offset, origin);
        }

        public override void SetLength(long length)
        {
            _ResponseStream.SetLength(length);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _ResponseStream.Read(buffer, offset, count);
        }
        #endregion

        public override void Write(byte[] buffer, int offset, int count)
        {
            char[] content;
            char[] charBuffer = this._Encoding.GetChars(buffer, offset, count);

            /// If some bytes were left for processing during last Write call
            /// then consider those into the current buffer
            if (null != this._PendingBuffer)
            {
                content = new char[charBuffer.Length + this._PendingBuffer.Length];
                Array.Copy(this._PendingBuffer, 0, content, 0, this._PendingBuffer.Length);
                Array.Copy(charBuffer, 0, content, this._PendingBuffer.Length, charBuffer.Length);
                this._PendingBuffer = null;
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
                            //string preview = _Encoding.GetString(_Encoding.GetBytes(content), pos, 100);

                            if (HasMatch(content, pos, BASE_TAG))
                            {
                                string href = this.FindBaseHref(HREF_ATTRIBUTE,
                                    content, pos, lastPosWritten);


                                href = this.parentDir(href);
                                this._urlPrefix = _Encoding.GetBytes(href);
                                this.parentDir(href);
                            }
                            else if (HasMatch(content, pos, IMG_TAG))
                            {
                                lastPosWritten = this.WritePrefixIf(SRC_ATTRIBUTE,
                                    content, pos, lastPosWritten, this._ImagePrefix, this._urlPrefix);
                            }
                            else if (HasMatch(content, pos, ANCHOR_TAG))
                            {
                                lastPosWritten = this.WritePrefixIf(HREF_ATTRIBUTE,
                                    content, pos, lastPosWritten, this._ImagePrefix, this._urlPrefix);
                            }
                            else if (HasMatch(content, pos, SCRIPT_TAG))
                            {
                                lastPosWritten = this.WritePrefixIf(SRC_ATTRIBUTE,
                                    content, pos, lastPosWritten, this._JavascriptPrefix, this._urlPrefix);
                            }
                            else if (HasMatch(content, pos, LINK_TAG))
                            {
                                lastPosWritten = this.WritePrefixIf(HREF_ATTRIBUTE,
                                    content, pos, lastPosWritten, this._CssPrefix, this._urlPrefix);
                            }
                            else 
                            {
                                lastPosWritten = this.WritePrefixIf(STYLE_ATTRIBUTE,
                                    content, pos, lastPosWritten, this._CssPrefix, this._urlPrefix);
                            }


                            // If buffer was written beyond current position, skip
                            // upto the position that was written
                            if (lastPosWritten > pos)
                                pos = lastPosWritten;
                        }
                    }
                    else
                    {
                        pos--;
                        // a tag started but it did not end in this buffer. Preserve the content
                        // in a buffer. On next write call, we will take an attempt to check it again
                        this._PendingBuffer = new char[content.Length - pos];
                        Array.Copy(content, pos, this._PendingBuffer, 0, content.Length - pos);

                        // Write from last write position upto pos. the rest is now in pending buffer 
                        // will be processed later
                        this.WriteOutput(content, lastPosWritten, pos - lastPosWritten);

                        return;
                    }
                }
            }

            // Write whatever is left in the buffer from last write pos to the end of the buffer
            this.WriteOutput(content, lastPosWritten, content.Length - lastPosWritten);
        }

        private int FindAttributeValuePos(char[] attributeName, char[] content, int pos)
        {
            for (int i = pos; i < content.Length - attributeName.Length; i++)
            {
                // Tag closing reached but the attribute was not found
                if ('>' == content[i]) return -1;

                if (HasMatch(content, i, attributeName))
                {
                    pos = i + attributeName.Length;

                    // find the position of the double quote from where value is started
                    // We won't allow value without double quote, not even single quote. 
                    // The content must be XHTML valid for now. 
                    while ((content.Length > pos) && ('"' != content[pos++]) && ('\'' != content[pos]))
                    {
                        
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

                    // find the position of the double quote from where value is started
                    // We won't allow value without double quote, not even single quote. 
                    // The content must be XHTML valid for now. 
                    //while ((content.Length > pos) && ('"' != content[pos++]) && ('\'' != content[pos]))
                    //{

                    //};

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
            // write upto the position where image source tag comes in
            int attributeValuePos = this.FindAttributeValuePos(attributeName, content, pos);

            if (attributeName == STYLE_ATTRIBUTE)
            {
                if (attributeValuePos > 0)
                {
                    // write upto the position where url(: comes in
                    attributeValuePos = this.FindUrlValuePos(URL_STYLE, content, attributeValuePos);
                }
            
            }



            while ((content.Length > pos) && ('"' != content[pos++])) ;

            // ensure attribute was found
            if (attributeValuePos > 0)
            {
                if (HasMatch(content, attributeValuePos, HTTP_PREFIX))
                {
                    // We already have an absolute URL. So, nothing to do
                    return lastWritePos;
                }
                else if (HasMatch(content, attributeValuePos, JAVASCRIPT_PREFIX))
                {
                    // We already have an absolute URL. So, nothing to do
                    return lastWritePos;
                }
                else if (HasMatch(content, attributeValuePos, RELATIVE_SLASH))
                {
                    // It's a relative URL. So, let's prefix the URL with the
                    // static domain name

                    // First, write content upto this position
                    this.WriteOutput(content, lastWritePos, attributeValuePos - lastWritePos);

                    
                        // Now write the prefix 
                        this.WriteBytes(prefix, 0, prefix.Length);

                        // Ensure the attribute value does not start with a leading slash because the prefix
                        // is supposed to have a trailing slash. If value does start with a leading slash,
                        // skip it
                        if ('/' == content[attributeValuePos]) attributeValuePos++;                                                         

                    
                    return attributeValuePos;
                }
                else if (HasMatch(content, attributeValuePos, PARENT_DIRECTORY))
                {
                    // First, write content upto this position
                    this.WriteOutput(content, lastWritePos, attributeValuePos - lastWritePos);
                    string strPrefix = _Encoding.GetString(pathPrefix);
                    
                    //string preview = _Encoding.GetString(_Encoding.GetBytes(content), attributeValuePos, 20);
                    
                    // It's a relative URL. So, let's prefix the URL with the
                    // static domain name
                    while (HasMatch(content, attributeValuePos, PARENT_DIRECTORY))
                    {
                        strPrefix = this.parentDir(strPrefix);
                        pathPrefix = _Encoding.GetBytes(strPrefix);

                        attributeValuePos += 3;
                    }

                    // Now write the relative path prefix 
                    this.WriteBytes(pathPrefix, 0, pathPrefix.Length);   
                    
                    return attributeValuePos;
                }

                else
                {
                    // First, write content upto this position
                    this.WriteOutput(content, lastWritePos, attributeValuePos - lastWritePos);

                    // Now write the relative path prefix 
                    this.WriteBytes(pathPrefix, 0, pathPrefix.Length);

                    // We have a local reference. So stitch on the full path
                    return attributeValuePos;;
                }
            }
            else
            {
                return lastWritePos;
            }
        }

        private bool HasMatch(char[] content, int pos, char[] match)
        {
            for (int i = 0; i < match.Length; i++)
                if  ((content.Count() > pos) 
                    && (content[pos + i] != match[i]
                    && content[pos + i] != char.ToUpper(match[i])))
                    return false;

            return true;
        }

        private bool HasTagEnd(char[] content, int pos)
        {
            for (; pos < content.Length; pos++)
                if ('>' == content[pos])
                    return true;

            return false;
        }

        private void WriteOutput(char[] content, int pos, int length)
        {
            if (length == 0) return;

            debug.Append(content, pos, length);
            byte[] buffer = this._Encoding.GetBytes(content, pos, length);
            this.WriteBytes(buffer, 0, buffer.Length);
        }
        private void WriteOutput(string content)
        {
            debug.Append(content);
            byte[] buffer = this._Encoding.GetBytes(content);
            this.WriteBytes(buffer, 0, buffer.Length);
        }

        private string parentDir(string path)
        {
            Uri myUri = new Uri(path, UriKind.RelativeOrAbsolute);
            return GetParentUriString(myUri);
        }

        private static string GetParentUriString(Uri uri)
        {
            return uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments[uri.Segments.Length - 1].Length - uri.Query.Length);
        }





        private void WriteBytes(byte[] bytes, int pos, int length)
        {
            this._ResponseStream.Write(bytes, 0, bytes.Length);
        }




        /// <summary>
        /// </summary>
        private string FindBaseHref(char[] attributeName, char[] content, int pos, int lastWritePos)
        {
            // write upto the position where image source tag comes in
            int attributeValuePos = this.FindAttributeValuePos(attributeName, content, pos);
            string href = "";

            while ((content.Length > attributeValuePos) && ('"' != content[attributeValuePos++]) && ('\'' != content[attributeValuePos]))
            {
                href = href + content[attributeValuePos];
                
            };

            return href;

        }
    }
}