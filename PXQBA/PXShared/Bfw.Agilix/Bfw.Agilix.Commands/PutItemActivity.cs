using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/PutItemActivity command
    /// </summary>
    public class PutItemActivity : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the enrollment id.
        /// </summary>
        /// <value>The enrollment id.</value>
        /// <remarks>Enrollment Id of the user to log activity against the item.</remarks>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>The item id.</value>
        /// <remarks>Id of the item whose activity is being logged.</remarks>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        /// <remarks>Time at which the activity started.</remarks>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Indicates that this call marks the start of a new attempt for the student.
        /// </summary>
        /// <value>The attempts.</value>
        /// <remarks>The number of attmempts this activity represents.</remarks>
        public bool NewAttempt { get; set; }

        /// <summary>
        /// Gets or sets the seconds.
        /// </summary>
        /// <value>The seconds.</value>
        /// <remarks>Number of seconds elapsed during the activity.</remarks>
        public int Seconds { get; set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/PutItemActivity command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/PutItemActivity</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            if (!(EnrollmentId.IsNullOrEmpty() || ItemId.IsNullOrEmpty()))
            {
                var cmd = new DlapRequest()
                {
                    Type = DlapRequestType.Post,
                    Mode = DlapRequestMode.Batch,
                    Parameters = new Dictionary<string, object>() {
                    { "cmd", "PutItemActivity" }
                }
                };

                var activity = new XElement("activity",
                            new XAttribute("enrollmentid", EnrollmentId),
                            new XAttribute("itemid", ItemId),
                            new XAttribute("seconds", Seconds));

                if (StartTime.HasValue)
                    activity.Add(new XAttribute("date", StartTime));
                if (NewAttempt)
                    activity.Add(new XAttribute("newattempt", NewAttempt));

                cmd.AppendData(activity);

                return cmd;
            }
            throw new InvalidOperationException("Invalid parameters for PutItemActivity.");
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/PutItemActivity command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks>Note that the response of PutItemActivity is empty.</remarks>
        public override void ParseResponse(DlapResponse response)
        {

        }

        #endregion
    }
}
