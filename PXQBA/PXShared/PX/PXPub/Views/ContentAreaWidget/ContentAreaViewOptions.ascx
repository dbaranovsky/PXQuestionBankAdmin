<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ContentArea>" %>
<!-- -->
<div id="contentarea-view-options">
<%  if (Model != null)
    {
        if (Model.Content.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
        {
            var previewUrl = Url.GetComponentHash("item", Model.Content.Id, new
            {
                mode = ContentViewMode.Preview,
                includeNavigation = true,
                isBeingEdited = false,
                renderFNE = true
            }); 
                    
            %>
            <span class="fne-action-btn">
                <a href="<%= previewUrl %>" id="fne-preview">
                    <span>Preview</span>
                    <span class="dropdown-icon"></span></a>
            </span>
            <div id="fne-edit-menu">
        <%  Html.RenderAction("XbookContentOptions", "ContentAreaWidget",
                new
                {
                    id = Model.Content.Id,
                    mode = ContentViewMode.Preview,
                    renderInFne = true
                }
            );
        %>  </div>
    <%  }
        else if (Model.Content.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
        {
            var defaultmode = Model.HeaderOptions.IsOptionAvailable(ContentViewMode.Edit) ? ContentViewMode.Edit : ContentViewMode.Assign;
            var url = Url.GetComponentHash("item", Model.Content.Id, new
            {
                mode = defaultmode,
                includeNavigation = true,
                isBeingEdited = defaultmode == ContentViewMode.Edit ? true : false,
                renderFNE = true
            }); 
                    
            %>
            <span class="fne-action-btn">
                <a href="<%= url %>" id="fne-edit">
                    <span><%=defaultmode.ToString() %></span>
                    <span class="dropdown-icon"></span></a>
            </span>
            <div id="fne-edit-menu">
        <%
            Html.RenderAction("XbookContentOptions", "ContentAreaWidget",
                new
                {
                    id = Model.Content.Id,
                    mode = defaultmode,
                    renderInFne = true
                }
            );
        %>  </div>
    <%  } 
    }%>
</div>