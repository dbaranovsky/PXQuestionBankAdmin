using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.ReminderService.DataContracts;
using Microsoft.Practices.ServiceLocation;
using Adc = Bfw.Agilix.DataContracts;
using System.Diagnostics;
using System.Xml.XPath;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using Bfw.PX.Biz.Services.Mappers;

namespace Bfw.PX.ReminderService
{
    public class DLAPService : IDLAPService
    {
        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        private static ISessionManager SessionManager { get; set; }

        public DLAPService()
        {
            ////Set the session
            var username = ConfigurationManager.AppSettings["user"];
            var password = ConfigurationManager.AppSettings["password"];
            SessionManager = ServiceLocator.Current.GetInstance<ISessionManager>();
            SessionManager.CurrentSession = SessionManager.StartNewSession(username, password, true, username);
        }

        /// <summary>
        /// Gets list of signals based on search parameters
        /// </summary>
        /// <param name="lastSignalId"></param>
        /// <param name="signalType"></param>
        /// <returns></returns>
        public List<Signal> GetSignalList(string lastSignalId, string signalType)
        {
            var result = new List<Adc.Signal>();

            var cmd = new GetSignalList()
            {
                SearchParameter = new Adc.SignalSearch()
                {
                    LastSignalId = lastSignalId,
                    SignalType = signalType
                }
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            if (!cmd.SignalList.IsNullOrEmpty())
            {
                result = cmd.SignalList;
            }

            return result;
        }

        /// <summary>
        /// Gets list of enrollments for the dropped signals
        /// </summary>
        /// <param name="signals"></param>
        /// <returns></returns>
        public List<EnrollmentSignal> GetDroppedEnrollmentList(List<Adc.Signal> signals)
        {
            var result = new List<EnrollmentSignal>();
            var batch = new Batch();
            var instructors = GetInstructorListFromSignals(signals);

            foreach (var signal in signals)
            {
                batch.Add(new GetEnrollment3() { EnrollmentId = signal.EntityId, Select = EnrollmentSelect.Course | EnrollmentSelect.CourseData | EnrollmentSelect.User });
            }

            SessionManager.CurrentSession.ExecuteAsAdmin(batch);

            var commands = batch.Commands.ToArray();
            for (var i = 0; i < commands.Length; i++)
            {
                var getEnrollment = commands[i] as GetEnrollment3;
                var enrollmentSignal = new EnrollmentSignal() { Signal = signals[i] };

                if (!getEnrollment.Enrollments.IsNullOrEmpty())
                {
                    enrollmentSignal.Enrollment = getEnrollment.Enrollments.FirstOrDefault(e => e.Priviliges.Equals(ConfigurationManager.AppSettings.Get("studentPriviliges")));
                }

                if (!instructors.IsNullOrEmpty())
                {
                    enrollmentSignal.Instructor = instructors.FirstOrDefault(u => u.Id.Equals(enrollmentSignal.Signal.CreationBy));
                }

                result.Add(enrollmentSignal);
            }

            return result;
        }

        /// <summary>
        /// Gets list of instructors based on signal's creation by
        /// </summary>
        /// <param name="signals"></param>
        /// <returns></returns>
        public List<AgilixUser> GetInstructorListFromSignals(List<Adc.Signal> signals)
        {
            var result = new List<AgilixUser>();
            var instructors = signals.Select(s => s.CreationBy).Distinct();
            var query = string.Empty;

            if (instructors != null)
            {
                foreach (var instructor in instructors.Where(i => !i.IsNullOrEmpty()))
                {
                    query += String.Format("/id={0} OR ", instructor);
                }

                if (query.Length > 0)
                {
                    query = query.Substring(0, query.Length - 3);
                    var listUsers = new ListUsers() { DomainId = "0", Query = query };

                    SessionManager.CurrentSession.ExecuteAsAdmin(listUsers);

                    if (!listUsers.Users.IsNullOrEmpty())
                    {
                        result = listUsers.Users;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the list of students based on course id
        /// </summary>
        /// <param name="entityId">course id to get the list of students</param>
        /// <returns>list of students</returns>
        public List<Sender> GetStudentList(string entityId)
        {
            List<Sender> entityEnrollmentList = null;
            Adc.EntitySearch searchParameters = new Adc.EntitySearch() { CourseId = entityId };
            var cmd = new GetEntityEnrollmentList() { SearchParameters = searchParameters };
            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            if (!cmd.Enrollments.IsNullOrEmpty())
            {
                entityEnrollmentList = cmd.Enrollments.Where(e => !e.User.Email.IsNullOrEmpty()).Map(e => new Sender
                {
                    FirstName = e.User.FirstName,
                    LastName = e.User.LastName,
                    Email = e.User.Email,
                    Id = int.Parse(e.User.Id),
                    EnrollmentId = int.Parse(e.Id),
                    Flags = e.Flags.ToString()
                }).ToList();
            }

            return entityEnrollmentList;
        }

        /// <summary>
        /// gets the assignment item object
        /// </summary>
        /// <param name="entityId">course id</param>
        /// <param name="itemId">item id</param>
        /// <returns></returns>
        public Adc.Item GetContent(string entityId, string itemId)
        {
            Adc.ItemSearch searchParameters = new Adc.ItemSearch() { EntityId = entityId, ItemId = itemId };
            var cmd = new GetItems() { SearchParameters = searchParameters };
            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            if (!cmd.Items.IsNullOrEmpty())
            {
                return cmd.Items.First();
            }

            return null;
        }

        /// <summary>
        /// gets the course object
        /// </summary>
        /// <param name="courseId"Course id></param>
        /// <returns></returns>
        public Adc.Course GetCourse(string courseId)
        {
            Adc.CourseSearch searchParameters = new Adc.CourseSearch() { CourseId = courseId };
            var cmd = new GetCourse() { SearchParameters = searchParameters };
            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            if (!cmd.Courses.IsNullOrEmpty())
            {
                return cmd.Courses.First();
            }

            return null;
        }

        /// <summary>
        /// Gets a list of items that are past the due date
        /// </summary>
        /// <param name="reminders"></param>
        /// <returns></returns>
        public List<Item> GetPastDueItems(List<ReminderEmail> reminders)
        {
            var batch = new Batch();
            foreach (var r in reminders)
            {
                batch.Add(new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch()
                    {
                        EntityId = r.CourseId,
                        ItemId = r.ItemId
                    }
                });
            }
            SessionManager.CurrentSession.ExecuteAsAdmin(batch);
            var cmd = batch.CommandAs<GetItems>(0);
            if (cmd != null && !cmd.Items.IsNullOrEmpty())
            {
                return cmd.Items.Where(i => i.DueDate < DateTime.Now).ToList();
            }
            return null;
        }
    }
}
