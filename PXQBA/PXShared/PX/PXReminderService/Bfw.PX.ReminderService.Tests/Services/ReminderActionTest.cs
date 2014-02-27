using System;
using System.Collections.Generic;
using System.Net.Mail;
using Bfw.Agilix.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;

namespace Bfw.PX.ReminderService.Tests.Services
{
    [TestClass]
    public class ReminderActionTest
    {
        ReminderAction action;
        IDBService dbService;
        IDLAPService dlapService;
        IMailerService mailerService;
        IRAService raService;

        [TestInitialize]
        public void TestInitialize()
        {
            dbService = Substitute.For<IDBService>();
            dbService.When(c => c.GetReminderConfiguration()).Do(r =>
            {

            });
            dlapService = Substitute.For<IDLAPService>();
            mailerService = Substitute.For<IMailerService>();
            raService = Substitute.For<IRAService>();

            action = ReminderAction.GetInstance();

            action.DB = dbService;
            action.DLAP = dlapService;
            action.Mailer = mailerService;
            action.RA = raService;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            action.GetType().GetField("instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(action, null);
        }

        [TestMethod]
        public void Run_Should_Process_Reminder_Emails()
        {
            var emails = new List<Bfw.PX.ReminderService.DataContracts.ReminderEmail>()
            {
                GetSuccessReminderEmail(),
                GetFailureReminderEmail()
            };
            dbService.GetReminderMails().Returns(emails);
            dlapService.GetSignalList("lastSignalId", "1.2").ReturnsForAnyArgs(new List<Signal>());
            mailerService.WhenForAnyArgs(o => o.SendMail(Arg.Any<MailMessage>(), Arg.Any<string>(), Arg.Any<string>())).Do(r =>
            {
                if (r.Arg<MailMessage>().To.First().Address == "failure@email.com")
                {
                    throw new Exception();
                }
            });

            action.Run();

            dbService.Received(1).GetReminderMails();            
            mailerService.Received(2).SendMail(Arg.Any<MailMessage>(), Arg.Any<string>(), Arg.Any<string>());
            dbService.Received(1).UpdateStatus(Arg.Any<int>(), Arg.Is<List<string>>(e => e.Count == 1), Arg.Is<List<string>>(e => e.Count == 0));
            dbService.Received(1).UpdateStatus(Arg.Any<int>(), Arg.Is<List<string>>(e => e.Count == 0), Arg.Is<List<string>>(e => e.Count == 1));
        }

        [TestMethod]
        public void Run_Should_Process_Signals()
        {
            var signals = new List<Signal>()
            {
                new Signal()
                {
                    CreationBy ="instructorId",
                    CreationDate = DateTime.Now,
                    DomainId = "domainId",
                    EntityId = "entityId",
                    SignalId = "signalId",
                    Type = "1.2",
                    OldStatus = EnrollmentStatus.Active,
                    NewStatus = EnrollmentStatus.Withdrawn 
                }
            };
            dbService.GetReminderMails().Returns(new List<DataContracts.ReminderEmail>());
            dbService.GetEmailTemplate(0).ReturnsForAnyArgs(new Helpers.Structs.EmailTemplate() { id = 0, TemplateText = string.Empty, TemplateHtml = string.Empty });
            dlapService.GetSignalList("lastSignalId", "1.2").ReturnsForAnyArgs(signals);
            dlapService.GetDroppedEnrollmentList(new List<Signal>()).ReturnsForAnyArgs(new List<DataContracts.EnrollmentSignal>() 
            {
                GetSuccessEnrollmentSignal(),
                GetFailureEnrollmentSignal()
            });
            mailerService.WhenForAnyArgs(o => o.SendMail(Arg.Any<MailMessage>(), Arg.Any<string>(), Arg.Any<string>())).Do(r =>
            {
                if (r.Arg<MailMessage>().To.First().Address == "failure@email.com")
                {
                    throw new Exception();
                }
            });

            action.Run();

            dlapService.Received(1).GetSignalList(Arg.Any<string>(), Arg.Any<string>());
            dlapService.Received(1).GetDroppedEnrollmentList(Arg.Any<List<Signal>>());
            dbService.Received(1).AddEmailTracking(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<DateTime>(), "sent", Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>());
            dbService.Received(1).AddEmailTracking(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<DateTime>(), "add", Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>());
        }

        [TestMethod]
        public void Run_Should_Track_LastSignalId()
        {
            var signals = new List<Signal>()
            {
                new Signal()
                {
                    CreationBy ="instructorId",
                    CreationDate = DateTime.Now,
                    DomainId = "domainId",
                    EntityId = "entityId",
                    SignalId = "signalId",
                    Type = "1.2",
                    OldStatus = EnrollmentStatus.Active,
                    NewStatus = EnrollmentStatus.Inactive 
                }
            };
            dbService.GetReminderMails().Returns(new List<DataContracts.ReminderEmail>());
            dbService.GetEmailTemplate(0).ReturnsForAnyArgs(new Helpers.Structs.EmailTemplate() { id = 0, TemplateText = string.Empty, TemplateHtml = string.Empty });
            dlapService.GetSignalList("lastSignalId", "1.2").ReturnsForAnyArgs(signals);

            action.Run();

            dbService.Received(1).AddEmailTracking(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<int>(), "signalId", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>());
        }

        [TestMethod]
        public void Run_Should_Filter_PastDueAssignment_Reminders()
        {
            var pastDueItemId = "item2";
            var pastDueFromAddress = "instructorWithPastDue@mail.com";
            var emails = new List<Bfw.PX.ReminderService.DataContracts.ReminderEmail>()
            {
                new Bfw.PX.ReminderService.DataContracts.ReminderEmail{ ItemId = "item1", NotificationType = 1, Senders = new List<DataContracts.Sender>{ new DataContracts.Sender{ Email = "instructor@mail.com" } }, TemplateBodyHTML = "", ToList = "success@email.com", Subject = "", TemplateBodyText = "" },
                new Bfw.PX.ReminderService.DataContracts.ReminderEmail{ ItemId = pastDueItemId, NotificationType = 1, Senders = new List<DataContracts.Sender>{ new DataContracts.Sender{ Email = pastDueFromAddress } }, TemplateBodyHTML = "", ToList = "success@email.com", Subject = "", TemplateBodyText = "" },
                new Bfw.PX.ReminderService.DataContracts.ReminderEmail{ ItemId = "item3", NotificationType = 3, Senders = new List<DataContracts.Sender>{ new DataContracts.Sender{ Email = "instructor@mail.com" } }, TemplateBodyHTML = "", ToList = "success@email.com", Subject = "", TemplateBodyText = "" },
            };
            dbService.GetReminderMails().Returns(emails);
            dlapService.GetSignalList("lastSignalId", "1.2").ReturnsForAnyArgs(new List<Signal>());
            dlapService.GetPastDueItems(Arg.Is<List<Bfw.PX.ReminderService.DataContracts.ReminderEmail>>(l => l.Count == 2)).Returns(new List<Item> { new Item { Id = pastDueItemId, DueDate = DateTime.Now.AddHours(-1) } });
            action.Run();
            mailerService.Received(2).SendMail(Arg.Is<MailMessage>(m => m.From.Address != pastDueFromAddress), Arg.Any<string>(), Arg.Any<string>());
        }

        private Bfw.PX.ReminderService.DataContracts.ReminderEmail GetSuccessReminderEmail()
        {
            return new Bfw.PX.ReminderService.DataContracts.ReminderEmail()
                {
                    EmailId = 1,
                    Senders = new List<DataContracts.Sender>()
                    {
                        new DataContracts.Sender()
                        {
                            Email = "instructor@email.com"
                        }
                    },
                    Subject = string.Empty,
                    TemplateBodyHTML = string.Empty,
                    TemplateBodyText = string.Empty,
                    ToList = "success@email.com"
                };
        }

        private Bfw.PX.ReminderService.DataContracts.ReminderEmail GetFailureReminderEmail()
        {
            return new Bfw.PX.ReminderService.DataContracts.ReminderEmail()
            {
                EmailId = 2,
                Senders = new List<DataContracts.Sender>()
                    {
                        new DataContracts.Sender()
                        {
                            Email = "instructor@email.com"
                        }
                    },
                Subject = string.Empty,
                TemplateBodyHTML = string.Empty,
                TemplateBodyText = string.Empty,
                ToList = "failure@email.com"
            };
        }

        private DataContracts.EnrollmentSignal GetSuccessEnrollmentSignal()
        {
            return new DataContracts.EnrollmentSignal()
            {
                Signal = new Signal()
                {

                },
                Instructor = new AgilixUser()
                {
                    Id = "1",
                    Email = "instructor@email.com"
                },
                Enrollment = new Enrollment()
                {
                    Course = new Course(),
                    User = new AgilixUser()
                    {
                        Email = "success@email.com"
                    }
                }
            };
        }

        private DataContracts.EnrollmentSignal GetFailureEnrollmentSignal()
        {
            return new DataContracts.EnrollmentSignal()
            {
                Signal = new Signal()
                {

                },
                Instructor = new AgilixUser()
                {
                    Id = "2",
                    Email = "instructor@email.com"
                },
                Enrollment = new Enrollment()
                {
                    Course = new Course(),
                    User = new AgilixUser()
                    {
                        Email = "failure@email.com"
                    }
                }
            };
        }
    }
}
