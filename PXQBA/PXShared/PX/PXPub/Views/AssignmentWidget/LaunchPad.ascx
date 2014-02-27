<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.UpcommingActivitiesModel>" %>
        <%if (Model != null && Model.CountOfAssignments > 0)
          {%>
<div id="LaunchPadAssignmentsWidget" >
    <div id="leftLaunchPad">
        <div id="clockImage">
        <%--<img src="<%= Url.Content("~/Content/images/clock_launchpad2.png") %>" id="imgNextSub" alt="clock image" />--%>
        </div>

        <div class="assignment-due">You have <%= string.Format("{0} {1}", Model.DueInDays, Model.CountOfAssignments > 1 ? "assignments" : "assignment") %> due in the next 7 days.</b></div>
    </div>
        <div id="rightLaunchPad">
        <div class="link-row"><a href="#state/calendar/month" class="viewAllAssignments"><span class="pxicon pxicon-calendar"></span>Show Assignment Calendar</a></div>
    </div>

    <div id="view-all-items-launch-pad">
    
    </div>
</div>
<%}
          else
          {%>

          <div id="LaunchPadAssignmentsWidget">

    <div id="leftLaunchPad">

        <div id="clockImage">
        <%--<img src="<%= Url.Content("~/Content/images/clock_launchpad2.png") %>" id="imgNextSub" alt="clock image" />--%>
        </div>

        <div class="no-assignment">You have no assignments due in the next 7 days.</div>
    </div>
        <div id="rightLaunchPad">
        <div class="no-assignment-link"><a href="#state/calendar/month" class="viewAllAssignments"><span class="pxicon pxicon-calendar"></span>Show Assignment Calendar</a></div>
    </div>

    <div id="view-all-items-launch-pad">
    
    </div>
</div>

<%} %>
<script type="text/javascript">

       
        (function ($) {
            PxPage.OnProductLoaded(function () {
                var deps = ['<%= Url.ContentCache("~/Scripts/CurrentDueItemWidget/CurrentDueItemWidget.js")%>'];
                
                PxPage.Require(deps, function () {
                  $("#PX_HOME_FACEPLATE_ZONE_2").PxCurrentItemDue();
                });
            });
        } (jQuery));    
   </script>
  

