<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.Biz.DataContracts.InstructorConsoleSettings>" %>
<div id="moreResourcesTitle-instructor-console">
    <span class="more-resources-dropdown-wrapper-console">
        <ul id="selResourceType-instructor-console">
            <%
                var showChapters = Model.ShowChapters ? "checked='checked'" : "";
            %>
            <li class="instructor-console-resource-link edit">
                <%= Html.CheckBoxFor(m => m.ShowChapters) %><span value="unit" onclick="PxInstructorConsoleWidget.ViewResources('all-chapters', 'true')"><span class="sub-icon all-chapters"></span><span class="title">Resources
                    by chapter</span></span></input></li>
            <div class="resources-sub-menu chapters-result" style="padding-left: 30px">
                <%Html.RenderAction("LoadFacetValues", "InstructorConsoleWidget", new { type = "topic", fieldName1 = "meta-topic_dlap_e", fieldName2 = "meta-topics/meta-topic_dlap_e", instructorConsoleSettings = Model, edit = true, flag = "topic" }); %></div>
            <li class="instructor-console-resource-link edit">
                <%= Html.CheckBoxFor(m => m.ShowTypes) %><span value="type" onclick="PxInstructorConsoleWidget.ViewResources('all-types', 'true')"><span class="sub-icon all-types"></span><span class="title">Resources
                    by type</span></span></input></li>
            <div class="resources-sub-menu types-result" style="padding-left: 30px">
                <%Html.RenderAction("LoadFacetValues", "InstructorConsoleWidget", new { type = "content-type", fieldName1 = "meta-content-type_dlap_e", instructorConsoleSettings = Model, edit = true, flag = "content-type" }); %></div>
            <% bool isInstructor = ((Bfw.PX.Biz.ServiceContracts.AccessLevel)ViewData["AccessLevel"]) ==
                         Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor;
               if (isInstructor)
               {%>
            <!--li value="ebook">
                <%= Html.CheckBoxFor(m => m.ShowEbook) %><span onclick="PxInstructorConsoleWidget.ViewResources('ebook', 'true');"
                    class="instructor-console-resource-link edit"><span class="sub-icon ebook"></span><span class="title">eBook</span></span></input></li-->
            <div class="resources-sub-menu ebook-result" style="padding-left: 30px">
                <%Html.RenderAction("LoadFacetValues", "InstructorConsoleWidget", new { type = "ebook", fieldName1 = "meta-topic_dlap_e", fieldName2 = "meta-topics/meta-topic_dlap_e", instructorConsoleSettings = Model, edit = true, flag = "ebook" }); %></div>
            <li class="instructor-console-resource-link" value="mine">
                <%= Html.CheckBoxFor(m => m.ShowMyResources) %>
                <span class="instructor-console-resource-link">My Resources</span></input></li>
            <%--<li value="removed">Removed Items</li>--%>
            <% }%>
            <%
                foreach (var feedName in Model.FacetsToShow.Keys)
                {
                    var checkedBox = "";
                    if (Model.Resources != null)
                    {
                        foreach (var res in Model.Resources)
                        {
                            if (res.Type == "rss" && res.Name.Equals(feedName))
                            {
                                checkedBox = "checked='checked'";
                            }
                        }
                    }
            %>
            <li class="instructor-console-resource-link rss" value='<%= Model.FacetsToShow[feedName] %>'>
                <input name="rss" type="checkbox" id="rss" value='<%="|"+feedName+"|" %>' <%=checkedBox %>><span
                    class="instructor-console-resource-link"><%= feedName %></span></input></li>
            <% }%>
            <%--            <%Html.RenderAction("FacePlateBrowseResourcesUnits", "BrowseMoreResourcesWidget"); %>
            <%Html.RenderAction("FacePlateBrowseResourcesTypes", "BrowseMoreResourcesWidget"); %>--%>
    </span>
</div>
