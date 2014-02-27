<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.TreeWidgetRoot>" %>

<!-- Add button drop down -->
<div class="faceplate-nav">
    <div class="faceplate-add-content-menu">
        <ul>
            <li><a id="create" href="javascript:void(0);" class="lnkCreateContent">Create new</a></li>
            <li><a id="browse" href="javascript:void(0);">Browse existing resources...</a></li>
        </ul>
    </div>
    <% if (Model.Settings.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor && Model.Settings.AllowEditing)
       { %>
        <div id="create-assignment" class="xb-btn">Create Assignment
          <input type="hidden" class="assignmentUnitTemplateId" value="<%= Model.Settings.UnitTemplateId %>" />
          <input type="hidden" class="assignmentUnitToc" value="<%= Model.Settings.AssignmentTOC ?? Model.Settings.TOC %>" />
        </div>
        <div class="btn-wrapper gradient">
            <a id="add-assignment-btn">Add <span class="pxicon pxicon-down-open"></span></a>
        </div>
    <%} %>
    <input type="hidden" id="view_mode" value="launchpad" />
    <input type="hidden" id="title_link_hovered" value="false" />
    <input type="hidden" id="activeChapterId" value="PX_MULTIPART_LESSONS" />
    <input type="hidden" id="activeChapterName" value="LaunchPad" />
</div>
<div id="launchPadView" class="faceplate_launchpad <%= Model.Settings.ProductType%>">
    <%Html.RenderPartial("~/Views/LaunchpadTreeWidget/LaunchPad.ascx", Model); %>
</div>
<div class="toc-bottom"></div>
