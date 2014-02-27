using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words;
using Aspose.Words.Saving;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Ionic.Zip;

namespace Bfw.PX.Biz.Direct.Services
{
    public class AsposeDocumentConverter : IDocumentConverter
    {
        #region IDocumentConverter Members

        /// <summary>
        /// Converts input document stream to requested file format using Aspose.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public Stream ConvertDocument(DocumentConversion document)
        {
            return ConvertDocuments(new List<DocumentConversion> { document });
        }

        /// <summary>
        /// Gets a word count from a <see cref="Stream"/> object.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public string GetDocumentWordCount(Stream document)
        {
            document.Seek(0, 0);
            var doc = new Document(document);
            doc.UpdateWordCount();
            return doc.BuiltInDocumentProperties.Words.ToString();
        }

        /// <summary>
        /// Converts input documents streams to zip file stream using Aspose.
        /// </summary>
        /// <param name="documents"></param>
        public Stream ConvertDocuments(IEnumerable<DocumentConversion> documents)
        {
            if (documents.Count() == 1)
            {
                return GetStream(documents.FirstOrDefault());
            }

            var zip = new ZipFile();
            foreach (var document in documents)
            {
                var fileName = document.FileName.Replace(" ", "_");
                if (zip[fileName] != null)
                    fileName += "_" + Guid.NewGuid();
                var stream = GetStream(document);
                zip.AddEntry(string.Format("{0}.{1}", fileName, document.OutputType.ToString().ToLower()), stream);
            }
            var zipStream = new MemoryStream();
            zip.Save(zipStream);
            zipStream.Seek(0, 0);
            return zipStream;
        }

        #endregion

        /// <summary>
        /// Gets a stream for the specified document information.
        /// Converts the content if the requested output type is HTML.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        private Stream GetStream(DocumentConversion document)
        {
            if (document.OutputType == DocumentOutputType.Html)
            {
                if (document.FileName.Contains(".pdf"))
                {
                    return GetPDFToHtmlStream(document.DataStream);
                }
                else
                {
                    return GetHtmlStream(document.DataStream);
                }
            }
            return GetDocumentStream(document);
        }

        /// <summary>
        /// Gets a stream for the specified document information.
        /// </summary>
        /// <param name="document">Document upload information.</param>
        /// <returns></returns>
        private Stream GetDocumentStream(DocumentConversion document)
        {
            Document doc;
            if (document.DataStream.Length == 0)
            {
                doc = new Document();
                var builder = new DocumentBuilder(doc);
                builder.InsertHtml("<div></div>");
            }
            else
            {
                var loadform = GetLoadFormat(document.OutputType.ToString().ToLowerInvariant());
                doc = new Document(document.DataStream, new LoadOptions(loadform, "", ""));
            }
            var outputStream = new MemoryStream();
            doc.Save(outputStream, GetAsposeFormat(document.OutputType));
            outputStream.Seek(0, 0);
            return outputStream;
        }


        private LoadFormat GetLoadFormat(string outputType)
        {
            LoadFormat format = new LoadFormat();

            switch (outputType)
            {
                case "doc":

                    format = LoadFormat.Doc;
                    break;
                case "docx":

                    format = LoadFormat.Docx;
                    break;

                default:
                    format = LoadFormat.Auto;
                    break;
            }
            return format;

        }

        /// <summary>
        /// Convert the content of a stream to HTML and returns a new stream.
        /// </summary>
        /// <param name="source">The source.</param>
        private Stream GetHtmlStream(Stream source)
        {
            var outputStream = new MemoryStream();
            var doc = new Document(source);
            var options = new HtmlSaveOptions(SaveFormat.Html)
            {
                ExportHeadersFooters = true,
                ExportXhtmlTransitional = true,
                ExportImagesAsBase64 = true,
                ImageSavingCallback = new HandleImageSaving()
            };

            doc.Save(outputStream, options);
            outputStream.Seek(0, 0);
            return outputStream;
        }

        /// <summary>
        /// Convert the content of a stream to HTML and returns a new stream.
        /// </summary>
        /// <param name="source">The source.</param>
        private Stream GetPDFToHtmlStream(Stream source)
        {
            var doc = new Aspose.Pdf.Document(source);
            var filename = System.IO.Path.GetTempFileName();

            var oStream = new MemoryStream();
            FileStream oFileStream = null;
            try
            {
                doc.Save(filename, Aspose.Pdf.SaveFormat.Html);
                //doc.Save(oStream, Aspose.Pdf.SaveFormat.Html);

                oFileStream = new FileStream(filename, FileMode.Open);
                byte[] bytes = new byte[oFileStream.Length];
                oFileStream.Read(bytes, 0, (int)oFileStream.Length);
                oStream.Write(bytes, 0, (int)oFileStream.Length);
            }
            finally
            {
                oFileStream.Close();
                if (File.Exists(filename)) File.Delete(filename);
            }

            oStream.Seek(0, 0);
            return oStream;
        }

        /// <summary>
        /// Gets the aspose save format based off the document output type.
        /// </summary>
        /// <param name="outputType">Type of the output.</param>
        /// <returns></returns>
        private SaveFormat GetAsposeFormat(DocumentOutputType outputType)
        {
            switch (outputType)
            {
                case DocumentOutputType.Doc:
                    return SaveFormat.Doc;
                case DocumentOutputType.Docx:
                    return SaveFormat.Docx;
                case DocumentOutputType.Pdf:
                    return SaveFormat.Pdf;
                case DocumentOutputType.Html:
                    return SaveFormat.Html;
                default:
                    return SaveFormat.Doc;
            }
        }
    }
    public class HandleImageSaving : IImageSavingCallback
    {

        void IImageSavingCallback.ImageSaving(ImageSavingArgs e)
        {

            e.ImageStream = new MemoryStream();

            e.KeepImageStreamOpen = false;

        }

    }
}