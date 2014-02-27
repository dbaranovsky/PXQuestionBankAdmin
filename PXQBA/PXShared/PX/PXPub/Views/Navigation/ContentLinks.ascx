<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.Link>>" %>
<div id="<%=ViewData["parentName"] %>" class="content-link-widget">
<ul class="navigationItemList sortable">
    <% foreach (var link in Model)
       {
           var lastItem = link == Model.LastOrDefault() ? " navigation-last-item" : "";
           //Fix for PLATX-3989
           var settings = ViewData["settings"] == null ? new Hashtable() : (Hashtable)ViewData["settings"];
           var isProductCourse = ViewData["settings"] != null ? (bool)settings["IsProductCourse"] : false;
           var css = "";
           css = isProductCourse ? "" : link.GetVisibilityClasses(isProductCourse);
           
    %>
    
    <li id="<%=link.Id%>" class="navigationItemDisplay <%=lastItem %> <%=css %>">
        <a href="<%= link.Url %>" target="<%= (link.ExtendedLinkType == "popup_external") ? "_blank" : "_self" %>" title="<%= link.Title%>"> <%= (link.Title.Length >25)?link.Title.Substring(0,25):link.Title%></a>
    </li>
    <%}
    %>
</ul>
</div>