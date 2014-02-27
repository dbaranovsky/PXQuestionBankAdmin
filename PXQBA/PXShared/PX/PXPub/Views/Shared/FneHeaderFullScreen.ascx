<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentView>" %>
<%@ Import Namespace="System.Activities.Expressions" %>
<%@ Import Namespace="Bfw.PX.Biz.ServiceContracts" %>

<%    
    if (Model != null && Model.Content != null && Model.Content.UserAccess == AccessLevel.Student)
    {
        ViewData["disableediting"] = "true";
    }
    //For right now, we want old xbook to just use the generic header.  In the future we might hook up the full preview features.
    if (Model == null || (Model.Content != null && Model.Content.CourseInfo.CourseType == CourseType.XBOOK))
    {
        Html.RenderPartial("FneHeaderGeneric");
    }
    else if (Model.Content == null || Model.Content.GetContainer(Model.Toc) == null ||
            (Model.Content.GetContainer(Model.Toc).ToLowerInvariant() != "launchpad" && //TODO: This is hacky - we should figure out a better way of identifying if an item is in a TOC for a course
             Model.Content.GetContainer(Model.Toc) != "ebook"))
    {
        Html.RenderPartial("FneHeaderFullNotInCourse");
    }
    else if (Model.ActiveMode == ContentViewMode.Preview)
    {
        Html.RenderPartial("FneHeaderFullPreview", Model);
    }
    else if (Model.ActiveMode == ContentViewMode.Results)
    {
        Html.RenderPartial("FneHeaderFullResults", Model);
    }
    else if (Model.ActiveMode == ContentViewMode.Grading)
    {
        // load the grading header
        // depending on type, px quizes, html quiz, and epage do not load the grading tool bar
        Html.RenderPartial("FneHeaderGeneric");
    }
    else if (Model.ContentIsBeingEdited())
    {
        Html.RenderPartial("FneHeaderFullEdit", Model);
    }
%>
    <% Html.RenderPartial("BackToBlackBoard"); %>
