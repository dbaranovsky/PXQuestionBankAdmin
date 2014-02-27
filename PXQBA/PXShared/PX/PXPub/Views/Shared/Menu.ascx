<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Menu>" %>

<%
    var IsSharedCourse = ViewData["IsSharedCourse"] != null ? Convert.ToBoolean(ViewData["IsSharedCourse"].ToString()) : false;
     %>

<div id="headermenu">
<%if (!IsSharedCourse)
    { %>
    <ul class="navigationItemList" id="<%=Model.Id%>">
        <%foreach (var menuItem in Model.MenuItems)
          { %>
            <%Html.RenderPartial("MenuItem", menuItem);%>
        <%} %>
        <li class="liAddMenuItem" style="display:none;float:left;" >  
            <a href="#" class="AddMenuItemLink">+ ADD TAB</a>
                <ul id="ulMenuItemAddList" class="contextMenu action_menu">
                    <li class="AddMenu AddMenuHeader" style="display:block;">Add a new tab:</li>
                <%foreach (var templateItem in Model.MenuItemTemplates)
                  {
                      if (templateItem.Id == "PX_MENU_ITEM_CUSTOM_TEMPLATE")
                          continue;
                      %>

                      <%var hide = Model.MenuItems.Exists(i => i.BfwMenuCreatedby == templateItem.Id) ? " display:none;" : "";%>
                        <li class="AddMenu" style="display: block;<%=hide%>" id='liMenuItem_<%=templateItem.Id%>'><a href="#" class="menuItem" onclick="PxMenuConfig.AddTemplateMenuItem('<%=templateItem.Id%>');"><%=templateItem.Title%></a>
                            <%=Html.Hidden("VisibleByInstructor", templateItem.VisibleByInstructor, new { id = "VisibleByInstructor" })%>    
                            <%=Html.Hidden("VisibleByStudent", templateItem.VisibleByStudent, new { id = "VisibleByStudent" })%>  
                        </li>
                <% } %>
                                
                    <li style="display: block;" class="AddMenu AddMenuTab"><a href="#" onclick="return PxMenuConfig.ShowCustomTabDialog();">Add custom tab...</a></li>
                </ul>
         </li>
    </ul>
    <% } %>
</div>




