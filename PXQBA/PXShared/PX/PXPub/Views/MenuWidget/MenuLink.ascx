<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.MenuItem>" %>
<% 
    var hurl = "#";
    var url = string.Empty;
    var disableClass = string.Empty;
    var useHash = Model.Properties.ContainsKey("bfw_usehash") ? Convert.ToBoolean(Model.Properties["bfw_usehash"].Value) : false;

    if (useHash)
    {
        var hashComponent = Model.Properties["bfw_hashcomponent"].Value.ToString();
        var hashFunction = Model.Properties.ContainsKey("bfw_hashfunction") ? Model.Properties["bfw_hashfunction"].Value.ToString() : string.Empty;

        hurl = Url.GetComponentHash(hashComponent, hashFunction);
    }

    if (!string.IsNullOrEmpty(Model.Url))
    {
        url = Model.Url;
    }
    else
    {
        RouteValueDictionary dict = Bfw.PX.PXPub.Models.MenuItem.createRouteValueDictionary(Model.Parameters, Model.Id);

        if (!Model.IsDisabled)
        {
            url = Url.Action(Model.Action, Model.Controller, dict);
        }

        if (Model.IsDisabled)
        {
            disableClass = "disabledmenu";
        }
    }
    
    var target = Model.Target;
    
%>

    <a href="<%=(useHash ? hurl : url) %>" ref="<%=url %>" rel="<%=target %>" class="menu-link <%= disableClass %>">
    
    <% if (Model.BfwCssClass.IsNullOrEmpty())
       { %>
        <%= Model.Title%>
    <% }
   else
   { %>
        <span class="<%= Model.BfwCssClass %>"><%= Model.Title %></span>
   <% } %>

   </a>

