using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.Common.Pagination;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizQuestion = Bfw.PX.Biz.DataContracts.Question;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using ModelQuestion = Bfw.PX.PXPub.Models.Question;

namespace Bfw.PX.PXPub.Controllers.Admin
{
    internal abstract class QuestionAdminHelper
    {
        #region internal Functions
        //***********************************************************************

        /// <summary>
        /// Populate Status List
        /// </summary>
        /// <param name="searchPanelModel"></param>
        internal static void PopulateStatusList(QuestionAdminSearchPanel searchPanelModel)
        {
            searchPanelModel.StatusSelectList = new List<SelectListItem>
    		                                    	{
    		                                    		new SelectListItem {Text = "Any", Value = "0", Selected = true},
    		                                    		new SelectListItem {Text = "flagged", Value = "1"},
    		                                    		new SelectListItem {Text = "not flagged", Value = "2"}
    		                                    	};
        }


        /// <summary>
        /// Populate QuizzesList after populating ChaptersList
        /// </summary>
        /// <param name="searchPanelModel"></param>
        /// <param name="entityId"> </param>
        /// <param name="QuestionAdminActions"> </param>
        internal static void PopulateQuizzesList(QuestionAdminSearchPanel searchPanelModel, string entityId, BizSC.IQuestionActions QuestionActions)
        {
            List<BizDC.ContentItem> allQuizzes = QuestionActions.GetCourseQuizzes(entityId).ToList();

            IEnumerable<ContentItem> chapters =
                searchPanelModel.ChapterSelectList.Select(ch => new ContentItem { Id = ch.Value, Title = ch.Text });

            var quizzesToDisplay = GetQuizzesWithDescription(chapters.ToList(), allQuizzes);

            searchPanelModel.QuizSelectList = quizzesToDisplay.Select(item =>
                                                             new SelectListItem
                                                             {
                                                                 Text = item.Title,
                                                                 Value = item.Id
                                                             });

            //searchPanelModel.QuizSelectList = searchPanelModel.QuizSelectList.Add("Any", "0");
        }


        /// <summary>
        /// Get Quizzes With Descriptions
        /// </summary>
        /// <param name="chaptersList"></param>
        /// <param name="allQuizzes"></param>
        /// <returns></returns>
        internal static IEnumerable<ContentItem> GetQuizzesWithDescription(IEnumerable<ContentItem> chaptersList, List<ContentItem> allQuizzes)
        {
            List<BizDC.ContentItem> quizzesWithTitle = null;

            foreach (var chapter in chaptersList)
            {
                var chapterId = chapter.Id;
                foreach (var quiz in allQuizzes.Where(quiz => quiz.ParentId == chapterId))
                {
                    quiz.Description = chapter.Title;
                    if (quizzesWithTitle == null) quizzesWithTitle = new List<ContentItem>();
                    quizzesWithTitle.Add(new ContentItem { Id = quiz.Id, Title = String.Format("{0} - {1}", quiz.Description, quiz.Title) });
                }
            }

            if (quizzesWithTitle != null)
            {
                var quizzesToDisplay = quizzesWithTitle.Distinct().OrderBy(i => i.Title).ToList();
                return quizzesToDisplay;
            }

            return null;
        }

        /// <summary>
        /// Populate Chapters List
        /// </summary>
        /// <param name="searchPanelModel"></param>
        /// <param name="entityId"> </param>
        /// <param name="QuestionAdminActions"> </param>
        /// <returns></returns>
        internal static List<BizDC.ContentItem> PopulateChaptersList(QuestionAdminSearchPanel searchPanelModel, string entityId,
                                                                BizSC.IQuestionActions QuestionActions)
        {
            List<BizDC.ContentItem> chapters = QuestionActions.GetCourseChapters(entityId);
            if (!chapters.IsNullOrEmpty())
            {
                chapters = chapters.OrderBy(i => i.Sequence).ToList();
            }

            searchPanelModel.ChapterSelectList = chapters.Select(item =>
                                                                 new SelectListItem
                                                                 {
                                                                     Text = String.Format("{0}", item.Title),
                                                                     Value = item.Id
                                                                 });
            //searchPanelModel.ChapterSelectList = searchPanelModel.ChapterSelectList.Add("Any", "0");

           
            return chapters;
        }

