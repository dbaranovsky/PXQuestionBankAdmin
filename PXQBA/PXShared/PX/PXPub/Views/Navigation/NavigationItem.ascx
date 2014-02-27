<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.NavigationItem>" %>

<%
var linkUrl = Url.RouteUrl("FeaturedContentItem", new { id = Model.TocId });
var settings = ViewData["settings"] == null ? new Hashtable() : (Hashtable)ViewData["settings"];
var isProductCourse = (bool)settings["IsProductCourse"];

var css = isProductCourse ? "" : Model.GetVisibilityClasses(isProductCourse); 
    
    
    
 if (Model.IsActive) { %>
 <%if(Model.Id == "PX_LOCATION_ZONE1_MENU0_CONTENT"){
    css += " fixedlink";
 } %>
<li id="<%=Model.Id%>" class="<%=css%> new">
    <%
     var menuId = string.IsNullOrEmpty(Model.ExtendedLinkType) ? "content_menu" : Model.ExtendedLinkType + "_menu";
    %>
    <a href="<%= linkUrl %>" id="<%=menuId %>"><%= Model.Title%></a>
</li>
<%} %>
<%foreach (var navItem in Model.Children)
  {
      if (navItem.ParentId != "PX_LOCATION_ZONE1_MENU_1")
      {
      %>
    <%if (navItem is Link)
      {
          var lnk = (Link)navItem;

          %>
         <%Html.RenderPartial("NavTemplateLink", lnk);%>
    <%}
      else
      { %>
        <%Html.RenderPartial("NavigationItem", navItem);%>
    <%} %>
<%}
  }%>