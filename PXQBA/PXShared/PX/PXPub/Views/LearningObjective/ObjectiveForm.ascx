<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<Bfw.PX.PXPub.Models.LearningObjective>>" %>

<div id="close-objective-link">
    <%= Html.ActionLink(" ", "Delete", null, new { @class = "close-link" })%>
</div>
<div style="clear: both;"></div>
<div id="add-objective-link">
    <%= Html.ActionLink("Add New...", "Add") %>
</div>
<div id="objective-box">
    <form id="objective-list-form" action="<%= Url.Action("Update") %>">
        <%: Html.Hidden("itemId", ViewData["itemId"]) %>

            <% if (Model.IsNullOrEmpty()) { %>
                    <div id="no-objective-message">
                        There are no correlated learning objectives
                     </div>
                    <div id="objective-list">
                    </div>
                    <div id="correlate-objective-button" style="visibility:hidden">
                        <input type="button" value="Update" />
                    </div>
            <% } else { %>
                       <div id="no-objective-message" style="visibility:hidden">
                           There are no correlated learning objectives
                       </div>
                       <div id="objective-list">
                           <% for (var i = 0; i < Model.Count; i++) { %>
                           <% var objId = "obj_" + Model[i].Guid; %>
                              <div id='<%=objId %>'>
                                  <%: Html.HiddenFor(x => x[i].Guid) %>
                                  <%: Html.CheckBoxFor(x => x[i].Checked) %>
                                  <%: Html.DisplayTextFor(x => x[i].Title) %>
                              </div>
                           <% } %>
                       </div>
                       <div id="correlate-objective-button">
                           <input type="button" value="Update" />
                       </div>
            <% } %>
    </form>
</div>

<div id="add-objective-dialog" title="Add Learning Objective">
<% Html.RenderAction("Add", "LearningObjective"); %>
</div>