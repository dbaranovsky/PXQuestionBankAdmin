<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Link>" %>

<%
var settings = ViewData["settings"] == null ? new Hashtable() : (Hashtable)ViewData["settings"];
var isInstructor = (bool)settings["IsInstructor"];
var isProductCourse =  (bool)settings["IsProductCourse"];
var isAnonymous = (bool)settings["IsAnonymous"];
var link = Model;
var extendedLinkType = link.ExtendedLinkType.ToLowerInvariant();
var menuId = extendedLinkType + "_menu";

if (extendedLinkType.Contains("assignment") && !extendedLinkType.Contains("writting") && !isProductCourse)
{ %>
    <li id="<%=link.Id%>" class="fixedlink <%=link.GetVisibilityClasses(isProductCourse)%>">
        <%= Html.ActionLink(link.Title, "Index", "AssignmentCenter", null, new { id = menuId, color = "#660000" })%>
    </li>
<% }
else if (extendedLinkType.Contains("gradebook"))
{%>
    <% if (!isProductCourse && !isAnonymous)
        {
            if (isInstructor)
            { %>
                <li id="<%=link.Id%>" class="fixedlink <%=link.GetVisibilityClasses(isProductCourse)%>">
                    <%= Html.ActionLink(link.Title, "ManageGroups", "Groups",null, new {id = menuId})%>
                </li><%}
                else
                { %>
                    <li id="<%=link.Id%>" class="fixedlink <%=link.GetVisibilityClasses(isProductCourse)%>">
                        <%= Html.RouteLink(link.Title, "Scorecard", null, new { id = menuId, @class = "fne-link" })%>
                    </li>
                <% } %>
        <%} %>
<% }
else if (link.ExtendedLinkType == "writting_assignment")
{%>
   <%-- <%if (isStudent && !isAnonymous) { %>--%>
    <li id="<%=link.Id%>" class="fixedlink <%=link.GetVisibilityClasses(isProductCourse)%>">
        <%= Html.ActionLink(link.Title, "Index", "WritingTab",null,new{id = menuId })%>
    </li>
<% }
else if (extendedLinkType == "syllabus")
{%>
<li id="<%=link.Id%>" class="fixedlink <%=link.GetVisibilityClasses(isProductCourse)%>">
    <% Html.RenderAction("Index", "SyllabusTool");%>
</li>
<% }
else if (extendedLinkType == "mywriting")
{
    if (!isAnonymous)
    { %>
        <li id="<%=link.Id%>" class="fixedlink <%=link.GetVisibilityClasses(isProductCourse)%>">
            <%= Html.ActionLink(link.Title, "Index", "WritingTab", null, new{id = menuId })%>
        </li>    
    <%}
}%>
<%else{
     if (link.ExtendedLinkType != "popup_external"){%>
        <li id="<%=link.Id%>" class="<%=link.GetVisibilityClasses(isProductCourse)%>">
            <a href="<%=link.Url %>" id="<%=menuId %>"><%=link.Title%></a>
        </li>
    <%}
}%>