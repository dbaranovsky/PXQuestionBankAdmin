using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using System.Xml;
using System.Configuration;
using System.IO;

using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using Models = Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Components;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;

using OfficeOpenXml;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    public static class GradebookExtensions
    {
        public static Grade ToGrade(this BizDC.Grade contractGrade)
        {
            var grade = new Grade()
            {
                EnrollmentId = contractGrade.EnrollmentId,
                EnrollmentName = contractGrade.EnrollmentName,
                ItemId = contractGrade.ItemId,
                Possible = contractGrade.Possible,
                Achieved = contractGrade.Achieved,
                SubmittedDate = contractGrade.SubmittedDate,
                ScoredDate = contractGrade.ScoredDate,
                ScoredVersion = contractGrade.ScoredVersion,
                ItemTitle = contractGrade.ItemName,
                GradeRule = (GradeRule)contractGrade.Rule,
                AttemptLimit = contractGrade.GradedItem != null ? contractGrade.GradedItem.AssessmentSettings.AttemptLimit : 0,
                AttemptList = contractGrade.Submissions != null ? contractGrade.Submissions.Map(s => s.ToAttempt()).ToList() : null
            };
            return grade;
        }

        public static Attempt ToAttempt(this BizDC.SubmissionLog submission)
        {
            var attempt = new Attempt()
            {
                Count = submission.AttemptNo,
                RawPossible = submission.RawPossible,
                RawAchieved = submission.RawAchieved,
                Submitted = submission.SubmittedDate.Value,
                Achieved = submission.Achieved,
                Possible = submission.Possible
            };
            return attempt;
        }
    }

    public class GradebookWidgetController : Controller
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        protected BizSC.IPxGradeBookActions GradeBookActions { get; set; }

        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// widget's pubic constructor
        /// </summary>
        /// <param name="context"></param>
        public GradebookWidgetController(BizSC.IBusinessContext context, BizSC.IEnrollmentActions enrollmentActions, BizSC.IPxGradeBookActions gradeBookActions, BizSC.IContentActions contentActions)
        {
            Context = context;
            EnrollmentActions = enrollmentActions;
            GradeBookActions = gradeBookActions;
            ContentActions = contentActions;
        }

        public ActionResult Gradebook() 
        {
            ViewData["mode"] = Context.ImpersonateStudent ? BizSC.AccessLevel.Student.ToString() : Context.AccessLevel.ToString();
            return View();
        }
        public ActionResult AssignedScores()
        {
            var model = new Models.GradeBook();

            model.Students = GradeBookActions.GetStudentList().Map(sp => sp.ToStudentProfile()).ToList();

            model.Assignments = GradeBookActions.GetGradeBookAssignments();

            if(model.Assignments.Count > 0)
                model.Grades = GetGrades(model.Students, model.Assignments);

            model.ClassAverage = GetAverage(model.Grades, true);
            
            return View(model);
        }

        public ActionResult GetItemGrades()
        {
            var model = new GradeData();
            model.ItemGrades = GradeBookActions.ItemAggregatedGrades(false).Map(g => g.ToGrade()).ToList();
            model.StudentGrades = GradeBookActions.StudentAggregatedGrades(false).Map(g => g.ToGrade()).ToList();

            if (model.ItemGrades.Count > 0)
            {
                model.ClassAverage = GetAverage(model.ItemGrades, false);
            }

            //show student export button if atleast one student is enrolled and atleast one assigned has been scored(graded)
            var students = GradeBookActions.GetStudentList().Map(sp => sp.ToStudentProfile()).ToList();
            var studentGrades = new List<Grade>();
            students.ForEach(student => { studentGrades.AddRange(GradeBookActions.ItemGradeDetails(student.EnrollmentId, false).Map(g => g.ToGrade()).ToList()); });
            var scoredGrades = studentGrades.Where(sg => sg.ScoredVersion > 0);

            ViewData["showAllAdditionalExport"] = students.Count() > 0 && scoredGrades.Count() > 0;
            
            return View("~/Views/GradebookWidget/OtherScores.ascx", model);
        }

        public ActionResult ItemScores(string itemId, string itemType, string userName = "")
        {
            var model = new ItemGradeData();

            if (itemType == "Item")
            {
                var item = ContentActions.GetItems(Context.EntityId, new List<string>() { itemId }).Map(c => c.ToContentItem(ContentActions)).FirstOrDefault();
                model.ItemName = item.Title;
                model.Grades = GradeBookActions.StudentGradeDetails(itemId, false).Map(g => g.ToGrade()).ToList();
            }
            else
            {
                model.ItemName = userName;
                model.Grades = GradeBookActions.ItemGradeDetails(itemId, false).Map(g => g.ToGrade()).ToList();    
            }

            if (model.Grades.Count() > 0)
            { 
                model.GradeRule = model.Grades.FirstOrDefault().GetGradeRule();
                var gradedSubmissions = model.Grades.Where(g => g.IsGraded);
                if (gradedSubmissions.Count() > 0)
                    model.Average = Math.Round(gradedSubmissions.Average(g => g.GradeScoreNumeric), 3).ToString("#0.0%");
                else
                    model.Average = "N/A";
            }
            model.ItemType = itemType;
            
            return View(model);
        }

        public ActionResult StudentDetail(string studentUserId, string studentEnrollmentId)
        {
            var model = new StudentGradebook();
            model.Student = GradeBookActions.GetStudent(studentUserId).ToStudentProfile();
            model.Assignments = GradeBookActions.GetGradeBookAssignments();
            model.Grades = GetStudentGrades(studentEnrollmentId, model.Assignments);

            model.AssignedAverage = GetAverage(model.Grades, true);

            model.UnAssingedGrades = GradeBookActions.ItemGradeDetails(studentEnrollmentId, false).Map(g => g.ToGrade()).ToList();

            model.UnAssignedAverage = GetAverage(model.UnAssingedGrades, false);

            //get the last updated date for the student gradebook
            var lastUpdatedDate = GetLastUpdatedDate(model);
            ViewData["LastUpdate"] = lastUpdatedDate;

            ViewData.Model = model;
            return View();
        }

        public ActionResult AssignmentSummary(string assignmentId)
        {
            //Just reusing the gradebook even though has way more properties then necessary
            var model = new StudentGradebook();
            var assignment = ContentActions.GetContent(Context.EntityId, assignmentId);
            model.Grades = GradeBookActions.StudentGradeDetails(assignmentId, true).Map(g => g.ToGrade());
            var average = GetAverage(model.Grades, true);
            ViewData["ItemName"] = assignment.Title;
            ViewData["Average"] = average;

            return View(model);
        }

        /// <summary>
        /// Returns an excel sheet containing the assigned report
        /// </summary>
        /// <returns></returns>
        public FileContentResult ExportAssignedReport(string studentUserId, string studentEnrollmentId)
        {
            var templateFile = new FileInfo(Server.MapPath(ConfigurationManager.AppSettings["AssignedReportTemplate"]));
            var xlPack = new ExcelPackage(null, templateFile);
            var ws = xlPack.Workbook.Worksheets[1];
            var replacementCells = GetReplacementCells(ws);
            
            //get the student info
            var studentInfo = GradeBookActions.GetStudent(studentUserId).ToStudentProfile();
            
            //get the assigned item grades
            var assignments = GradeBookActions.GetGradeBookAssignments();
            var assignedItemGrades = GetStudentGrades(studentEnrollmentId, assignments);
            var assignedFolderGrades = GetAssignmentFolderGrades(assignments, assignedItemGrades);
            var totalPossible = assignedItemGrades.Sum(grade => grade.Possible);
            var totalAchieved = assignedItemGrades.Sum(grade => grade.Achieved);
            var assingedAverage = totalPossible > 0 ? totalAchieved / totalPossible : 0;

            // replacement values for symbols in the Excel Work Sheet
            var replacementValues = new Dictionary<String, String>();
            replacementValues.Add("_formattedname_", studentInfo.FormattedName.Trim());
            replacementValues.Add("_coursetitle_", Context.Course.Title.Trim());
            replacementValues.Add("_currentdate_", DateTime.Now.ToString("MM/dd/yyyy - hh:mm tt"));
            replacementValues.Add("_emailaddress_", studentInfo.Email.Trim());
            replacementValues.Add("_average_", string.Format("{0:0.00%}", assingedAverage));
            replacementValues.Add("_numberquizzes_", assignedItemGrades.Count().ToString());

            // replacing the symbols in the work sheet
            ReplaceSymbolsInCells(ws, replacementCells, replacementValues);

            // Load Header information into the report
            var assignmentFolderDataDump = ws.Workbook.Names["AssignmentFolderDataDump"];
            var assignmentDataDump = ws.Workbook.Names["AssignmentDataDump"];
            var headerDataDump = ws.Workbook.Names["DataDump"];
            var startingReference = headerDataDump.Start;
            var titleTexts = new List<Object>();
            var headerText = new List<Object>();
            var studentGrades = new List<Object>();
            int index = 0;
            foreach(var folderAssignment in assignments.Keys)
            {
                titleTexts.Add(folderAssignment.Title + " >");
                headerText.Add("Folder Average");
                var folderGrade = assignedFolderGrades.Where(af => af.ItemId == folderAssignment.Id).FirstOrDefault();
                studentGrades.Add(folderGrade.GradeScore);

                ws.Column(startingReference.Column + index).Width = ws.Column(assignmentFolderDataDump.Start.Column).Width;
                assignmentFolderDataDump.Copy(ws.Cells[startingReference.Row, startingReference.Column + index++]);
                
                foreach(var assignment in assignments[folderAssignment])
                {
                    titleTexts.Add(assignment.Title);
                    var assignmentGrade = assignedItemGrades.Where(ai => ai.ItemId == assignment.Id).FirstOrDefault();
                    headerText.Add(assignmentGrade != null ? assignmentGrade.Possible : 0);
                    studentGrades.Add(assignmentGrade!=null?assignmentGrade.GradeScore:"");

                    ws.Column(startingReference.Column + index).Width = ws.Column(assignmentDataDump.Start.Column).Width;
                    assignmentDataDump.Copy(ws.Cells[startingReference.Row, startingReference.Column + index++]);
                }
            }

            headerDataDump.LoadFromArrays(new List<Object[]> { titleTexts.ToArray(), headerText.ToArray(), studentGrades.ToArray() });

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var downloadFile = ConfigurationManager.AppSettings["AssignedReport"];
            return File(xlPack.GetAsByteArray(), contentType, downloadFile);
        }
        
        /// <summary>
        /// Returns an excel sheet containing additional item report (other scores)
        /// </summary>
        /// <returns></returns>
        public FileContentResult ExportAdditionalItemReport(string studentUserId, string studentEnrollmentId)
        {
            var templateFile = new FileInfo(Server.MapPath(ConfigurationManager.AppSettings["UnAssignedReportTemplate"]));
            var xlPack = new ExcelPackage(null, templateFile);
            var ws = xlPack.Workbook.Worksheets[1];
            var replacementCells = GetReplacementCells(ws);

            //get the student info
            var studentInfo = GradeBookActions.GetStudent(studentUserId).ToStudentProfile();

            //get the assigned item grades
            var assignments = GradeBookActions.GetGradeBookAssignments();
            var UnAssignedGrades = GradeBookActions.ItemGradeDetails(studentEnrollmentId, false).Map(g => g.ToGrade()).ToList();

            var totalPossible = UnAssignedGrades.Sum(grade => grade.Possible);
            var totalAchieved = UnAssignedGrades.Sum(grade => grade.Achieved);
            var unassignedAvg = totalPossible > 0 ? totalAchieved / totalPossible : 0;
            var totalAttempts = UnAssignedGrades.Sum(g => g.Attempts);

            var replacementValues = new Dictionary<String, String>();
            replacementValues.Add("_formattedname_", studentInfo.FormattedName.Trim());
            replacementValues.Add("_coursetitle_", Context.Course.Title.Trim());
            replacementValues.Add("_currentdate_", DateTime.Now.ToString("MM/dd/yyyy - hh:mm tt"));
            replacementValues.Add("_emailaddress_", studentInfo.Email.Trim());
            replacementValues.Add("_average_", string.Format("{0:0.00%}", unassignedAvg));
            replacementValues.Add("_numberquizzes_",  UnAssignedGrades.Count().ToString());
            replacementValues.Add("_numberattempts_", totalAttempts.ToString());

            ReplaceSymbolsInCells(ws, replacementCells, replacementValues);
            
            //Load data into the report
            var rowDataDump = ws.Workbook.Names["RowDataDump"];
            var dataDump = ws.Workbook.Names["DataDump"];
            var startingReference = dataDump.Start;
            var rowIndex = startingReference.Row;
            var rowGradeList = new List<Object[]>();
            foreach (var grade in UnAssignedGrades)
            {
                rowDataDump.Copy(ws.Cells[rowIndex++, startingReference.Column]);
                var rowData = new Object[] { grade.ItemTitle, grade.Achieved, grade.Possible, grade.GradeScore, grade.SubmittedDate.Value.ToString("MM/dd/yy hh:mm tt") }.ToArray();
                rowGradeList.Add(rowData);
            }

            //load the student grade data into the excel sheet
            dataDump.LoadFromArrays(rowGradeList);

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var downloadFile = ConfigurationManager.AppSettings["UnAssignedReport"];
            return File(xlPack.GetAsByteArray(), contentType, downloadFile);
        }

        /// <summary>
        /// Returns the excel sheet containing the grade details of all the students in a class
        /// </summary>
        /// <returns></returns>
        public FileContentResult ExportAllAssignmentsReport()
        {
            var templateFile = new FileInfo(Server.MapPath(ConfigurationManager.AppSettings["AllAssignmentReportTemplate"]));
            var xlPack = new ExcelPackage(null, templateFile);
            var ws = xlPack.Workbook.Worksheets[1];
            var replacementCells = GetReplacementCells(ws);

            var students = GradeBookActions.GetStudentList().Map(sp => sp.ToStudentProfile()).ToList();
            var assignments = GradeBookActions.GetGradeBookAssignments();
            var grades = GetGrades(students, assignments);
            var classAvg = string.Format("{0:0.00%}", GetAverage(grades, true));
            var assignedItemCount = GetAssignedItems(assignments).Count;

            // replacement values for symbols in the Excel Work Sheet
            var replacementValues = new Dictionary<String, String>();
            replacementValues.Add("_coursetitle_", Context.Course.Title.Trim());
            replacementValues.Add("_currentdate_", DateTime.Now.ToString("MM/dd/yyyy - hh:mm tt"));
            replacementValues.Add("_classaverage_", classAvg.ToString());
            replacementValues.Add("_countofstudents_", students.Count().ToString());
            replacementValues.Add("_totalassigneditems_", assignedItemCount.ToString());
            
            // replacing the symbols in the work sheet
            ReplaceSymbolsInCells(ws, replacementCells, replacementValues);

            // Load Header information into the report
            var assignmentFolderDataDump = ws.Workbook.Names["AssignmentFolderDataDump"];
            var assignmentDataDump = ws.Workbook.Names["AssignmentDataDump"];
            var headerDataDump = ws.Workbook.Names["DataDump"];
            var startingReference = headerDataDump.Start;
            var titleTexts = new List<Object>();
            var headerText = new List<Object>();
            int index = 0;

            foreach (var folderAssignment in assignments.Keys)
            {
                titleTexts.Add(folderAssignment.Title + " >");
                headerText.Add("Folder Average");

                ws.Column(startingReference.Column + index).Width = ws.Column(assignmentFolderDataDump.Start.Column).Width;
                assignmentFolderDataDump.Copy(ws.Cells[startingReference.Row, startingReference.Column + index++]);

                foreach (var assignment in assignments[folderAssignment])
                {
                    titleTexts.Add(assignment.Title);
                    var assignmentGrade = grades.Where(g => g.ItemId == assignment.Id).FirstOrDefault();
                    if(assignmentGrade != null)
                        headerText.Add(string.Format("{0} Points Possible", assignmentGrade.Possible));
                    ws.Column(startingReference.Column + index).Width = ws.Column(assignmentDataDump.Start.Column).Width;
                    assignmentDataDump.Copy(ws.Cells[startingReference.Row, startingReference.Column + index++]);
                }
            }
            headerDataDump.LoadFromArrays(new List<Object[]> { titleTexts.ToArray(), headerText.ToArray() });

            //style the student grade rows
            var studentDataDump = ws.Workbook.Names["studentDataDump"];
            var lastCol = assignments.Count +  assignedItemCount + 3; //column number of the last assignment in the excel sheet = number of folders + number of assignments + first three columns
            var studentDataRow = studentDataDump.Start.Row;
            var studentTemplateCells = ws.Cells[studentDataDump.Start.Row, studentDataDump.Start.Column, studentDataDump.Start.Row, lastCol];
            students.ForEach(s => { studentTemplateCells.Copy(ws.Cells[studentDataRow++, 1]); });

            //Load the student gradebook info into the report
            var studentGradeRowList = new List<Object[]>();
            foreach (var student in students)
            {
                var studentGradeRow = new List<Object>();
                var studentGrade = grades.Where(g => (g.EnrollmentId == student.EnrollmentId && g.ItemId == null)).FirstOrDefault();

                studentGradeRow.Add(student.FormattedName);
                studentGradeRow.Add(student.Email);
                studentGradeRow.Add(studentGrade.GradeScore);

                foreach (var assignmentFolder in assignments.Keys)
                {
                    var assignmentItems = assignments[assignmentFolder];
                    var folderGrade = grades.Where(g => (g.EnrollmentId == student.EnrollmentId && g.ItemId == assignmentFolder.Id)).FirstOrDefault();
                    studentGradeRow.Add(folderGrade.GradeScore);
                    foreach (var assignmentItem in assignmentItems)
                    {
                        var assignmentGrade = grades.Where(g => (g.EnrollmentId == student.EnrollmentId && g.ItemId == assignmentItem.Id)).FirstOrDefault();
                        if(assignmentGrade!=null) studentGradeRow.Add(assignmentGrade.Achieved);
                    }
                }
                studentGradeRowList.Add(studentGradeRow.ToArray());
            }
            if(studentGradeRowList.Count > 0)
                studentDataDump.LoadFromArrays(studentGradeRowList);

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var downloadFile = ConfigurationManager.AppSettings["AllAssignmentReport"];
            return File(xlPack.GetAsByteArray(), contentType, downloadFile);
        }

        /// <summary>
        /// Returns the excel sheet containing the grade details of all unassigned items
        /// </summary>
        /// <returns></returns>
        public FileContentResult ExportAllAdditionalItemReport()
        {
            var templateFile = new FileInfo(Server.MapPath(ConfigurationManager.AppSettings["AllAdditionalReportTemplate"]));
            var xlPack = new ExcelPackage(null, templateFile);
            var ws = xlPack.Workbook.Worksheets[1];
            var replacementCells = GetReplacementCells(ws);

            //list of enrolled students
            var students = GradeBookActions.GetStudentList().Map(sp => sp.ToStudentProfile()).ToList();
            
            //student aggregate grades
            var studentAggGrades = GradeBookActions.StudentAggregatedGrades(false).Map(g => g.ToGrade()).ToList();

            //student grades
            var studentGrades = new List<Grade>();
            students.ForEach(student => { studentGrades.AddRange(GradeBookActions.ItemGradeDetails(student.EnrollmentId, false).Map(g => g.ToGrade()).ToList()); });
            var itemAggGrades = GradeBookActions.ItemAggregatedGrades(false).Map(g => g.ToGrade()).ToList();

            var totalAttempts = 0;
            studentGrades.ForEach(sg => { totalAttempts += sg.Attempts; });

            // replacement values for symbols in the Excel Work Sheet
            var replacementValues = new Dictionary<String, String>();
            replacementValues.Add("_coursetitle_", Context.Course.Title.Trim());
            replacementValues.Add("_currentdate_", DateTime.Now.ToString("MM/dd/yyyy - hh:mm tt"));
            //replacementValues.Add("_classaverage_", classAvg.ToString());
            replacementValues.Add("_countofstudents_", students.Count().ToString());
            replacementValues.Add("_numberofquizzes_", itemAggGrades.Count().ToString());
            replacementValues.Add("_totalattempts_", totalAttempts.ToString());

            // replacing the symbols in the work sheet
            ReplaceSymbolsInCells(ws, replacementCells, replacementValues);

            //style the student rows
            var studentAggDataDump = ws.Workbook.Names["StudentAggTemplateRow"];
            var studentDetailDataDump = ws.Workbook.Names["StudentDetailTemplateRow"];
            var startRowDataDump = ws.Workbook.Names["StartRowDataDump"];
            var consideredScoreDump = ws.Workbook.Names["consideredscoreDump"];
            var consideredScore = ws.Workbook.Names["consideredscore"];

            var startRow = startRowDataDump.Start.Row;
            var startCol = startRowDataDump.Start.Column;
            var index = 0;

            var aggTemplateRow = ws.Cells[studentAggDataDump.Start.Row, studentAggDataDump.Start.Column, studentAggDataDump.End.Row, studentAggDataDump.End.Column];
            var detailTemplateRow = ws.Cells[studentDetailDataDump.Start.Row, studentDetailDataDump.Start.Column, studentDetailDataDump.End.Row, studentDetailDataDump.End.Column];

            foreach (var student in students)
            {
                aggTemplateRow.Copy(ws.Cells[startRow + index++, startCol]);
                foreach (var grade in studentGrades.Where(sg => sg.EnrollmentId == student.EnrollmentId))
                {
                    foreach (var attempt in grade.AttemptList)
                    {
                        detailTemplateRow.Copy(ws.Cells[startRow + index++, startCol]);
                    }
                }
            }

            //load the student gradebook info into the report
            var studentGradeRowList = new List<Object[]>();
            var consideredScoreRow = consideredScore.Start.Row;
            var consideredScoreCol = consideredScore.Start.Column;
            foreach (var student in students)
            {
                var studentAggGrade = studentAggGrades.Where(sg => sg.EnrollmentId == student.EnrollmentId).FirstOrDefault();
                var studentAggGradeRow = new List<Object>() { student.FormattedName, student.Email, studentAggGrade.GradeScore, string.Empty, string.Format("TOTAL: {0}", studentAggGrade.Achieved), string.Format("TOTAL: {0}", studentAggGrade.Possible), string.Empty, string.Empty, string.Empty, string.Empty };
                studentGradeRowList.Add(studentAggGradeRow.ToArray());
                consideredScoreRow++;
                foreach (var grade in studentGrades.Where(sg => sg.EnrollmentId == student.EnrollmentId))
                {
                    var attemptCount = 1;
                    var highestAttemptScore = grade.AttemptList.Max(at => at.Achieved);
                    var lowestAttemptScore = grade.AttemptList.Min(at => at.Achieved);

                    foreach (var attempt in grade.AttemptList)
                    {
                        // considered student score should be color coded based on the grade rule
                        switch (grade.GradeRule)
                        {
                            case GradeRule.Last:
                                if (attemptCount == grade.Attempts)
                                    consideredScoreDump.Copy(ws.Cells[consideredScoreRow, consideredScoreCol]);
                                break;
                            case GradeRule.First:
                                if (attemptCount == 1)
                                    consideredScoreDump.Copy(ws.Cells[consideredScoreRow, consideredScoreCol]);
                                break;
                            case GradeRule.Highest:
                                if(attempt.Achieved == highestAttemptScore)
                                    consideredScoreDump.Copy(ws.Cells[consideredScoreRow, consideredScoreCol]);
                                break;
                            case GradeRule.Lowest:
                                if(attempt.Achieved == lowestAttemptScore)
                                    consideredScoreDump.Copy(ws.Cells[consideredScoreRow, consideredScoreCol]);
                                break;
                            case GradeRule.Average:
                                consideredScoreDump.Copy(ws.Cells[consideredScoreRow, consideredScoreCol]);
                                break;
                            case GradeRule.Total:
                                consideredScoreDump.Copy(ws.Cells[consideredScoreRow, consideredScoreCol]);
                                break;
                            default:
                                break;
                        }

                        var studentAttemptRow = new List<Object>() { student.FormattedName, string.Empty, string.Empty, grade.ItemTitle, attempt.Achieved, attempt.Possible, attempt.ActualScore, attempt.Submitted.ToString("MM/dd/yyyy hh:mm tt"), string.Empty, string.Format("attempt {0} ", attemptCount++) };
                        studentGradeRowList.Add(studentAttemptRow.ToArray());
                        consideredScoreRow++;
                    }
                }
            }

            startRowDataDump.LoadFromArrays(studentGradeRowList);

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var downloadFile = ConfigurationManager.AppSettings["AllAdditionalReport"];
            return File(xlPack.GetAsByteArray(), contentType, downloadFile);
        }

        /// <summary>
        /// Gets a view for the student version of the gradebook
        /// </summary>
        /// <returns>Student gradebook view</returns>
        public ActionResult StudentGradeBook()
        {
            var studentUserId = Context.CurrentUser.Id;
            var studentEnrollmentId = Context.EnrollmentId;

            var model = new StudentGradebook();
            model.Student = GradeBookActions.GetStudent(studentUserId).ToStudentProfile();
            model.Assignments = GradeBookActions.GetGradeBookAssignments();
            model.Grades = GetStudentGrades(studentEnrollmentId, model.Assignments);

            model.AssignedAverage = GetAverage(model.Grades, true);

            model.UnAssingedGrades = GradeBookActions.ItemGradeDetails(studentEnrollmentId, false).Map(g => g.ToGrade()).ToList();

            model.UnAssignedAverage = GetAverage(model.UnAssingedGrades, false);

            ViewData["DisplayAverage"] = true;
            ViewData["IsStudent"] = true;
            ViewData["DisplaySubmittedData"] = true;
            
            //get the last updated date for the student gradebook
            var lastUpdatedDate = GetLastUpdatedDate(model);
            ViewData["LastUpdate"] = lastUpdatedDate;
            ViewData.Model = model;

            return View("StudentGradebook");
        }

        /// <summary>
        /// Gets the grade points for all assignments (including assignment folder) for all students
        /// </summary>
        /// <param name="students"></param>
        /// <param name="assignments"></param>
        /// <returns></returns>
        private IList<Grade> GetGrades(IList<StudentProfile> students, IDictionary<BizDC.ContentItem, IList<BizDC.ContentItem>> assignments)
        {
            var gradeEnrollments = GradeBookActions.GetEnrollments();
            var gradeCollection = gradeEnrollments.SelectMany(delegate(BizDC.Enrollment enrollment) {
                IEnumerable<Grade> grades = new List<Grade>();
                if (!enrollment.ItemGrades.IsNullOrEmpty())
                {
                    grades = enrollment.ItemGrades.Map(delegate(BizDC.Grade bizGrade)
                    {
                        var grade = new Grade()
                        {
                            EnrollmentId = enrollment.Id,
                            ItemId = bizGrade.ItemId,
                            Possible = bizGrade.Possible,
                            Achieved = bizGrade.Achieved,
                            SubmittedDate = bizGrade.SubmittedDate,
                            ScoredDate = bizGrade.ScoredDate,
                            ScoredVersion = bizGrade.ScoredVersion,
                            ItemTitle = bizGrade.ItemName,
                            GradeRule = (GradeRule)bizGrade.Rule,
                        };
                        return grade;
                    });
                }
            return grades;
            }).ToList();

            //aggregate grade for assignment folders for each student assignment folder combination
            students.ToList().ForEach(delegate(StudentProfile student)
            {
                //aggegate grade for each student
                var studentAggGrade = new Grade();
                double aggPossibleScore = 0, aggAchievedScore = 0;
                studentAggGrade.EnrollmentId = student.EnrollmentId;

                foreach (var assignmentFolder in assignments)
                {
                    var assignmentFolderGrade = new Grade();
                    var assignmentFolderItem = assignmentFolder.Key;
                    var assignmentItems = assignmentFolder.Value;
                    double possibleScore = 0, achievedScore = 0;

                    assignmentFolderGrade.ItemId = assignmentFolderItem.Id;
                    assignmentFolderGrade.EnrollmentId = student.EnrollmentId;
                    assignmentFolderGrade.IsAssignmentFolder = true;

                    assignmentItems.ToList().ForEach(delegate(BizDC.ContentItem assignmentItem) {
                        var assignmentItemGrade = gradeCollection.Where(g => g.EnrollmentId == student.EnrollmentId && g.ItemId == assignmentItem.Id).FirstOrDefault();
                        if (assignmentItemGrade != null)
                        {
                            possibleScore += assignmentItemGrade.Possible;
                            achievedScore += assignmentItemGrade.Achieved;
                        }
                    });

                    assignmentFolderGrade.Possible = possibleScore;
                    assignmentFolderGrade.Achieved = achievedScore;
                    aggPossibleScore += possibleScore;
                    aggAchievedScore += achievedScore;

                    gradeCollection.Add(assignmentFolderGrade);
                }

                studentAggGrade.Possible = aggPossibleScore;
                studentAggGrade.Achieved = aggAchievedScore;
                gradeCollection.Add(studentAggGrade);
            });

            return gradeCollection;
        }
        /// <summary>
        /// Get the grades points for all the assignments of a given student
        /// </summary>
        /// <returns></returns>
        private IList<Grade> GetStudentGrades(string studentEnrollmentId, IDictionary<BizDC.ContentItem, IList<BizDC.ContentItem>> assignments)
        {
            var bizGrades = GradeBookActions.GetGradesByEnrollment(studentEnrollmentId, true);
            var grades = bizGrades.Map(delegate(BizDC.Grade bizGrade)
            {
                var grade = new Grade()
                {
                    EnrollmentId = studentEnrollmentId,
                    ItemId = bizGrade.ItemId,
                    Possible = bizGrade.Possible,
                    Achieved = bizGrade.Achieved,
                    SubmittedDate = bizGrade.SubmittedDate,
                    ScoredDate = bizGrade.ScoredDate,
                    ScoredVersion = bizGrade.ScoredVersion,
                    ItemTitle = bizGrade.ItemName,
                    GradeRule = (GradeRule)bizGrade.Rule,
                    AttemptLimit = bizGrade.GradedItem.AssessmentSettings.AttemptLimit,
                    AttemptList = bizGrades.Count() > 0 ? GradeBookActions.GetAttemptsByStudent(bizGrade.ItemId, studentEnrollmentId).Map(sl => new Attempt() { Count = sl.AttemptNo, RawPossible = sl.RawPossible, RawAchieved = sl.RawAchieved, Submitted = sl.SubmittedDate.Value }).ToList():null
                };
                return grade;
            }).ToList();

            foreach (var assignmentFolder in assignments)
            {
                var assignmentFolderItem = assignmentFolder.Key;
                var assignmentItems = assignmentFolder.Value;
                assignmentItems.ToList().ForEach(delegate(BizDC.ContentItem assignmentItem)
                {
                    var grade = grades.Where(g => g.ItemId == assignmentItem.Id).FirstOrDefault();
                    if(grade!=null) grade.ParentFolderId = assignmentFolderItem.Id;
                });
            }

            return grades.ToList();
        }

        /// <summary>
        /// Replaces that found symbols in the Worksheet with a dictionary of dynamic replacement strings
        /// </summary>
        /// <param name="ws">Excel Work Sheet Object</param>
        /// <param name="foundSymbols">A dictionary object
        /// String - key being the symbol
        /// List - value being a list of Excel Cells that contained this symbol
        /// NOTE: A symbol is considered any text within a cell that matches the following regular expression /(_\w+_)/
        /// </param>
        /// <param name="replacements">A dictionary object
        /// String - key being a known symbol 
        /// String - replacement value
        /// </param>
        private void ReplaceSymbolsInCells(ExcelWorksheet ws, IDictionary<String, IList<ExcelRangeBase>> foundSymbols, IDictionary<String, String> replacements)
        {
            foreach (var rpl in foundSymbols)
            {
                var replacementKey = rpl.Key;
                var replacementCell = rpl.Value.ToList();
                var replacementVal = "";

                if (replacements.ContainsKey(replacementKey))
                    replacementVal = replacements[replacementKey];

                replacementCell.ForEach(rc => rc.Value = rc.Value.ToString().Replace(replacementKey, replacementVal));
            }
        }

        /// <summary>
        /// Finds all symbols with in an Excel WorkBook
        /// </summary>
        /// <param name="ws">Excel Work Sheet Object</param>
        /// <returns>A dictionary object
        /// String - key being the symbol
        /// List - value being a list of Excel Cells that contained this symbol
        /// NOTE: A symbol is considered any text within a cell that matches the following regular expression /(_\w+_)/
        /// </returns>
        private Dictionary<string, IList<ExcelRangeBase>> GetReplacementCells(ExcelWorksheet ws)
        {
            var replacementCells = new Dictionary<string, IList<ExcelRangeBase>>();

            var replacementSymbol = new Regex(@"(_\w+_)");

            foreach (var cell in ws.Cells)
            {
                if (cell.Value as string == null)
                {
                    continue;
                }
                var matches = replacementSymbol.Matches(cell.Value.ToString());

                foreach (var match in matches)
                {
                    var key = match.ToString();
                    if (replacementCells.ContainsKey(key))
                    {
                        replacementCells[key].Add(cell);
                    }
                    else
                    {
                        replacementCells.Add(match.ToString(), new List<ExcelRangeBase>{ cell });
                    }
                }
            }

            return replacementCells;
        }

        /// <summary>
        /// returns the assignment folder grades of a student
        /// </summary>     
        private IList<Grade> GetAssignmentFolderGrades(IDictionary<BizDC.ContentItem, IList<BizDC.ContentItem>> assignments, IList<Grade> assignedItemGrades)
        {
            var assignedFolderGrades = new List<Grade>();
            foreach (var assignmentFolder in assignments)
            {
                var assignmentFolderGrade = new Grade();
                var assignmentFolderItem = assignmentFolder.Key;
                var assignmentItems = assignmentFolder.Value;
                double possibleScore = 0, achievedScore = 0;

                assignmentFolderGrade.ItemId = assignmentFolderItem.Id;
                assignmentFolderGrade.IsAssignmentFolder = true;

                assignmentItems.ToList().ForEach(delegate(BizDC.ContentItem assignmentItem)
                {
                    var assignmentItemGrade = assignedItemGrades.Where(g => g.ItemId == assignmentItem.Id).FirstOrDefault();
                    if (assignmentItemGrade != null)
                    {
                        possibleScore += assignmentItemGrade.Possible;
                        achievedScore += assignmentItemGrade.Achieved;
                    }
                });

                assignmentFolderGrade.Possible = possibleScore;
                assignmentFolderGrade.Achieved = achievedScore;
                assignedFolderGrades.Add(assignmentFolderGrade);
            }

            return assignedFolderGrades;
        }

        /// <summary>
        /// Returns an average for graded assessments
        /// </summary>
        /// <param name="grades">Grades to calculate an average for</param>
        /// <param name="assigned">If the assessments are assigned or unassigned </param>
        /// <returns></returns>
        private double GetAverage(IEnumerable<Grade> grades, bool assigned)
        {
            double classAvg = 0;
            if (assigned)
            {
                var graded = grades.Where(g => g.IsGraded);
                if(graded.Count() > 0)
                {
                    classAvg = Math.Round(graded.Average(delegate(Grade grade)
                    {
                        return (grade.Possible > 0 ? grade.Achieved / grade.Possible : 0);
                    }),3);
                }
            }
            else
            {
                var graded = grades.Where(g => g.Possible != 0);
                if (graded.Count() > 0)
                {
                    classAvg = Math.Round(graded.Average(delegate(Grade grade)
                                {
                                    return grade.Achieved / grade.Possible;
                                }), 3);
                }
            }
            return classAvg;
        }

        /// <summary>
        /// Gets the list of assigned items in the current course.
        /// </summary>
        /// <param name="assignments"></param>
        /// <returns></returns>
        private IList<BizDC.ContentItem> GetAssignedItems(IDictionary<BizDC.ContentItem, IList<BizDC.ContentItem>> assignments)
        {
            var assignedItemLst = new List<BizDC.ContentItem>();

            foreach (var assingmentFolder in assignments)
            {
                assignedItemLst.AddRange(assingmentFolder.Value);
            }

            return assignedItemLst;
        }

        /// <summary>
        /// gets the last updated date for the student gradebook
        /// </summary>
        /// <returns></returns>
        private string GetLastUpdatedDate(StudentGradebook studentGradeBook)
        {
            var lastUpdatedDate = string.Empty;
            var studentGrades = studentGradeBook.Grades.Concat(studentGradeBook.UnAssingedGrades).Where(g => g.IsGraded);
            if (studentGrades.Count() > 0)
            {
                lastUpdatedDate = studentGrades.Max(g => g.ScoredDate).Value.ToString("MM/dd/yyyy HH:mm tt");
            }
            return lastUpdatedDate;
        }
    }
}
