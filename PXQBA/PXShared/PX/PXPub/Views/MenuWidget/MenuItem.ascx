<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.MenuItem>" %>
<%           
    string parentId = string.Empty;
    bool renderChildren = false;
    
    string level = "1";
    string activeClass = "";
    //string disableClass = string.Empty;

    if (Model.ParentId != null)
    {
        parentId = Model.ParentId;
    }

    if (Model.MenuItems != null && Model.MenuItems.Count > 0)
    {
        renderChildren = true;
    }

    if (Model.IsActive)
    {
        activeClass = "active";
    }

    //if (Model.IsDisabled)
    //{
    //    disableClass = "disabledmenu";
    //}
    
%>
<li class="level-<%=level%> menu-item <%=activeClass %> <%--<%= disableClass %>--%>" mw-index='<%=Model.MenuSequence %>' id="<%= Model.Id %>" >
    
    <%
        Html.RenderPartial("~/Views/MenuWidget/MenuLink.ascx", Model);
            if (renderChildren)
            {
    %>
    <ul class="child">
        <%
            foreach (var child in Model.MenuItems)
            {
                if (child.IsHidden != true)
                {
                    Html.RenderPartial("~/Views/MenuWidget/MenuItem.ascx", child);
                }
            }
        %>
    </ul>
    <%
        }
     
    %>
</li>
