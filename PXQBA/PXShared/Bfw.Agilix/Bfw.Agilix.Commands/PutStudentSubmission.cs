using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Ionic.Zip;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/PutStudentSubmission command.
    /// </summary>
    public class PutStudentSubmission : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>The entity id.</value>
        /// <remarks></remarks>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the submission.
        /// </summary>
        /// <value>The submission.</value>
        /// <remarks></remarks>
        public Submission Submission { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the zip file that represents the data to post to the
        /// Agilix server
        /// </summary>
        /// <returns></returns>
        private Stream BuildRequestStream()
        {
            var metaFile = new MemoryStream();
            var zipFile = new MemoryStream();
            var zip = new ZipFile();

            var metaFilefSw = new StreamWriter(metaFile);
            var metadata = Submission.ToEntity().ToString();
            metaFilefSw.Write(metadata);
            metaFilefSw.Flush();
            metaFile.Seek(0, SeekOrigin.Begin);
            zip.AddEntry("meta.xml", metaFile);

            if (!string.IsNullOrEmpty(Submission.Body))
            {
                var msgFile = new MemoryStream();
                var msgFileSw = new StreamWriter(msgFile);
                msgFileSw.Write(Submission.Body);
                msgFileSw.Flush();
                msgFile.Seek(0, SeekOrigin.Begin);
                zip.AddEntry("submission.htm", msgFile);
            }

            if (Submission.StreamData != null)
            {
                MemoryStream streamData = new MemoryStream();
                Submission.StreamData.CopyTo(streamData);
                streamData.Seek(0, SeekOrigin.Begin);
                zip.AddEntry("data.xml", streamData);
            }
            else if (Submission.Data != null)
            {
                var dataFile = new MemoryStream();
                var dataFileSw = new StreamWriter(dataFile);
                dataFileSw.Write(Submission.Data.ToString());
                dataFileSw.Flush();
                dataFile.Seek(0, SeekOrigin.Begin);
                zip.AddEntry("data.xml", dataFile);
            }

            zip.Save(zipFile);
            zipFile.Flush();
            zipFile.Seek(0, SeekOrigin.Begin);

            return zipFile;
        }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/PutStudentSubmission command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/PutStudentSubmission</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                ContentType = "application/zip",
                Parameters = new Dictionary<string, object>() {
                    { "cmd", "putstudentsubmission" },
                    { "enrollmentid", Submission.EnrollmentId },
                    { "itemid", Submission.ItemId }
                }
            };
            request.AppendData(BuildRequestStream());

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/PutStudentSubmission command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("PutStudentSubmission failed with code: {0}", response.Code));

            var submission = new Submission();
            submission.ParseEntity(response.ResponseXml.Root);
            Submission.Version = submission.Version;
        }

        #endregion        
    }
}
