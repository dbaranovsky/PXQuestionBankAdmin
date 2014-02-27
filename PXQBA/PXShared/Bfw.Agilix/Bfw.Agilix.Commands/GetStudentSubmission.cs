using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Ionic.Zip;

namespace Bfw.Agilix.Commands
{

    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmission command.
    /// </summary>
    public class GetStudentSubmission : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>The search parameters.</value>
        /// <remarks>Parameters used to filter the submissions.</remarks>
        public SubmissionSearch SearchParameters { get; set; }

        /// <summary>
        /// Gets or sets the submission.
        /// </summary>
        /// <value>The submission.</value>
        /// <remarks>Submission retured as the command resposne.</remarks>
        public Submission Submission { get; protected set; }

        /// <summary>
        /// Gets or sets the submission XML.
        /// </summary>
        /// <value>The submission XML.</value>
        /// <remarks>XML of the command response.</remarks>
        public XDocument SubmissionXml { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the response XML.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>XML document containing the resposne body.</returns>
        /// <remarks>Parses the response body as an XML document.</remarks>
        private XDocument GetResponseXml(Stream response)
        {
            XDocument responseDocument = null;

            if (response.Length > 0)
            {
                responseDocument = XDocument.Load(new XmlTextReader(response, XmlNodeType.Element, null));
            }

            return responseDocument;
        }

        /// <summary>
        /// Parses the stream response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Submission read from the ZIP file represented by <see cref="resposne" />.</returns>
        /// <remarks>Response stream is assumed to be a ZIP file conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmission</remarks>
        private Submission ParseStreamResponse(Stream response)
        {
            var submission = new Submission();

            if (null != response)
            {
                const int bsize = 4096;
                var buff = new byte[bsize];
                int read;
                using (var zipFile = new MemoryStream())
                {

                    while ((read = response.Read(buff, 0, bsize)) > 0)
                    {
                        zipFile.Write(buff, 0, read);
                    }

                    zipFile.Seek(0, SeekOrigin.Begin);

                    if (zipFile.Length > 0)
                    {
                        var zip = ZipFile.Read(zipFile);
                        var metaFile = new MemoryStream();
                        var submissionFile = new MemoryStream();
                        var dataFile = new MemoryStream();
                        var meta = zip["meta.xml"];
                        var submissionContent = zip["submission.htm"];
                        var data = zip["data.xml"];

                        if (meta != null)
                        {
                            meta.Extract(metaFile);
                            metaFile.Seek(0, SeekOrigin.Begin);
                            XElement element = null;
                            if (metaFile.Length > 0)
                                element = XElement.Load(new XmlTextReader(metaFile, XmlNodeType.Element, null));
                            if (null != element)
                                submission.ParseEntity(element);
                        }

                        if (submissionContent != null)
                        {
                            submissionContent.Extract(submissionFile);
                            submissionFile.Seek(0, SeekOrigin.Begin);
                            StreamReader html = null;
                            if (submissionFile.Length > 0)
                                html = new StreamReader(submissionFile);
                            if (null != html)
                                submission.Body = html.ReadToEnd();
                        }

                        if (data != null)
                        {
                            data.Extract(dataFile);
                            MemoryStream streamData = new MemoryStream();
                            dataFile.Seek(0, SeekOrigin.Begin);
                            dataFile.CopyTo(streamData);
                            submission.StreamData = streamData;

                            dataFile.Seek(0, SeekOrigin.Begin);
                            try{
                                submission.Data = XDocument.Load(dataFile);
                            }
                            catch { }
                        }

                        metaFile.Close();
                        submissionFile.Close();
                        dataFile.Close();
                    }

                }
            }

            return submission;
        }        

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmission command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmission</returns>
        public override DlapRequest ToRequest()
        {
            if (string.IsNullOrEmpty(SearchParameters.EnrollmentId))
                throw new DlapException("GetStudentSubmission requires an enrollment id");

            if (string.IsNullOrEmpty(SearchParameters.ItemId))
                throw new DlapException("GetStudentSubmission requires an item id");

            var request = new DlapRequest
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>()
            };



            if (!System.String.IsNullOrEmpty(SearchParameters.PackageType))
            {
                request.Parameters["cmd"] = "getstudentsubmission";
                request.Parameters["enrollmentid"] = SearchParameters.EnrollmentId;
                request.Parameters["itemid"] = SearchParameters.ItemId;
                request.Parameters["packagetype"] = SearchParameters.PackageType;
                request.Parameters["filepath"] = SearchParameters.FilePath;
            }
            else
            {
                request.Parameters["cmd"] = "getstudentsubmission";
                request.Parameters["enrollmentid"] = SearchParameters.EnrollmentId;
                request.Parameters["itemid"] = SearchParameters.ItemId;
                request.Parameters["packagetype"] = "zip";
            }

            if (SearchParameters.Version != 0)
                request.Parameters.Add("version", SearchParameters.Version);

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmission command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (response.ContentType == "text/xml")
            {
                SubmissionXml = GetResponseXml(response.ResponseStream);
            }
            else
            {
                Submission = ParseStreamResponse(response.ResponseStream);
            }
        }

        #endregion        
    }
}
