<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.SubmissionGradeAction>" %>

<select id="ScoredAttempt" name="ScoredAttempt">
    <% foreach (SubmissionGradeAction value in Enum.GetValues(typeof(SubmissionGradeAction)))
       { %>
           <option value="<%= value %>"<% if (Model == value) { %> <%: Html.Raw("selected=\"selected\"") %><% } %>><%= value.ToString().Replace ( "_", " " ) %></option>
      <% } %>
</select>