        /// <summary>
        /// Populate Format List
        /// </summary>
        /// <param name="searchPanelModel"></param>
        /// <param name="QuestionTypes"> </param>
        internal static void PopulateFormatList(QuestionAdminSearchPanel searchPanelModel, Dictionary<string, string> QuestionTypes)
        {
            searchPanelModel.FormatSelectList = QuestionTypes.Select(Type =>
                                                                     new SelectListItem
                                                                     {
                                                                         Text = Type.Value,
                                                                         Value = Type.Key
                                                                     });

            //searchPanelModel.FormatSelectList = searchPanelModel.FormatSelectList.Add("Any", "0");

            searchPanelModel.FormatSelectList.First().Selected = true;


            if (searchPanelModel.FormatSelectedValues == null)
                searchPanelModel.FormatSelectList.First().Selected = true;

            else if (searchPanelModel.FormatSelectedValues != null && searchPanelModel.FormatSelectedValues.Any())
            {
                string defaultFormat = searchPanelModel.FormatSelectedValues.First();
                var selectedItem = searchPanelModel.FormatSelectList.FirstOrDefault(f => f.Value == defaultFormat);
                if (selectedItem != null)
                {
                    searchPanelModel.FormatSelectList.First(f => f.Value == defaultFormat).Selected = true;
                }
            }
        }


        /// <summary>
        /// Is Search Conditions SetUp
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        internal static bool IsSearchConditionsSetUp(QuestionAdminSearchPanel searchModel)
        {
            if (searchModel != null &&
                                         (
                                          (searchModel.ChapterSelectedValues != null && searchModel.ChapterSelectedValues.All(i => i != "0")) ||
                                          (searchModel.QuizSelectedValues != null && searchModel.QuizSelectedValues.All(i => i != "0")) ||
                                          (searchModel.FormatSelectedValues != null && searchModel.FormatSelectedValues.All(i => i != "0")) ||
                                          (searchModel.FlagSelectedValues != null && searchModel.FlagSelectedValues.All(i => i != "0")) ||
                                          (searchModel.StatusSelectedValues != null && searchModel.StatusSelectedValues.All(i => i != "0")) ||
                                          (!string.IsNullOrEmpty(searchModel.SearchKeyword) && !searchModel.SearchKeyword.Contains("keyword")
                                          )
                                         )
              ) return true;
            return false;
        }

        /// <summary>
        /// Get Questions List To View
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="questionIds"></param>
        /// <param name="searchQuestionsFilter"></param>
        /// <param name="count"></param>
        /// <param name="ContentActions"> </param>
        /// <param name="questionActions"> </param>
        /// <returns></returns>
        internal static IEnumerable<ModelQuestion> GetQuestionsListToView(string entityId, IEnumerable<string> questionIds, string searchQuestionsFilter, int? count, BizSC.IContentActions ContentActions, BizSC.IQuestionActions questionActions)
        {
            List<BizQuestion> bizQuestions = questionActions.GetQuestions(entityId, questionIds, searchQuestionsFilter, count.GetValueOrDefault()).ToList();
            IEnumerable<ModelQuestion> questionsToView = bizQuestions.Select(q => q.ToQuestion()).Distinct();
            return questionsToView;
        }

        /// <summary>
        /// Set Pagination Options For Search Questions Filter
        /// </summary>
        internal static void SetPaginationOptionsForSearchQuestionsFilter()
        {
            PaginationOptions.ShowNext = true;
            PaginationOptions.ShowLast = false;
            PaginationOptions.ShowPrevious = true;
            PaginationOptions.ShowFirst = false;

            PaginationOptions.DisableFirstIfNoMorePages = true;
            PaginationOptions.DisableLastIfNoMorePages = true;
            PaginationOptions.DisableNextIfNoMorePages = true;
            PaginationOptions.DisablePreviousIfNoMorePages = true;
            PaginationOptions.ShowDisplayingItemsLegend = true;
            PaginationOptions.ShowDisplayingItemsAlways = true;

            PaginationOptions.PageSize = 25;
            PaginationOptions.PagesInPagination = 25;

            PaginationOptions.DisplayingItemsText = "Displaying 25 items ";
            PaginationOptions.NextText = ">";
            PaginationOptions.LastText = "";
            PaginationOptions.PreviousText = "<";
            PaginationOptions.FirstText = "";
        }

