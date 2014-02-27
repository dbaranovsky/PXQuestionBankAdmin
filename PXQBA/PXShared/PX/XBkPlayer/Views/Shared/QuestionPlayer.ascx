<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.XBkPlayer.Biz.DataContracts.XBkPlayerData>" %>
<% if (Model.Questions != null)
   {%>
    <% foreach (var question in Model.Questions)
       { %>
            <div class="clearer" />
            <%= Html.LabelFor(m => question.Text, "Q :") %> <%= string.IsNullOrEmpty(question.Text)?"":question.Text %>
            <div class="clearer" />
            <% if(question.Type=="essay") { %>
                <%= Html.TextAreaFor(m => question.ResponseText, new { @class = "html-editor", style = "width:auto;resize:none;width:70%" }) %>
            <%} 
            else {%>
                <%= Html.TextBoxFor(m => question.ResponseText, new { @class = "html-editor", style = "width:auto;resize:none;width:70%" }) %>
            <%} %>
            
            <div class="clearer" />
    <% } %>
<%} %>