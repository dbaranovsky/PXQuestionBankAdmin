<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IDictionary<string,string>>" %>
<div id="moreResourcesTitle-instructor-console">
    <span class="more-resources-dropdown-wrapper-console">
            <ul id="selResourceType-instructor-console">
                <li class="instructor-console-resource-link" value="unit" onclick="PxInstructorConsoleWidget.ViewResources('all-chapters')">Resources by chapter</li>
                <div class="chapters-result" style="padding-left:30px"></div>
                <li class="instructor-console-resource-link" value="type" onclick="PxInstructorConsoleWidget.ViewResources('all-types')"></span>Resources by type</li>
                <div class="types-result" style="padding-left:30px"></div>
                <% bool isInstructor = ((Bfw.PX.Biz.ServiceContracts.AccessLevel) ViewData["AccessLevel"]) ==
                         Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor;
                   if (isInstructor)
                   {%>
                <li class="instructor-console-resource-link" value="ebook" onclick="PxInstructorConsoleWidget.ViewResources('ebook');">eBook</li>
                <div class="ebook-result" style="padding-left:30px"></div>
                <li class="instructor-console-resource-link" value="mine" onclick="PxInstructorConsoleWidget.OpenBrowseMoreResources('myresources');">My Resources</li>
                <%--<li value="removed">Removed Items</li>--%>
               
                  <% }%>
                   <%
                        foreach (var feedName in Model.Keys)
                        {%>
                    <li class="instructor-console-resource-link rss" value='<%= Model[feedName] %>' onclick="PxInstructorConsoleWidget.OpenBrowseMoreResources('rss','<%= feedName %>')"><%= feedName %></li>  
                  <% }%>

<%--            <%Html.RenderAction("FacePlateBrowseResourcesUnits", "BrowseMoreResourcesWidget"); %>
            <%Html.RenderAction("FacePlateBrowseResourcesTypes", "BrowseMoreResourcesWidget"); %>--%>
    </span>
</div>
