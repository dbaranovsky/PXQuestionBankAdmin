<%@ Control Language="C#"   Inherits="System.Web.Mvc.ViewUserControl<QuestionLog>" %>
<%@ Import Namespace="System.Diagnostics" %>
 <% if (Model.ParsedFields != null && Model.ParsedFields.Count() > 0)
           { %>
         <tr>
            <td class="Field_col headerCaption"><b>Field</b></td>
            <td class="original_col headerCaption"><b>Original Value</b></td>
            <td class="new_col headerCaption"><b>New Value</b></td>
        </tr>
                <% foreach (var logEntry in Model.ParsedFields)
                   { %>
                <tr>
                    <td class="Field_col"><%=logEntry.Field %></td>
                    <td class="original_col"><%=logEntry.OriginalValue %></td>
                    <td class="new_col"><%=logEntry.NewValue %></td>
                </tr>
                <%} %>
         <tr ><td colspan="3" >&nbsp;</td></tr>
         <tr ><td colspan="3" class="blank_row">&nbsp;</td></tr>
        <% } %>
        