        /// <summary>
        /// Create Pagination For Items To View
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="itemsToView"> </param>
        /// <param name="totalCount"></param>
        /// <param name="pageSubmitFunction"> </param>
        /// <returns></returns>
        internal static IEnumerable<T> CreatePaginationForItemsToView<T>(QuestionAdminSearchPanel searchModel, IEnumerable<T> itemsToView, int totalCount, string pageSubmitFunction)
        {
            int currentPage;
            int iToSkip;

            int currentPageByButton;

            if (searchModel.Pagination == null)
            {
                currentPage = 1;
                currentPageByButton = 1;
            }
            else
            {
                currentPage = searchModel.Pagination.CurrentPage;
                currentPageByButton = searchModel.Pagination.CurrentPageByButton;
                if (searchModel.Pagination.TotalItems != totalCount & searchModel.Pagination.TotalItems != 0)
                {
                    currentPage = 1;
                    currentPageByButton = 1;
                }
            }

            searchModel.Pagination = new Pagination
            {
                CurrentPage = currentPage,
                CurrentPageByButton = currentPageByButton,
                TotalItems = totalCount,
                PageSubmitFunction = pageSubmitFunction //"PxQuestionAdmin.submitPagination",

            };

            iToSkip = searchModel.Pagination.Skip;
            int iToTtake = searchModel.Pagination.Take;

            itemsToView = itemsToView.Skip(iToSkip).Take(iToTtake).ToList();
            return itemsToView;
        }

        /// <summary>
        /// Add All AssignedQuizzes
        /// </summary>
        /// <param name="questionsAdminList"></param>
        /// <param name="questionsToView"></param>
        /// <returns></returns>
        internal static IEnumerable<ModelQuestion> AddAllAssignedQuizzes(IEnumerable<QuestionAdmin> questionsAdminList, IEnumerable<ModelQuestion> questionsToView)
        {
            if (questionsToView == null) return null;

            List<ModelQuestion> questionsWithQuizzes = null;

            foreach (var question in questionsToView)
            {

                var quizzesForCurrentQuestion = questionsAdminList.Where(itm => itm.QuestionId == question.Id).Select(qz => new Bfw.PX.PXPub.Models.ContentItem() { Id = qz.QuizId }).ToList();

                question.AssignedQuizes = quizzesForCurrentQuestion;

                if (questionsWithQuizzes == null) questionsWithQuizzes = new List<ModelQuestion>();

                questionsWithQuizzes.Add(question);
            }

            return questionsWithQuizzes;
        }

        /// <summary>
        /// Add Assigned Quizzes
        /// </summary>
        /// <param name="questionsAdminList"></param>
        /// <param name="questionsToView"></param>
        /// <returns></returns>
        internal static IEnumerable<ModelQuestion> AddAssignedQuizzes(IEnumerable<QuestionAdmin> questionsAdminList, IEnumerable<ModelQuestion> questionsToView)
        {
            if (questionsToView == null) return null;

            List<ModelQuestion> questionsWithQuizzes = null;

            foreach (var question in questionsToView)
            {

                IList<Bfw.PX.PXPub.Models.ContentItem> quizzesForCurrentQuestion = questionsAdminList.Where(itm => itm.QuestionId == question.Id)
                                                                                 .Select(qz => new Bfw.PX.PXPub.Models.ContentItem() { Id = qz.QuizId, ParentId=qz.ParentId})
                                                                                 .Distinct()
                                                                                 .ToList();

                question.AssignedQuizes = quizzesForCurrentQuestion;
                question.AssignedChapter = quizzesForCurrentQuestion.SingleOrDefault().ParentId;  
                if (questionsWithQuizzes == null) questionsWithQuizzes = new List<ModelQuestion>();

                questionsWithQuizzes.Add(question);
            }

            return questionsWithQuizzes;
        }


