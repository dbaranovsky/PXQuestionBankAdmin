<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.GroupSet>>" %>

<% if (!Model.IsNullOrEmpty())
   { %>
        <ul>
            <% foreach (var groupSet in Model)
               { %>
                    <li class="group-set">
                        <%= Ajax.ActionLink(groupSet.Name, "StudentList", new { groupSetId = groupSet.Id }, new AjaxOptions() { InsertionMode = InsertionMode.Replace, UpdateTargetId = "student-list" }, new { @class = "group-link" }) %></a>
                         <div class="setcount">(<%= groupSet.Groups.Count %> Groups, <%= groupSet.Groups.Sum(g => g.Members.Count()) %> Students)</div>
                        <div class="actions closed">
                            <%= Html.ActionLink("Clone", "EditGroupSet", new { Type = "clone", Id = groupSet.Id }, new { @class = "fne-link button small linkButton", title = "Clone Group Set" }) %>
                            <%= Html.ActionLink("Edit", "EditGroupSet", new { Type = "edit", Id = groupSet.Id }, new { @class = "fne-link button small linkButton", title = "Edit Group Set" })%>
                            <%= Ajax.ActionLink("Delete", "DeleteGroupSet", new { Id = groupSet.Id }, new AjaxOptions() { OnSuccess = "PxGroups.DeleteComplete", Confirm = "Delete this group?" }, new { @class = "deleteGroup button small linkButton" })%>
                        </div>
                    </li>
            <% } %>
        </ul>
<% }
else
   { %>
        <div class="no-group-sets-msg">No group sets have been created.</div>
<% } %>