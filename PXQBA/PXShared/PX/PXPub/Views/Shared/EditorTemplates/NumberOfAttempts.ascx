<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.NumberOfAttempts>" %>
<select name="NumberOfAttempts.Attempts">
    <% if (Model != null)
       {
           for (int i = -1; i <= 10; i++)
           {
               var isSelected = Model.Attempts == i;
               var showThisOption = i > -1 || isSelected;
               
               if (showThisOption)
               {
                   var text = (i == 0) ? "Unlimited" : (i == -1) ? "" : i.ToString();
                %>
                    <option value="<%= i %>" <% if(isSelected) { %>selected="selected"<% } %> ><%= text %></option>
                <% 
               }
           }
       } %>
</select>

&nbsp;Attempts