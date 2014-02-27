<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<Bfw.PX.PXPub.Models.TocCategory>>" %>

<% if (!Model.IsNullOrEmpty())
   {
       var cats = Model.ToArray(); %>
    <% for (var i = 0; i < cats.Length; ++i )
       { %>
        <%= Html.Hidden(string.Format("Categories[{0}].Id", i), cats[i].Id) %>
        <%= Html.Hidden(string.Format("Categories[{0}].Text", i), cats[i].Text) %>
        <%= Html.Hidden(string.Format("Categories[{0}].ItemParentId", i), cats[i].ItemParentId) %>
        <%= Html.Hidden(string.Format("Categories[{0}].Sequence", i), cats[i].Sequence) %>
    <% } %>
<% } %>