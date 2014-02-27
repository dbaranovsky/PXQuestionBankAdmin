<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.PageDefinition>" %>

<script type="text/javascript" language="javascript">
    (function ($) {
        $(PxPage.switchboard).bind("PxPageReady", function () {

            $(".pageContainer").PageLayout({
                zoneClass: 'zone',
                zoneList: '#PX_HOME_FACEPLATE_START_ZONE_LEFT, #PX_HOME_FACEPLATE_START_ZONE_RIGHT',
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
            $("#PX_HOME_FACEPLATE_START_ZONE_TOP .widgetToolbar").remove();
            $("#PX_HOME_FACEPLATE_START_ZONE_TOP .addWidget").remove();
        });

        // capture the event, when page edit mode ends
        $(PxPage.switchboard).bind("StopPageEdit", function (event) {
            $(".pageContainer").PageLayout("disableEditing");
        });

    } (jQuery))
   
</script>
    
<style type="text/css">
    .closeWidgetButton
    {
        background-image: none;
        float: right;
        height: 25px;
        width: 60px;
        margin-top: 5px;
        font-weight: bold;
        color: #EB7C00;
        font-size: small;
        text-decoration: none;
    }
    .closeWidgetButton:before
    {
        content: 'Remove';
    }
</style>

<% 
    //accessLevel let us know whether it is a student or instructor
    string accessLevel = ViewData["AccessLevel"].ToString();
    var isAllowedToCreateCourse = (bool)ViewData["IsAllowedToCreateCourse"];
    %>

<div class="pageContainer startpagecontainer" >
<%var isSupportHide = false; %>

<div id="left" style="width:400px;display:none;"></div>
        <% if (this.Model == null || this.Model.Zones == null)
           {
               return;
           } %>

           <div class="OuterContainer">
            <div class="OuterZonesContainer">
             <% Html.RenderAction("StartPageCourseHeader", "Header"); %>
             
                <%foreach (Zone zone in this.Model.Zones.OrderByDescending(i => i.Id))
                 {%>
                    <% 
                     if (zone.Id == "PX_HOME_FACEPLATE_START_ZONE_TOP")
                     {                       
                        %> 
                        <div style="float:left; width:70%">
                        <%
                     }                            
                        if (zone.IsSupportHide) { isSupportHide = true; } %>
                      <div id="<%=zone.Id %>" itemId="<%=zone.Id %>" class="zoneParent <%=zone.IsSupportHide ? "zoneIsSupportHide" : "" %>">
                        
                            <div id="<%= zone.Id + "_Add_" %>" class="addWidget"><span class="prompt"> + Add new widget</span></div> 
                            <div id="<%= zone.Id + "_Choose_" %>" style="display: none;" class="chooseWidget">
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
                                        <%= liItems %>
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
                           
                            <div id="<%= zone.Id %>" itemId = "<%=zone.Id %>" class="zone" allowedWidgets="<%=zone.AllowedWidgetList %>">
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
                              
                        </div>                
                            <div class="zoneDebug"></div>
                      </div>

                      <div class="customWidgetModal">
                        <div class="placeHolderCustomwidget">
                        </div>
                      </div>
                      
                 <% 
                         if (zone.Id == "PX_HOME_FACEPLATE_START_ZONE_TOP") 
                         {
                %>
                    </div>
                    <div style="float:right; width:30%">

                    <div class="OuterButtonContainer">            
                        <%var formAction = Url.Action("FromStart", "Home"); %>
                        <form action="<%= formAction %>"  method="post" class="enter-form">
                            <div>
                                <a class="EnterCourse" href="#">Enter Course</a>
                            </div>
                        </form>
                    </div>
                        
                    </div>
                    <div style="clear:both"></div>
                <%
                         }                     
                     }
                 %>

              </div>
           
           <input type="hidden"  id="view_mode" value="launchpad" />

           </div>
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
      PxPage.Require([], function () {
          $("#PX_HOME_FACEPLATE_START_ZONE_TOP .widgetHeader").remove();
      });

      $(document).ready(function () {
          $('.EnterCourse').bind('click', function () {
              $('.enter-form').submit();
          });
      });

  </script>