        /// <summary>
        /// Add Assigned Quizzes
        /// </summary>
        /// <param name="quizzes"></param>
        /// <param name="questionsToView"></param>
        /// <returns></returns>
        internal IEnumerable<ModelQuestion> AddFirstAssignedQuizToEachQuestion(IEnumerable<ContentItem> quizzes, IEnumerable<ModelQuestion> questionsToView)
        {
            List<ModelQuestion> questionsWithQuizzes = null;

            foreach (var question in questionsToView)
            {
                string quizId = (from quiz in quizzes
                                 let quizQuestions = quiz.QuizQuestions
                                 where quizQuestions.Any(q => q.QuestionId == question.Id)
                                 select quiz.Id).FirstOrDefault();

                if (string.IsNullOrEmpty(quizId)) continue;

                question.AssignedQuizes = new List<Models.ContentItem> { new Models.ContentItem { Id = quizId, Type = "Assessment"} }; ;

                if (questionsWithQuizzes == null) questionsWithQuizzes = new List<ModelQuestion>();

                questionsWithQuizzes.Add(question);
            }

            return questionsWithQuizzes;
        }


        /// <summary>
        /// Add First Found Assigned Quizzes PerPage
        /// </summary>
        /// <param name="quizzes"></param>
        /// <param name="questionsToView"></param>
        /// <param name="searchPanel"></param>
        /// <returns></returns>
        internal static IEnumerable<ModelQuestion> AddFirstFoundAssignedQuizzesPerPage(IEnumerable<ContentItem> quizzes, IEnumerable<ModelQuestion> questionsToView, QuestionAdminSearchPanel searchPanel = null)
        {
            List<ModelQuestion> questionsWithQuizzes = null;

            int nextPageStartSearchQuestion = 0;
            int nextPageStartSearchQuiz = 0;

            if (searchPanel != null)
            {
                nextPageStartSearchQuestion = searchPanel.NextPageStartSearchQuestion;
                nextPageStartSearchQuiz = searchPanel.NextPageStartSearchQuiz;
            }

            var arrayOfQuestions = questionsToView.OrderBy(q => q.Id).Skip(nextPageStartSearchQuestion).ToArray();
            var arrayOfQuizzes = quizzes.OrderBy(q => q.Id).Skip(nextPageStartSearchQuiz).ToArray();

            for (int i = 0; i < arrayOfQuestions.Count(); i++)
            {
                string quizId = null;
                var question = arrayOfQuestions[i];

                for (int z = 0; z < arrayOfQuizzes.Count(); z++)
                {
                    var quiz = arrayOfQuizzes[z];
                    IEnumerable<BizDC.QuizQuestion> quizQuestions = quiz.QuizQuestions;
                    if (quizQuestions.All(q => q.QuestionId != question.Id)) continue;

                    quizId = quiz.Id;
                    nextPageStartSearchQuiz = z + 1;
                    break;
                }

                if (string.IsNullOrEmpty(quizId)) continue;

                question.AssignedQuizes = new List<Models.ContentItem> { new Models.ContentItem() { Id = quizId, Type = "Assessment"} };
                nextPageStartSearchQuestion = i + 1;

                if (questionsWithQuizzes == null) questionsWithQuizzes = new List<ModelQuestion>();
                questionsWithQuizzes.Add(question);

                if (searchPanel == null) continue;
                searchPanel.NextPageStartSearchQuestion = nextPageStartSearchQuestion;
                searchPanel.NextPageStartSearchQuiz = nextPageStartSearchQuiz;

                if (questionsWithQuizzes.Count > PaginationOptions.PageSize) break;
            }

            if (questionsWithQuizzes == null) return questionsToView;
            return questionsWithQuizzes;
        }

        /// <summary>
        /// Get List of Question IDs for List of Quizzes
        /// </summary>
        /// <param name="quizzes"></param>
        /// <returns></returns>
        internal static IEnumerable<QuestionAdmin> GetQuestionsAdminList(IEnumerable<ContentItem> quizzes)
        {
            List<QuestionAdmin> questionsAdminList = null;

            if (quizzes != null)
            {
                var quizzesWithQuestionIds = quizzes.Select(qz => new ContentItem { QuizQuestions = qz.QuizQuestions, Id = qz.Id , ParentId = qz.DefaultCategoryParentId});

                foreach (ContentItem quiz in quizzesWithQuestionIds)
                {
                    var quizId = quiz.Id;

                    var questions = quiz.QuizQuestions;

                    var questionsToAdd = questions.Select(q => new QuestionAdmin { QuizId = quizId, QuestionId = q.QuestionId, ParentId = quiz.ParentId });

                    if (questionsAdminList == null) questionsAdminList = new List<QuestionAdmin>();

                    questionsAdminList.AddRange(questionsToAdd);
                }
            }
            return questionsAdminList;
        }

