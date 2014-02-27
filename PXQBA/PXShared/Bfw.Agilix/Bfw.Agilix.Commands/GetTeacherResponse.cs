using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Ionic.Zip;
using System.Xml;


namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/GetTeacherResponse command.
    /// </summary>
    public class GetTeacherResponse : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>The search parameters.</value>
        /// <remarks>Filters the teacher responses.</remarks>
        public TeacherResponseSearch SearchParameters { get; set; }

        /// <summary>
        /// Gets or sets the teacher response.
        /// </summary>
        /// <value>The teacher response.</value>
        /// <remarks>Reponse that matched <see cref="SearchParameters" /></remarks>
        public TeacherResponse TeacherResponse { get; protected set; }

        #endregion

        #region override GetTeacherResponse

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetTeacherResponse command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetTeacherResponse</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            if (string.IsNullOrEmpty(SearchParameters.EnrollmentId))
                throw new DlapException("GetTeacherResponse requires an enrollment id");

            if (string.IsNullOrEmpty(SearchParameters.ItemId))
                throw new DlapException("GetTeacherResponse requires an item id");

            var request = new DlapRequest
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object> {
                    { "cmd", "getteacherresponse" },
                    { "enrollmentid", SearchParameters.EnrollmentId},
                    { "itemid", SearchParameters.ItemId },
                    { "packagetype", "zip" }
                }
            };

            if (SearchParameters.Version != 0)
                request.Parameters.Add("version", SearchParameters.Version);

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetTeacherResponse command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            TeacherResponse = ParseStreamResponse(response.ResponseStream);
        }

        /// <summary>
        /// Parse Stream Response
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private TeacherResponse ParseStreamResponse(Stream response)
        {
            var teacherResponse = new TeacherResponse();

            if (null != response)
            {
                const int bsize = 4096;
                var buff = new byte[bsize];
                int read;
                using (var zipFile = new MemoryStream())
                {
                    response.Position = 0;
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
                        var data = from e in zip.Entries
                                   where System.IO.Path.GetFileName(e.FileName).StartsWith("attachment1")
                                   select e;

                        if (meta != null)
                        {
                            meta.Extract(metaFile);
                            metaFile.Seek(0, SeekOrigin.Begin);
                            XElement element = null;
                            if (metaFile.Length > 0)
                                element = XElement.Load(new XmlTextReader(metaFile, XmlNodeType.Element, null));
                            if (null != element)
                                teacherResponse.ParseEntity(element);
                        }

                        if (data != null && data.Count() > 0)
                        {
                            teacherResponse.ResourceStream = new MemoryStream();
                            data.FirstOrDefault().Extract(dataFile);
                            dataFile.Seek(0, SeekOrigin.Begin);
                            dataFile.CopyTo(teacherResponse.ResourceStream);
                        }

                        metaFile.Close();
                        submissionFile.Close();
                        dataFile.Close();
                    }

                }
            }

            return teacherResponse;
        }       

        #endregion
    }
}
