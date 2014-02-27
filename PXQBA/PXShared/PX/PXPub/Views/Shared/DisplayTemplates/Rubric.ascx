<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Rubric>" %>
<script type="text/javascript">
    PxPage.Require(['<%= Url.ContentCache("~/Scripts/Rubric/Rubric.js") %>'], function () {
        jQuery(document).ready(function() { PxRubric.Init(); });
    });
</script>
<%var hasParentLesson = ViewData["hasParentLesson"];%>
<div class="rubric-content-view px-default-text">
    <h2 class="content-title"><%= HttpUtility.HtmlDecode(Model.Title) %>
        <% if (!Model.ReadOnly)
           { %>
            <div class="menu edit-link">
                <%
                    var editUrl = Url.Action("DisplayItem", "ContentWidget",
                                        new
                                            {
                                                id = Model.Id,
                                                mode = ContentViewMode.Edit,
                                                isMultipartLessons = Model.IsMultipartLessons,
                                                hasParentLesson = hasParentLesson,
                                                includeNavigation = false,
                                                isBeingEdited = true
                                            });
                    %>
                <a href="<%=editUrl %>" class="linkButton nonmodal-link">Edit</a>
            </div>
        <% } %>
    </h2>
    <div class="html-container description-content"><%= Model.Description %></div>

    <% if (!Model.Rubric.ColumnTitles.IsNullOrEmpty() && !Model.Rubric.Rows.IsNullOrEmpty()){ %>
    <div class="rubric-statistics">The rubric currently has <%= Model.Rubric.ColumnTitles.Count %> column(s) and <%= Model.Rubric.Rows.Count %> row(s) and a maximum number of <%= Model.Rubric.MaxScore %> points.</div>
    <% } %>
    <div class="rubric-view-buttons">
        <%=Html.ActionLink("Open Rubric","EditRubric","Rubric",new{id=Model.Id},new {@class="fne-link linkButton"}) %>
    </div>
    <div class="rubric-associated-activities-container">
        <h3 class="sub-title">Associated Activities</h3>
        <%Html.RenderAction("AssociatedItems", "Rubric", new { rubric = Model}); %>
    </div>
</div>