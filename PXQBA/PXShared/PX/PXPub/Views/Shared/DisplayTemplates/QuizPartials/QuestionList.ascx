<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>
<%@ Import Namespace="Bfw.PX.PXPub.Controllers" %>
<%
    var mode = (QuizBrowserMode) ViewData.GetValue("mode", QuizBrowserMode.QuestionPicker);
    if (mode == QuizBrowserMode.Resources)
    {
        Html.RenderPartial("~/Views/BrowseMoreResourcesWidget/ResourceBreadcrumb.ascx", ViewData);
    }

    bool isQuestionOverview = (ViewData["isQuestionOverview"] == null) ? false : Convert.ToBoolean(ViewData["isQuestionOverview"]);
    string mainQuizId = (ViewData["mainQuizId"] == null) ? "" : ViewData["mainQuizId"].ToString();
    string disciplineId = (ViewData["disciplineId"] == null) ? "" : ViewData["disciplineId"].ToString();

    
    bool allowSelection = true;
    bool allowDrag = true;
    bool showExpand = true;
    bool showAddLink = true;
    bool showAddToPool = true;
    bool showPoints = true;
    if (mode == QuizBrowserMode.Resources)
    {
        allowSelection = false;
        allowDrag = false;
        showExpand = false;
        showAddLink = false;
        showAddToPool = false;
        showPoints = false;
    }
    bool isStringSearch = (bool)ViewData.GetValue("isStringSearch", false);
    // For the SingleQuestion partial view
    ViewData["allowSelection"] = allowSelection;
    ViewData["allowDrag"] = allowDrag;
    ViewData["showExpand"] = showExpand;
    ViewData["showAddLink"] = showAddLink;
    ViewData["showPoints"] = showPoints;
    ViewData["mode"] = mode;
    ViewData["isQuestionOverview"] = isQuestionOverview;
    ViewData["mainQuizId"] = mainQuizId;

    bool runQuizInit = (bool)ViewData.GetValue("RunQuizInit", true);

    if (!ViewData.Keys.Contains("isPoolQuestion"))
    {
        ViewData["isPoolQuestion"] = false;
    }

    bool isPoolQuestion = (bool)ViewData["isPoolQuestion"];
%>

<h2 class="folder-title"></h2>
 <%
    if (Model.QuizPaging != null && Model.Questions != null)
    {
 %>
            <%= Html.HiddenFor(m => m.QuizPaging.LastIndex) %>
            <%= Html.HiddenFor(m => m.QuizPaging.TotalCount) %> 
            <%= Html.Hidden("questionlist_quizid", Model.Id) %> 
            <input type="hidden" class="hidden-question-count" value="<%= Model.QuizPaging.TotalCount %>"/>
<div class="question-count">
    <%  
        var questionString = "";
        var questionsLoaded = int.Parse(Model.QuizPaging.LastIndex);
        var questionsTotal = Model.QuizPaging.TotalCount;
        if (!isStringSearch)
        {
            if (questionsLoaded < questionsTotal)
           {
               questionString = String.Format("Showing {0} questions out of approximately {1} ({2}% loaded...)", questionsLoaded, questionsTotal, Math.Round((double)100 * questionsLoaded / questionsTotal));
           }
           else
           {
               questionString = String.Format("{0} questions", questionsTotal);
           }
       }
       else
       {
           if (questionsLoaded < questionsTotal)
           {
               questionString = String.Format("Showing {0} questions out of {1} found", questionsLoaded, questionsTotal);
           }
           else
           {
               questionString = String.Format("{0} questions found", questionsTotal);
           }
       }
    %>
    <span class="questions-loaded"> <%= questionString %> </span>   
</div>             
<% 
        }
     else 
     {
         if (Model.Questions != null && Model.Questions.Count() > 0)
         { %>
                <div class="question-count"><span class="questions-loaded"> <%= Model.Questions.Count() %> question<%= Model.Questions.Count() > 0 ? "s" : "" %></span></div>
            <% }
         else
         {%>
                <div class="question-count">No questions found</div><% 
         }

     } 
    
%>
<a class="showfilter" href="javascript:"><span class="show-filter-available-question">Show Filters</span></a>
<span class="filter-count-available-question"></span>

