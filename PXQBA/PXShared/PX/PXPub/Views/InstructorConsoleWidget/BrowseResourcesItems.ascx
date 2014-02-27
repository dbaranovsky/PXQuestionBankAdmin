<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.Biz.DataContracts.InstructorConsoleSettings>" %>
<div>
<% 
    var viewType = (ViewData["ViewType"] == null) ? "" : ViewData["ViewType"].ToString().ToLower();
    if (Model.ShowChapters)
    { %>
        <li class="link" value="unit" onclick="$.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', ['chapter'], '');"><a href="javascript:" class="link">Resources by chapter</a></li>
    <%}

    if (Model.ShowMyResources )
    { %>
        <li class="resources" value="mine" onclick="PxInstructorConsoleWidget.OpenBrowseMoreResources('myresources');"><a href="javascript:" class="link">My Resources</a></li>
    <%}
        if (Model.ShowEbook)
    { %>
        <li class="link" value="ebook" onclick="$.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', ['ebook']);"><a href="javascript:" class="link">eBook</a></li>
    <%}
      
    if (Model.ShowTypes)
    { %>
        <li class="link" value="typse" onclick="$.fn.FacePlateBrowseMoreResources('showMoreResourcesWindow', ['type'], '');"><a href="javascript:" class="link">Resources by type</a></li>
    <%}
        if (Model.Resources != null)
        {
            foreach (var resource in Model.Resources)
            {
                if (resource.Type.Equals("rss"))
                {
                    %>
                    <li class="link rss" onclick="PxInstructorConsoleWidget.OpenBrowseMoreResources('rss','<%= resource.Name %>')"><a href="javascript:" class="link"><%= resource.Name + " (rss)"%></a></li>
                    <%
                }
                else if (resource.Type.Equals("topic"))
                {
                    %>
                            <li class="link"><a href="javascript:" class="link" onclick="PxInstructorConsoleWidget.OpenBrowseMoreResources('chapter','<%= resource.Name %>');">
            <%=resource.Name.Replace("student_", "").Replace("instructor_", "")%></a></li>
                    <%
                }
                else if (resource.Type.Equals("content-type"))
                {
                    %>
                            <li class="link"><a href="javascript:" class="link" onclick="PxInstructorConsoleWidget.OpenBrowseMoreResources('type','<%= resource.Name %>');">
            <%=resource.Name.Replace("student_", "").Replace("instructor_", "")%></a></li>
                    <%
                }
                else if (resource.Type.Equals("ebook"))
                {
                    %>
                            <li class="link"><a href="javascript:" class="link" onclick="PxInstructorConsoleWidget.OpenBrowseMoreResources('ebook','<%= resource.Name %>');">
            <%=resource.Name.Replace("student_", "").Replace("instructor_", "")%></a></li>
                    <%
                }
            }
        }
      
      %>

</div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnProductLoaded(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/InstructorConsoleWidget/InstructorConsole.js") %>'], function () {
                PxInstructorConsoleWidget.Init();
            });
        });
    } (jQuery));    
</script>