<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ActivitiesWidget>" %>
<%  
    
    string checkedstring = "";

    if (Model.isSortable) {

        checkedstring = "checked='checked'";
    }
    %>
    <% if (Model.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
       { %>

<div id="studentDiv" style="padding:12px">
    <span>Students:</span> Click on any activity to get started
</div>
<%} %>
<div class="tablehead-border"></div>
    <div id="tableHeader">
        <span>LearningCurve activities</span>
        <div id="sortcontainer">
        <input type="checkbox" name="" id="sortCheckbox"  <%=checkedstring %>/><label for="sortCheckbox" id="sortLabel">Sort By Due Date</label>
        </div>
    </div>
     
<table class="activitiesWidgetBody <%= Model.UserAccess.ToString().ToLowerInvariant() %>" >

    <% if (!Model.GroupedActivities.IsNullOrEmpty())
       {


    %>

    
    <%if (Model.isSortable)
      { %>
        <%Html.RenderPartial("SortedByDueDate"); %>
    <%}
      else
      { %>
      <%Html.RenderPartial("Unsorted"); %>
    <%} %>
    

    <%} %>

</table>
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/ActivitiesWidget/ActivitiesWidget.js")%>', 
            '<%= Url.ContentCache("~/Scripts/ContentWidget/ContentWidget.js") %>',
             '<%= Url.ContentCache("~/Scripts/Highlight/highlights.js") %>',
             '<%= Url.ContentCache("~/Scripts/Highlight/autoHeight.js") %>'], function () {
                PxActivitiesWidget.Init();
                ContentWidget.Init();
            });
        });

    } (jQuery));    
                   
</script>