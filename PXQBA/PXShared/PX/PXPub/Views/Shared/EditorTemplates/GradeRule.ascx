<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.GradeRule>" %>

<select id="GradeRule" name="GradeRule">
    <% foreach (GradeRule value in Enum.GetValues(typeof(GradeRule)))
       { %>
           <option value="<%= value %>"<% if (Model == value) { %> <%: Html.Raw("selected=\"selected\"") %><% } %>><%= value.ToString().Replace ( "_", " " ) %></option>
      <% } %>
</select>