<div class="question-filter collapsed"></div>
<span style="display: none;" class="QuizType" ><%= disciplineId %></span>
<div class="question-list"  id="questions-<%=Model.Id %>" style="display: none">
<div id="show-question" class="question-dialog-text"></div>
    <%-- <% if (!Model.Questions.IsNullOrEmpty())
       {%>
        <div class="questions-menu" style="display:block;" ></div>
    <%
        }
       else
       {%>
        <div class="questions-menu" style="display:none;" ></div>
     <%} %>--%>
    <% Html.RenderAction("QuizTemplateId", "Quiz"); %>
    <div class="question-container">
        <span class="initial-drop-target"></span>
        <div class="no-question-text" style="display: none;">
            <span class="no-question">You have not yet added any questions to this assesment.</span>
            <span class="no-question">Browse question banks on the left, then click "Add" or drag and drop to place questions in your assessment.</span>
            <span class="no-question">You can also click "Create" to make your own questions.</span>
        </div>
        <h2 class="quiz-title"><%= Model.Title %></h2>        
        <ul class="questions">
            <% if (!Model.Questions.IsNullOrEmpty())
               {
                   bool isOdd = true;
                   bool isReused = false;

                   foreach (Question question in Model.Questions)
                   {
                       string questionEditedType = "Publisher question (not edited)";
                       if (!question.SearchableMetaData.IsNullOrEmpty())
                       {
                           if (question.SearchableMetaData.Keys.Contains("createdBy"))
                           {
                               questionEditedType = "User-created question";
                           }
                           else if (question.SearchableMetaData.Keys.Contains("publisherSupplied"))
                           {
                               if (question.SearchableMetaData.Keys.Contains("publisherEdited"))
                               {
                                   questionEditedType = "Publisher question (edited)";
                               }
                           }
                       }


                       isReused = false;
                       string extraClass = question.Type.ToLower();
                       if (extraClass == "bank")
                       {
                           extraClass += " quiz";
                       }

                       if (Model.ShowReused && question.AssignedQuizes.Count > 0)
                       {
                           isReused = true;
                       }

                       string attempts = question.Attempts ?? Model.AttemptLimit.ToString();
                       string attemptsText = attempts == "0" ? "Unlimited" : attempts;
                       string review = question.Review ?? "Off";
                       string scrambled = question.Scrambled ?? "Not";
                       string hints = question.Hints ?? "Off";
                       string score = question.Score ?? Model.SubmissionGradeAction;
                       string timelimit = question.TimeLimit ?? Model.TimeLimit.ToString();
                       string timelimitText = timelimit == "0" ? "Unlimited" : timelimit + ":00";
                       IList<LearningCurveQuestionSettings> learngingCurveSettings = question.LearningCurveQuestionSettings;
                       bool isPrimary = false;
                       if (learngingCurveSettings != null && learngingCurveSettings.Count > 0)
                       {
                           isPrimary = (from c in learngingCurveSettings where c.QuizId == Model.Id select c.PrimaryQuestion).FirstOrDefault();
                       }

                       ViewData["isPrimary"] = isPrimary;
                       ViewData["questionEditedType"] = questionEditedType;
                       ViewData["extraClass"] = extraClass;
                       ViewData["isOdd"] = isOdd;
                       ViewData["isReused"] = isReused;
                       ViewData["question"] = question;

                       Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/SingleQuestion.ascx", ViewData);
            %> 
            <% isOdd = !isOdd;
                   }
               } %>
        </ul>
        <% if (Model.QuizPaging != null && Model.QuizPaging.LastIndex != null && Int32.Parse(Model.QuizPaging.LastIndex) < Model.QuizPaging.TotalCount) {
               if (isStringSearch)
               {//if we are searching rather than browsing content, we load my results by user action rather than automatically%>
                <div class="load-more-questions"><a href="javascript:PxQuiz.LoadMoreQuestions(true, '<%= mode.ToString() %>');">Show more questions</a></div>
             <%}
               else
               {//show loading container as we load results automatically  %>
                    <div class="quiz-list-loading"></div>
            <% } %>
        <% }%>
        <div class="clear"></div>
    </div>
</div>
<% if (Model is LearningCurveActivity)
   { %>
    <div id="edit-lc-question-settings-dialog">
    </div>
    <div id="preview-lc-question-dialog">
        <%
            var rand = new Random();
            string frameHostId = string.Format("lc-viewer-frame-host-{0}", rand.Next());
        %>
        <div id="<%= frameHostId %>" class="lc_iframe-host" style="height: 600px"></div>
    </div>
<% } %>

<% if ((Model.CourseInfo != null))
   { %>
    <div id="question-card-template" style="display: none;"><div class="question-card-layout"><% = Model.CourseInfo.QuestionCardLayout %></div></div>
<% } %>
   
 <script type="text/javascript">
   <% if (runQuizInit)
      { %>
          
      
           PxPage.OnReady(function() {
               PxPage.Require(<%= ResourceEngine.JsonFor("~/Scripts/quiz.js") %>, function () {
                   PxQuiz.Init("questions-<%= Model.Id %>");
                   <% if (mode == QuizBrowserMode.Resources)
                          //only start loading additional questions for questions pools, 
                      {
                          if (!isStringSearch)
                          { %>
                               PxQuiz.LoadMoreQuestions(true, "Resources");
                               PxQuiz.QuestionSearchMode = false;    
                   <% }
                          else
                          { %>
                                PxQuiz.QuestionSearchMode = true;    
                          <% }
                      }
                      else if (mode == QuizBrowserMode.QuestionPicker)
                      {
                          if (!isStringSearch)
                          { %>
                               PxQuiz.LoadMoreQuestions(true, "QuestionPicker");
                               PxQuiz.QuestionSearchMode = false;    
                   <% }
                          else
                          { %>
                                PxQuiz.QuestionSearchMode = true;    
                          <% }
                      } %>
               });
           });

       

<% }
      else
      {%>
          $("#questions-<%= Model.Id %>").fadeIn(0, 1);
      <%}%>
</script>