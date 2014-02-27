<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IDictionary<string,string>>" %>
<div id="moreResourcesTitle-instructor-console">
    <span class="more-resources-dropdown-wrapper-console">
            <ul id="selResourceType-instructor-console">
                <li class="instructor-console-resource-link" value="unit" onclick="PxInstructorConsoleWidget.ViewResources('all-chapters')"><span class="sub-icon all-chapters"></span><span class="title">Resources by chapter</span></li>
                <div class="resources-sub-menu chapters-result" style="padding-left:30px"><%Html.RenderAction("LoadFacetValues", "InstructorConsoleWidget", new { type = "topic", fieldName1 = "meta-topic_dlap_e", fieldName2 = "meta-topics/meta-topic_dlap_e" }); %></div>
                <li class="instructor-console-resource-link" value="type" onclick="PxInstructorConsoleWidget.ViewResources('all-types')"><span class="sub-icon all-types"></span><span class="title">Resources by type</span></li>
                <div class="resources-sub-menu types-result" style="padding-left:30px"><%Html.RenderAction("LoadFacetValues", "InstructorConsoleWidget", new { type = "content-type", fieldName1 = "meta-content-type_dlap_e" }); %></div>
                <% bool isInstructor = ((Bfw.PX.Biz.ServiceContracts.AccessLevel) ViewData["AccessLevel"]) ==
                         Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor;
                   if (isInstructor)
                   {%>
                <!--li class="instructor-console-resource-link" value="ebook" onclick="PxInstructorConsoleWidget.ViewResources('ebook');"><span class="sub-icon ebook"></span><span class="title">eBook</span></li>
                <div class="resources-sub-menu ebook-result" style="padding-left:30px"><%Html.RenderAction("LoadFacetValues", "InstructorConsoleWidget", new { type = "ebook", fieldName1 = "meta-topic_dlap_e", fieldName2 = "meta-topics/meta-topic_dlap_e" }); %></div-->
                <li class="instructor-console-resource-link" value="mine" onclick="PxInstructorConsoleWidget.OpenBrowseMoreResources('myresources');">My Resources</li>
                <%--<li value="removed">Removed Items</li>--%>
               
                  <% }%>
                   <%
                        foreach (var feedName in Model.Keys)
                        {%>
                    <li class="instructor-console-resource-link rss" value='<%= Model[feedName] %>' onclick="PxInstructorConsoleWidget.OpenBrowseMoreResources('rss','<%= feedName %>')"><%= feedName %></li>  
                  <% }%>
                
            </ul>
    </span>
</div>