        /// <summary>
        /// Get Quizzes
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="entityId"> </param>
        /// <param name="ContentActions"> </param>
        /// <param name="QuestionActions"> </param>
        /// <returns></returns>
        internal static IEnumerable<ContentItem> GetQuizzes(QuestionAdminSearchPanel searchModel, string entityId, BizSC.IContentActions ContentActions, BizSC.IQuestionActions QuestionActions)
        {
            IEnumerable<ContentItem> quizzes;
            if (searchModel.QuizSelectedValues != null && searchModel.QuizSelectedValues.All(i => i != "0"))
            {
                quizzes = ContentActions.GetItems(entityId, searchModel.QuizSelectedValues.ToList(), includingShortCuts: true);
            }
            else if (searchModel.ChapterSelectedValues != null && searchModel.ChapterSelectedValues.All(i => i != "0"))
            {
                quizzes = QuestionActions.GetQuizzesForSelectedChapters(entityId, searchModel.ChapterSelectedValues);
            }
            else
            {
                quizzes = QuestionActions.GetCourseQuizzes(entityId);
            }

            return quizzes;
        }

        /// <summary>
        /// Creates Query For Search Conditions
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        internal static string GetSearchQuestonsFilter(QuestionAdminSearchPanel searchModel)
        {
            //Create searchQuestonsFilter
            string searchQuestonsFilter = "";

            //InteractionType:
            if (searchModel.FormatSelectedValues != null && searchModel.FormatSelectedValues.Count() != 0 && searchModel.FormatSelectedValues.ToList()[0] != "0")
            {
                searchQuestonsFilter = searchModel.FormatSelectedValues.Select(v => string.Format("/interaction@type='{0}'", v)).Fold(" OR ");
                if (searchQuestonsFilter != "") searchQuestonsFilter = string.Format("({0})", searchQuestonsFilter);
            }

            //Question Flag:
            if (searchModel.FlagSelectedValues != null && searchModel.FlagSelectedValues.Count() != 0)
            {
                string flagFilter = "";
                foreach (var status in searchModel.FlagSelectedValues.Where(s => s != "0"))
                {
                    var adminFlag = "='true'";

                    if (status == "2") adminFlag = String.Format("!{0}", adminFlag);

                    flagFilter = (flagFilter == "") ? String.Format("/meta-data/adminflag{0}", adminFlag) :
                                                          String.Format("{0} OR {1}", flagFilter, String.Format("/meta-data/adminflag{0}", adminFlag));
                }

                if (flagFilter != "" && searchQuestonsFilter != "") searchQuestonsFilter = string.Format("{0} AND ({1})", searchQuestonsFilter, flagFilter);
                else if (flagFilter != "") searchQuestonsFilter = flagFilter;
            }

            if (searchModel.StatusSelectedValues != null && searchModel.StatusSelectedValues.Count() != 0 && searchModel.StatusSelectedValues.ToList()[0] != "0")
            {
                string statusFilter = "";
                foreach (var status in searchModel.StatusSelectedValues.Where(s => s != "0"))
                {

                    statusFilter = (statusFilter == "") ? String.Format("/meta/questionstatus{0}", status) :
                                                          String.Format("{0} OR {1}", statusFilter, String.Format("/meta-data/adminflag{0}", status));
                }

                if (statusFilter != "" && searchQuestonsFilter != "") searchQuestonsFilter = string.Format("{0} AND ({1})", searchQuestonsFilter, statusFilter);
                else if (statusFilter != "") searchQuestonsFilter = statusFilter;
            }

            ////Keyword:
            //string searchKeyword = searchModel.SearchKeyword;
            //if (!string.IsNullOrEmpty(searchKeyword) && !searchKeyword.Contains("keyword"))
            //{
            //    searchKeyword = string.Format("/meta-data/keyword='{0}'", searchModel.SearchKeyword);
            //    searchQuestonsFilter = string.Format("{0} AND {1}", searchQuestonsFilter, searchKeyword);
            //}
            return searchQuestonsFilter;
        }

