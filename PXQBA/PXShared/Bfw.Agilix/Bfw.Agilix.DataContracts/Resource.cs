using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents a resource that can be stored or loaded in agilix
    /// </summary>
    public class Resource : IDlapEntityTransformer, IDisposable
    {
        #region Properties

        /// <summary>
        /// Id of the entity the resource will live under.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Path to the resource.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Extension of the underlying file type, e.g. html, pdf, etc.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// stores the resource's binary data, if any.
        /// </summary>
        public MemoryStream ResourceStream { get; set; }

        /// <summary>
        /// The content type of the resource, defaults to empty but should be set
        /// to one of the values defined by ResourceContentType.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Determines if a resource is hidden or normal (default).
        /// </summary>
        public ResourceStatus Status { get; set; }
        
        /// <summary>
        /// Date that resource created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Date that resource created.
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a resource with an empty stream and a status of Normal.
        /// </summary>
        public Resource()
        {
            Status = ResourceStatus.Normal;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a read/write/seek capable stream that holds the resource's 
        /// binary data, if any. The stream returned will be set to the first
        /// byte (i.e. offset 0 from SeekOrigin.Begin).
        /// </summary>
        /// <returns>Stream containing the bindary resource.</returns>
        public Stream GetStream()
        {
            if (null == ResourceStream)
            {
                ResourceStream = new MemoryStream();
            }

            ResourceStream.Seek(0, SeekOrigin.Begin);

            return ResourceStream;
        }

        /// <summary>
        /// Parses metadata about a resource from an XML element.
        /// </summary>
        /// <param name="element">The element containing the resource metadata.</param>
        /// <remarks></remarks>
        public void ParseEntity(System.Xml.Linq.XElement element)
        {
            var entityid = element.Attribute(ElStrings.Entityid);
            var url = element.Attribute(ElStrings.Path);
            var createdDate = element.Attribute(ElStrings.CreationDate);
            var modifiedDate = element.Attribute(ElStrings.ModifiedDate);

            if (null != entityid)
            {
                EntityId = entityid.Value;
            }

            if (null != url)
            {
                Url = url.Value;
            }

            if (null != createdDate)
            {
                CreationDate = DateTime.Parse(createdDate.Value);
            }

            if (null != modifiedDate)
            {
                ModifiedDate = DateTime.Parse(modifiedDate.Value);
            }
        }

        #endregion

        #region IDlapEntityTransformer Members

        /// <summary>
        /// Transforms internal object state into an XElement representation of a DLAP entity
        /// </summary>
        /// <returns>XElement containing the transformed object state</returns>
        /// <remarks></remarks>
        public XElement ToEntity()
        {
            var root = new XElement(ElStrings.Resource,
                new XAttribute(ElStrings.Entityid, EntityId),
                new XAttribute(ElStrings.Path, Url));

            return root;
        }        

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        /// <remarks></remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ResourceStream != null)
                {
                    try
                    {
                        ResourceStream.Close();
                    }
                    catch { }
                }
            }
        }

        #endregion
    }
}