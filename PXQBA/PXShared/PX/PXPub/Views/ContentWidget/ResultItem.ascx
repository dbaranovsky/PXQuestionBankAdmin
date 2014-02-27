<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EportfolioResultItem>" %>

<% var currentView = ViewData["CurrentView"] ?? "Multiple"; %>

<tr>
    <td class="erportfolio-result-item-title">
        
        <span style="margin-left:<%= (Model.Level*15)%>px;" class="<%= (Model.Type == "UploadOrCompose" || Model.Type == "ReflectionAssignment" || Model.Type == "HtmlDocument") ? "erportfolio-result-item-document" : "erportfolio-result-item-folder" %>">
            <div class="icon"></div>
            <%if (Model.Type == "ReflectionAssignment" && currentView=="Single"){ %><% = Model.AssignmentStatus == AssignmentStatus.Unsubmitted ? (Model.ComposedDate == null ? "Not Composed" : string.Format("Composed {0} (not yet submitted)", Model.ComposedDate.Value.ToString("MM/dd/yyyy"))) : string.Format("Submitted {0}", Model.SubmittedDate.Value.ToString("MM/dd/yyyy"))%><% } %>
            <%else if(Model.Type == "ReflectionAssignment") { %><% = string.Format("Reflection Assignment: {0} {1}", Model.Name, Model.AssignmentStatus == AssignmentStatus.Unsubmitted ? "(not submitted)" : "") %><% } %>
            <%else if (Model.AssignmentStatus != null) { %><% = string.Format("Folder Assignment: {0} {1}", Model.Name, Model.AssignmentStatus == AssignmentStatus.Unsubmitted ? "(not submitted)" : "") %><% } %>
            <%else { %><% = Model.Name %><% } %>
        </span>
    </td>
    <td class="erportfolio-result-item-comments"><span><%= Model.CommentCount %></span></td>
    <td class="erportfolio-result-item-rubric">
        <% if(Model.RawPointsAvailable == null) {%>
            <span>--</span>
        <% }else {%>
            <span><%= string.Format("{0} of {1}", Model.RawPointsScored??0,Model.RawPointsAvailable??0) %></span>
            <span class="erportfolio-result-item-rubric-percentile"><%= string.Format("({0}%)", ((Model.RawPointsScored ?? 0) * 100 / (Model.RawPointsAvailable ?? 0)).ToString("0.00"))%></span>
        <% }%>
    </td>
    <td class="erportfolio-result-item-grade"><span><%= Model.AssignmentStatus != AssignmentStatus.Graded ? "--" : (Model.Grade??0).ToString()%></span></td>
</tr>
<% foreach(var item in Model.Children) {
       Html.RenderPartial("ResultItem", item, new ViewDataDictionary() { { "CurrentView", currentView } });
} %>