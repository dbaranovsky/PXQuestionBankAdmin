<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DashBoardWidget>" %>

<% 
    String[] highLightedCourseIds = ViewData["HighLightIds"] as String[];
    highLightedCourseIds = highLightedCourseIds ?? new String[0];
%>

<div>
   <h1 class="dashboard-title">My Dashboard</h1>
    <input class="createeportfolio" type="button" onclick="return PxEportfolioDashboard.ShowCreateFromTemplate( '', '');" value="Create e-Portfolio" /> 

<script type="text/javascript" language="javascript">
    if ($('body .viewModal').length == 0) {
        $('body').append('<div class="viewModal"></div>');
    }
</script>

</div>

<div class="table-container">

<% if ( Model.CourseEportfolios.Count == 0 && Model.MyTemplates.Count == 0 ) {  %>
  <span class="nodata">You haven't created any ePortfiolio courses. To do so, use the "Create ePortfolio" button above.</span>
<%} %>

<%if (Model.CourseEportfolios.Count > 0) { %>
<h2>My Course e-Portfolios</h2>

<!-- Table goes in the document BODY -->
    <table class="dashboardgrid">
    <tr>
	    <th>Name</th><th>Status</th><th>Students</th><th>Shared With</th>
    </tr>
        <%foreach (var item in Model.CourseEportfolios) {

              var isShared = !item.SharedInstructors.IsEmpty();
              var sharedInstructors = isShared ? item.SharedInstructors : "Not Shared";
              var shareLinkText = isShared ? "Edit Sharing" : "Share";
              %>
        <tr id="<%=item.Id%>" class="<%=item.Status%>-row <%= highLightedCourseIds.Contains(item.Id) ? "fade-effect" : "" %>">
	        <td class="titleCell">
            <% var link = Url.RouteUrl("CourseSectionHome", new { courseid = item.URL });%>
    
            <a href="<%=link%>"><%=item.Name%></a>
   
            </td>
            <td class="statusCell"><%=item.Status%></td>
            <td class="enrollCell"><%=item.Count%>
             <span class="displayOnRowHover" style="display:none; float:right">
             <%if (item.Count == 0)
               { %>
                         <a href="#" onclick="return PxEportfolioDashboard.DeleteCourse('<%=item.Id%>');">Delete e-Portfolio</a>
                <% }%></span>
            </td>
            <td class="sharedwithCell"> 
                <span class="sharedinstructors"><%= sharedInstructors %></span> 
                <span class="manageshare" courseid="<%= item.URL %>"><%= shareLinkText %></span>
            </td>
        </tr>
        <%}%>
    </table>
    <%} %>


<%if (Model.MyTemplates.Count > 0)
  { %>

<h2>My e-Portfolio Templates</h2>

<table class="dashboardgrid myTemplates">
<tr>
	<th>Name</th><th>Derived e-Portfolios</th>
</tr>

<%foreach (var item in Model.MyTemplates)
  { %>
<tr id="<%=item.Id%>">
	<td class="titleCell"><a href="<%=item.URL%>"><%=item.Name%></a></td>
    <td class="enrollCell"><%=item.Count%>
     <span class="displayOnRowHover" style="display: none; float:right">
     <%if (item.Count == 0)
       { %>
                 <a href="#" onclick="return PxEportfolioDashboard.DeleteCourse('<%=item.Id%>');">Delete My e-Portfolio Template</a>
        <% }%></span>
    </td>
</tr>
<%}%>
</table>

<%} %>

<% if (Model.ProgramManagerTemplates.Count > 0)
   { %>
<br />
<br />
<div class="grid-title-wrapper"><h2>Program Manager e-Portfolio Templates</h2><span class="title-description">To customize program manager templates, copy to "My Templates"</span></div>
<br />

<table class="dashboardgrid">
<tr>
	<th>Name</th>
</tr>

<%foreach (var item in Model.ProgramManagerTemplates)
  { %>
<tr class="courseId" id="<%=item.Id%>">
	<td class="titleCell NoRightBorder">
            <div class= "name"><a href="<%=item.URL%>"><%=item.Name%></a></div>
            <div class= "showCopy"><a href="#">Copy to My Templates</a></div>
        
    </td>
</tr>
<%}%>
</table>
<%} %>

<% if (Model.PublisherTemplates.Count > 0)
   { %>
<br />
<br />
<div class="grid-title-wrapper"><h2>Publisher e-Portfolio Templates</h2><span class="title-description">To customize publisher templates, copy to "My Templates"</span></div>
<br />

<table class="dashboardgrid">
<tr>
	<th>Name</th>
</tr>

<%foreach (var item in Model.PublisherTemplates)
  { %>
<tr class="courseId" id="<%=item.Id%>">
	<td class="titleCell NoRightBorder">
            <div class= "name"><a href="<%=item.URL%>"><%=item.Name%></a></div>
            <div class= "showCopy"><a href="#">Copy to My Templates</a></div>
        
    </td>
</tr>
<%}%>
</table>
<%} %>
</div>

<% Html.RenderAction("SharedWithMeSummary", "DashBoardWidget"); %>


<div id="dialog-delete-confirm" style="display: none;" title="Delete e-Portfolio">
    <p>Are you sure you want to delete this e-Portfolio?</p>
    <br />
	<p><span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>You cannot undo this deletion.</p>    
</div>

<script type="text/javascript">
    PxPage.Require(['<%= Url.ContentCache("~/Scripts/EportfolioDashboard/EportfolioDashboard.js") %>', '<%= Url.ContentCache("~/Scripts/EportfolioShare/EportfolioShare.js") %>', '<%= Url.ContentCache("~/Scripts/jquery/jquery.fauxtree.js") %>', '<%= Url.ContentCache("~/Scripts/jquery/jquery.qtip.min.js") %>'], function () {
        PxEportfolioShare.BindTriggers();
        PxEportfolioDashboard.BindControls();
        $("#bannersearch").hide();
        $(".editpagebtnwrp").hide();
        PxPage.Fade();
    });
</script>
