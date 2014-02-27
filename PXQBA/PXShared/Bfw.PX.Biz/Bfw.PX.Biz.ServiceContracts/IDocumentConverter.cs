using System.IO;
using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that manipulate documents.
    /// </summary>
    public interface IDocumentConverter
    {
        /// <summary>
        /// Creates a stream from a document uploaded via an upload transaction.
        /// </summary>
        /// <param name="document">Document upload transaction info such as file name and output type.</param>
        /// <returns></returns>
        Stream ConvertDocument(DocumentConversion document);

        /// <summary>
        /// Gets a word count from a <see cref="Stream" /> object.
        /// </summary>
        /// <param name="document">The document.</param>
        string GetDocumentWordCount(Stream document);

        /// <summary>
        /// Creates a <see cref="ZipStream" /> from a collection of documents to upload.
        /// </summary>
        /// <param name="documents">The documents.</param>
        Stream ConvertDocuments(IEnumerable<DocumentConversion> documents);
    }
}