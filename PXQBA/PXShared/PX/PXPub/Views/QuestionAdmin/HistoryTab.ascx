<%@ Control Language="C#"   Inherits="System.Web.Mvc.ViewUserControl<QuestionLogs>" %>
<%@ Import Namespace="System.Diagnostics" %>
<div id="HistoryTab">    
    <%
        if (Model.LogList.Count > 0)
        {
    %>
    <%--<ul id="QBA-LogList">--%>
    <table id="QBA-LogList" class="action_table">
    <% 
            var displayText = "";
            foreach (var log in Model.LogList)
            {
                switch (log.Type.ToLower())
                {
                    case "added":
                        displayText = "added new question";
                        break;
                    case "modified":
                        displayText = " made changes ";//"modified the question";
                        break;
                    case "flagged":
                        displayText = "flagged the question";
                        break;
                    case "unflagged":
                        displayText = "removed the question flag";
                        break;
                    case "noteadded":
                        displayText = "added new note to the question";
                        break;
                    default:
                        displayText = "";
                        break;
                }
    %>   
        <%--<li>--%>
        
        <tr>
            <td colspan="3">
            <%= log.FirstName%> <%= log.LastName%> <%= displayText%> on <%= log.Created.ToString("MMM d, yyyy @ hh:mm tt")%>  <%--[ version No:  <%= log.Version %> ]--%>
            <input id="QuestionId" type="hidden" name = "<%= log.QuestionId %>" value='<%= log.Version %>' />
            </td>
        <%--</li>--%>   
        </tr>
       
                 <% Html.RenderPartial("HistoryTabDetails", log); %>

    <% } %>
        
    <%
        }
        else
        {
     %>
        <div class="norecord">No history record found for this question. </div>
     <%
        }
     %>
     </table>

</div>