<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<Bfw.PX.PXPub.Models.QuizCollection>>" %>
<%@ Import Namespace="Bfw.PX.PXPub.Controllers" %>

<%
    var mode = (QuizBrowserMode) ViewData["mode"];
    var courseFilterMetadata = (ViewData["CourseFilterMetadata"] == null) ? null : (IEnumerable<QuestionFilterMetadata>)ViewData["CourseFilterMetadata"];
%>
<div class="browsercategories">
    <% if (!Model.IsNullOrEmpty())
       {
           foreach (QuizCollection collection in Model)
           {
               QuizCollectionType classType = collection.CollectionType;
               string questionclass = "";
               string question_info = "";
               string rootitemId = "";
               switch (classType)
               {
                   case QuizCollectionType.ByPublisher:
                       questionclass = "browsebypublisher";
                       question_info = "Questions by chapter";
                       rootitemId = "PX_LOR";
                       break;
                   case QuizCollectionType.InExistingAssessment:
                       questionclass = "browseinassessment";
                       question_info = "Questions by assessment";
                       rootitemId = "PX_MY_QUESTIONS";
                       break;
                   case QuizCollectionType.ByMe:
                       questionclass = "browsemyquestions";
                       question_info = "Questions I've edited or created";
                       rootitemId = "PX_EDITED";
                       break;
               }
    %>  
            <div class="<%= questionclass %> browse-question-category ">
                <span class="<%= questionclass %>-info"><%= mode != QuizBrowserMode.Resources ? question_info : string.Empty%></span>
                <input id="browsermainpagetype" type="hidden" value="<%= rootitemId %> " />
                <div id="browseQuestionList">
                    <% if (mode == QuizBrowserMode.Resources && classType != QuizCollectionType.ByMe) //
                       {
                           ViewData["Title"] = question_info;
                           Html.RenderPartial("~/Views/BrowseMoreResourcesWidget/ResourceBreadcrumb.ascx", ViewData); //UserCreatedOrEdited renders its own breadcrumb
                       }
                       if (classType == QuizCollectionType.ByMe)
                       {//renders the breadcumb as well
                           Html.RenderAction("UserCreatedOrEdited", "QuizEdit");
                       }
                       else if (collection.Items.Count > 0)
                       {
                           Func<string, object> convert = str =>
                           {
                               int number = 0;
                               int.TryParse(str, out number);
                               return number;
                           };
                           var itemList = collection.Items.OrderBy(i => Regex.Split((i.Title == null) ? String.Empty : i.Title.Replace(" ", ""), "([0-9]+)").Select(convert)
                                                                       , new EnumerableComparer<object>());
                           Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/BrowseByType.ascx", itemList, ViewData);
                       }
                       else
                       { %>
                        <div class="no-questions">
                            No existing assignments in this course.
                        </div>
                    <% } %>
                </div>
            </div>
    <%
           }
       }
    %>
    <div class="browsemyquestions browse-question-category" style="display: none;">
        <span class="browsemyquestions-info">Questions I've edited or created</span>
        <ul class="quiz-item-links">
            <li>
                <div class="click-target">
                    <a id="currently-in-use" class="my-edited-question" href="#">Currently in use</a>
                </div>
            </li>
            <li>
                <div class="click-target">
                    <a id="not-currently-in-use" class="my-edited-question" href="#">Not currently in use</a>
                </div>
            </li>
        </ul>
    </div>


</div>