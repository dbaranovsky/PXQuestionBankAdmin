<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<QuizSearchResults>" %>
<div class="question-search-list">
    <div id="search-results-panel">
        <div class="blade">
        </div>
        <%--<span id="search-total" class="px-default-text">
        </span>--%>
        <div id="search-results">
            <div class="ResultList">
                <% ViewData["isStringSearch"] = true; %>
                <% Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/QuestionList.ascx", Model.Quiz, ViewData); %>
                <%= Html.HiddenFor(m => m.Query.IncludeWords) %>
            </div>
        </div>
    </div>
</div>