        /// <summary>
        /// Apply Search Filter
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="questionsList"></param>
        /// <param name="QuestionTypes"> </param>
        /// <returns></returns>
        internal static IEnumerable<ModelQuestion> ApplySearchFilter(QuestionAdminSearchPanel searchModel, IEnumerable<ModelQuestion> questionsList, Dictionary<string, string> QuestionTypes)
        {
            if (questionsList == null) return null;
            List<ModelQuestion> filterdQuestionsList = null;

            //InteractionType:           
            if (searchModel.FormatSelectedValues != null && searchModel.FormatSelectedValues.Count() != 0 && searchModel.FormatSelectedValues.All(v => v != "0"))
            {
                List<string> formatSelectedValues = searchModel.FormatSelectedValues.Where(f => f != "0").ToList();

                foreach (var format in formatSelectedValues)
                {
                    var responseFormat = format; //QuestionTypes.First(t => t.Key == format).Value;
                    foreach (var question in questionsList)
                    {
                        if (AdminQuestionType(question.Type).ToLower() != responseFormat.ToLower()) continue;
                        if (filterdQuestionsList == null) filterdQuestionsList = new List<ModelQuestion>();
                        filterdQuestionsList.Add(question);

                    }

                }

                if (filterdQuestionsList == null || !filterdQuestionsList.Any()) return null;
            }

            //Question Flag:
            if (searchModel.StatusSelectedValues != null && searchModel.StatusSelectedValues.Count() != 0)
            {
                List<ModelQuestion> foundRecords = new List<ModelQuestion>();
                if (filterdQuestionsList == null) filterdQuestionsList = questionsList.ToList();

                List<string> SelectedValues = searchModel.StatusSelectedValues.ToList();

                foreach (var status in SelectedValues)
                {

                    if (!filterdQuestionsList.IsNullOrEmpty())
                    {

                        foundRecords.AddRange(filterdQuestionsList.Where(q => q.QuestionStatus == status));
                    }


                }

                if (foundRecords == null || !foundRecords.Any()) return null;
                filterdQuestionsList = foundRecords.ToList();
            }
          
            //Question Status: filter QuestionAdmin Flag for Flagging question
            if (searchModel.FlagSelectedValues != null && searchModel.FlagSelectedValues.Count() != 0 && searchModel.FlagSelectedValues.All(v => v != "0"))
            {
                List<ModelQuestion> foundRecords = null;
                if (filterdQuestionsList == null) filterdQuestionsList = questionsList.ToList();

                //List<string> StatusSelectedValues = searchModel.FlagSelectedValues.Where(s => s != "0").ToList();
                //foundRecords = filterdQuestionsList.Where(q => StatusSelectedValues.Contains(q.QuestionStatus)).ToList();
                string  StatusSelectedValues = searchModel.FlagSelectedValues.Where(s => s != "0").SingleOrDefault();
                if (!string.IsNullOrEmpty(StatusSelectedValues))
                {
                    switch (StatusSelectedValues)
                    {
                        case "1":
                            foundRecords = filterdQuestionsList.Where(q => q.AdminFlag == true).ToList();
                            break;
                        case "2":
                            foundRecords = filterdQuestionsList.Where(q => q.AdminFlag == false).ToList();
                            break;
                    }
                }
                if (foundRecords == null || !foundRecords.Any()) return null;
                filterdQuestionsList = foundRecords.ToList();
            }


            string searchKeyword = searchModel.SearchKeyword;
            if (!string.IsNullOrEmpty(searchKeyword) && !searchKeyword.Contains("keyword"))
            {
                //Metadata Keyword Will be used in searchQuestionsFilter later
                searchKeyword = keywordPattern(searchModel.SearchKeyword);

                if (filterdQuestionsList == null) filterdQuestionsList = questionsList.ToList();
                List<ModelQuestion> foundRecords = filterdQuestionsList.Where(q => (Regex.IsMatch(q.Text, searchKeyword))).ToList();

                if (!foundRecords.Any()) return null;

                return foundRecords.ToList();
            }

            if (filterdQuestionsList != null && filterdQuestionsList.Any()) return filterdQuestionsList;

            return questionsList;
        }


