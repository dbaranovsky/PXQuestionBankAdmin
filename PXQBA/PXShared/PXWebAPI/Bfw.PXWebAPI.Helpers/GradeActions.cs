using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PXWebAPI.Mappers;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.DTO;
using GetGrades = Bfw.Agilix.Commands.GetGrades;

namespace Bfw.PXWebAPI.Helpers
{
    public class GradeActions
    {
        #region Properties

        protected ISessionManager SessionManager { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GradeActions"/> class.
        /// </summary>
        /// <param name="sessionManager">The session manager.</param>
        public GradeActions(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        #endregion

        public IEnumerable<Adc.Grade> GetGradesByEnrollment(string enrollmentId, List<string> itemId, out Adc.Enrollment enrollment)
        {
            var results = new List<Adc.Grade>();
            var cmd = new GetGrades
                          {

                              SearchParameters = new Adc.GradeSearch
                                                     {
                                                         EnrollmentId = enrollmentId,
                                                         ItemIds = itemId
                                                     }

                          };

            SessionManager.CurrentSession.Execute(cmd);
            enrollment = cmd.Enrollments.FirstOrDefault();
            foreach (var item in cmd.Enrollments)
            {
                results.AddRange(item.Grades);
            }
            return results;
        }

    }
}
