<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.PageDefinition>" %>

<script type="text/javascript" language="javascript">
    (function ($) {
        $(PxPage.switchboard).bind("PxPageReady", function () {

            $(".pageContainer").PageLayout({
                zoneClass: 'zone',
                widgetClass: 'widgetItem',
                addWidgetClass: 'addWidget',
                chooseWidgetClass: 'chooseWidget',
                widgetDisplayItemClass: 'widgetDisplayItem',
                removeWidgetClass: "closeWidgetButton",
                widgetToolbarClass: "widgetToolbar",
                toggleDisplayClass: 'widgetToggleDisplay',
                widgetBodyClass: 'widgetBody',
                widgetCollapseClass: 'widgetCollapse',
                widgetExpandClass: 'widgetExpand',
                pageName: '<%=Model.Name %>',
                ajaxUrlAddWidget: PxPage.Routes.addWidget,
                ajaxUrlEditWidget: PxPage.Routes.editWidget,
                ajaxUrlRemoveWidget: PxPage.Routes.removeWidget,
                ajaxUrlMoveWidget: PxPage.Routes.moveWidget,
                ajaxUrlGetWidgetTemplate: PxPage.Routes.getWidgetTemplate,
                ajaxUrlSetWidgetDisplay: PxPage.Routes.setWidgetDisplay,
                ajaxUrlSaveCustomWidget: PxPage.Routes.saveCustomWidget,
                ajaxUrlReloadCustomWidget: PxPage.Routes.reloadCustomWidget,
                ajaxUrlRenameCourse: PxPage.Routes.renameCourse
            });

            $(".widgetItem").show();
        });

        // capture the event, when page gets in the edit mode
        $(PxPage.switchboard).bind("StartPageEdit", function (event) {
            $(".pageContainer").PageLayout("enableEditing");
        });

        // capture the event, when page edit mode ends
        $(PxPage.switchboard).bind("StopPageEdit", function (event) {
            $(".pageContainer").PageLayout("disableEditing");
        });

        // capture the event, in case of rename course
        $(PxPage.switchboard).bind("renameCourse", function (event, courseName) {
            $(".pageContainer").PageLayout("renameCourse", courseName);
        });

    } (jQuery))
   
</script>

<% 
    //accessLevel let us know whether it is a student or instructor
    string accessLevel = ViewData["AccessLevel"].ToString();
    var isAllowedToCreateCourse = ViewData["IsAllowedToCreateCourse"]!=null?(bool)ViewData["IsAllowedToCreateCourse"]:false;
    %>

<div class="pageContainer" >
    
    <%var isSupportHide = false; %>
    <div id="left" style="width:400px;display:none;"></div>
    <% if (this.Model == null || this.Model.Zones == null)
       {
           return;
       } %>

    <%foreach (Zone zone in this.Model.Zones)
      {%>
                
        <% if (zone.IsSupportHide) { isSupportHide = true; } %>
        <div id="<%=zone.Id %>" class="zoneParent <%=zone.IsSupportHide ? "zoneIsSupportHide" : "" %>">   
            <div id="<%= zone.Id + "_Add_" %>" class="addWidget"><span class="prompt"> + Add new widget</span></div>
            <div id="<%= zone.Id + "_Choose_" %>" style="display: none;" class="chooseWidget">
                <div class="addWidgetDisplay">
                    Add New Widget</div>
                <div style="padding-left: 5%; padding-right: 5%">
                    <ul id="topnav">
                        <%
           string liItems = string.Empty;
           foreach (var allowedWidget in zone.AllowedWidgets)
           {
               if (allowedWidget.widgetType != "PX_Custom")
               {
                   liItems = liItems + string.Format("<li class='widgetDisplayItem' widgetType='{0}'><a href='#' >{1}</a></li>", allowedWidget.widgetType, allowedWidget.widgetName);
               }
           }                                                 
                        %>
                        <%= liItems%>
                    </ul>
                </div>
                <%
           foreach (var allowedWidget in zone.AllowedWidgets)
           {
               if (allowedWidget.widgetType == "PX_Custom")
               {                            
                %>
                        <div class="createYourWidget" widgetType="<%= allowedWidget.widgetType%>"><a href="#"><%= allowedWidget.widgetName%></a></div>
                <%      }
           } %>
            </div>        
                          
            <div itemId = "<%=zone.Id %>" class="zone" allowedWidgets="<%=zone.AllowedWidgetList %>">
                <% foreach (var widget in zone.Widgets)
                   {
                       if (widget.WidgetDisplayOptions.DisplayOptions.Exists(d => d.ToString().ToLowerInvariant() == accessLevel))
                       {
                           if (isAllowedToCreateCourse == false)
                           {
                               Html.RenderPartial("WidgetWrapper", widget);
                           }
                       }

                   }                 
                %>
                <% if (zone.DefaultPage != null)
                   {
                       Html.RenderPartial("PageContainer", zone.DefaultPage);
                   } %> 
            </div>                
            <div class="zoneDebug"></div>
        </div>

        <div class="customWidgetModal">
            <div class="placeHolderCustomwidget">
            </div>
        </div>
<%  }
    if (Model.CustomDivs != null)
    {
        foreach (var customDiv in Model.CustomDivs)
        {%>
            <div class="<%=customDiv %>"></div>
    <%  }
    }%> 
</div>
     <%if (isSupportHide && accessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor.ToString().ToLowerInvariant())
                    { %>
                  <script type="text/javascript">
                      PxPage.Require([], function () {
                          PxPage.InjectZoneHide();                        
                         
                      });
                  </script>
  <%} %>

<script type="text/javascript">
    var deps = [];
    PxPage.Require(deps, function () {
        if (PxPage) {
            PxPage.OnReady(function() {
                PxPage.SetActivationLinks();
            });
        }
    });
</script>