        /// <summary>
        /// Gets the type of the question.
        /// </summary>
        /// <value>
        /// The type of the question.
        /// For mapping references see web.config:
        /// key="QuestionTypes" value="choice:Multiple Choice|text:Short Answer|essay:Essay|match:Matching|answer:Multiple Answer|custom:Advanced Question"
        /// </value>
        internal static string AdminQuestionType(string type)
        {
            if (string.IsNullOrEmpty(type)) return string.Empty;

            switch (type.ToUpper())
            {
                case "MC":
                    return "choice";
                case "A":
                    return "answer";
                case "E":
                    return "essay";
                case "MT":
                    return "match";
                case "TXT":
                    return "text";

                case "HTS":
                case "GRAPH":
                case "CUSTOM":
                    return "custom";

                case "COMP":
                    return "composite";
                default:
                    return type;
            }

        }


        internal static string keywordPattern(string searchKeyword)
        {
            var keywords = searchKeyword.Split(' ').Select(k => k.Trim()).Where(k => k != "").Select(Regex.Escape);
            return @"\b(" + string.Join("|", keywords) + @")\b";
        }

        internal static IEnumerable<string> KeywordSearch(QuestionAdminSearchPanel searchModel, string entityId, BizSC.ISearchActions SearchActions)
        {
            List<string> questionIds = new List<string>();

            Models.SearchQuery query = new Models.SearchQuery();
            if (!string.IsNullOrEmpty(searchModel.SearchKeyword))
                query.IncludeWords = searchModel.SearchKeyword;

            query.Rows = 1000;
            if (System.Configuration.ConfigurationManager.AppSettings["QuestionSearchMaxRowCount"] != null)
            {
                int rowCount;
                if (int.TryParse(System.Configuration.ConfigurationManager.AppSettings["QuestionSearchMaxRowCount"], out rowCount))
                    query.Rows = rowCount;
            }

            query.Start = 0;
            query.ClassType = "question";
            query.EntityId = entityId;

            query.ExactQuery = BuildExactQuery(searchModel);
            var SearchResults = new AdvancedSearchResults { Query = query };

            var bizQuery = query.ToSearchQuery();
            try
            {
                SearchActions.DoProductSearch(false);
                var srs = SearchActions.DoSearch(bizQuery);
                SearchResults.Results = srs.ToSearchResults();

            }
            catch (Exception)
            {
                //Fix:- If Solr search throws error, show the user
                // as 0 results found.
                SearchResults.Results = new SearchResults { numFound = "0" };
            }
            var results = SearchResults.Results;

            questionIds.AddRange(results.docs.Map(q => q.dlap_id.Split('|')[2]));
            return questionIds.Distinct();
        }

        internal static string BuildExactQuery(QuestionAdminSearchPanel searchModel)
        {
            string searchQuestonsFilter = "";
            //InteractionType:
            if (searchModel.FormatSelectedValues != null && searchModel.FormatSelectedValues.Count() != 0 && searchModel.FormatSelectedValues.ToList()[0] != "0")
            {
                searchQuestonsFilter = searchModel.FormatSelectedValues.Select(v => string.Format("{0}", v)).Fold(" ");
                if (searchQuestonsFilter != "") searchQuestonsFilter = string.Format("(dlap_q_type:{0})", searchQuestonsFilter);
            }

            string searchFlagFilter = "";
            //Question Flag:
            if (searchModel.FlagSelectedValues != null && searchModel.FlagSelectedValues.Count() != 0 && searchModel.FlagSelectedValues.ToList()[0] != "0")
            {
                switch (searchModel.FlagSelectedValues.ToList()[0])
                {
                    case "1":
                        searchFlagFilter = string.Format("(adminflag:{0})", "true");
                        break;
                    case "2":
                        searchFlagFilter = string.Format(" NOT (adminflag:{0})", "true");
                        break;
                }

                searchQuestonsFilter = searchQuestonsFilter != "" ? string.Format("{0} AND {1}", searchQuestonsFilter, searchFlagFilter) : searchFlagFilter;
            }

            return searchQuestonsFilter;
        }


        #endregion  internal functions
    }
}
