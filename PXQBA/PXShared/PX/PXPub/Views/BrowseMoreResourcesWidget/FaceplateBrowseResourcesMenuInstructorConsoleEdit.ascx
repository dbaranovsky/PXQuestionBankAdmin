<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<div id="moreResourcesTitle-instructor-console">
    <span class="more-resources-dropdown-wrapper-console">
            <ul id="selResourceType-instructor-console">
                <%
                    var showChapters = Model.ConsoleSettings.ShowChapters ? "checked='checked'" : "";
                     %>
                <li class="instructor-console-resource-link" ><%= Html.CheckBoxFor(m => m.ConsoleSettings.ShowChapters) %><span value="unit" onclick="PxInstructorConsoleWidget.ViewResources('all-chapters', 'true')">Resources by chapter</span></input></li>
                <div class="chapters-result" style="padding-left:30px"></div>
                <li class="instructor-console-resource-link" ><%= Html.CheckBoxFor(m => m.ConsoleSettings.ShowTypes) %><span value="type" onclick="PxInstructorConsoleWidget.ViewResources('all-types', 'true')">Resources by type</span></input></li>
                <div class="types-result" style="padding-left:30px"></div>
                <% bool isInstructor = ((Bfw.PX.Biz.ServiceContracts.AccessLevel) ViewData["AccessLevel"]) ==
                         Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor;
                   if (isInstructor)
                   {%>
                <li value="ebook"><%= Html.CheckBoxFor(m => m.ConsoleSettings.ShowEbook) %><span onclick="PxInstructorConsoleWidget.ViewResources('ebook', 'true');" class="instructor-console-resource-link" >eBook</span></input></li>
                <div class="ebook-result" style="padding-left:30px"></div>
                <li class="instructor-console-resource-link" value="mine"> <%= Html.CheckBoxFor(m => m.ConsoleSettings.ShowMyResources) %><span  class="instructor-console-resource-link" >My Resources</span></input></li>
                <%--<li value="removed">Removed Items</li>--%>
               
                  <% }%>
                   <%
                        foreach (var feedName in Model.ConsoleSettings.FacetsToShow.Keys)
                        {%>
                    <li class="instructor-console-resource-link rss" value='<%= Model.ConsoleSettings.FacetsToShow[feedName] %>' ><input type="checkbox" id="rss" value='<%= Model.ConsoleSettings.FacetsToShow[feedName] %>'><span class="instructor-console-resource-link" ><%= feedName %></span></input></li>  
                  <% }%>

<%--            <%Html.RenderAction("FacePlateBrowseResourcesUnits", "BrowseMoreResourcesWidget"); %>
            <%Html.RenderAction("FacePlateBrowseResourcesTypes", "BrowseMoreResourcesWidget"); %>--%>
    </span>
</div>
