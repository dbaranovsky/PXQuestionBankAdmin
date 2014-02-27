<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ProductMinimal.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.Course>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Enroll
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderAdditions" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">
<div id="PX_HOME_ZONE_2" class="zoneParent">
<div class="course-item">
<% var enrollmentId = ViewData["EnrollmentId"].ToString();

   if (string.IsNullOrEmpty(enrollmentId))
   {
    %>
    <div id="greeting">OK <%=ViewData["CurrentUser"].ToString() %></div>
    

    <h2 class="course-greeting-header">Below is the course you are set to enroll in.</h2>
    <div class="description">Confirm the details and click Continue.  If the course is wrong, click the correction link below and you will be allowed to change the course.</div>
    <% using (Html.BeginForm("EnrollmentConfirmation", "Home", new AjaxOptions() { }))
    {%>
    <table class="course-info">
        <tr>
            <td class="first-col">
                <%= Html.Label("UNIVERSITY:  ") %>
            </td>
             <td class="second-col">
                <%= Html.TextBox("University", ViewData["University"].ToString(), new { size = 50, disabled = "disabled", @class = "course_product-name" })%>
            </td>
        </tr>
        <tr>
            <td class="first-col">
                <%= Html.Label("ACADEMIC TERM:  ")%>
            </td>
            <td class="second-col">
                <%= Html.Label(ViewData["AcademicTerm"].ToString())%>
            </td>
        </tr>
        <tr>
            <td class="first-col">
                <%= Html.Label("INSTRUCTOR:  ")%>
            </td>
            <td class="second-col">
                <%= Html.Label(ViewData["Instructor"].ToString())%>
            </td>
        </tr>
        <tr>
            <td class="first-col">
                <%= Html.Label("COURSE TITLE:  ")%>
            </td>
             <td class="second-col">
                <%= Html.Label(Model.CourseProductName)%>
            </td>
        </tr>
        <tr>
           <td class="first-col">
                <%= Html.Label("COURSE NUMBER:  ")%>
            </td>
             <td class="second-col">
                <%= Html.Label(Model.CourseNumber)%>
            </td>
        </tr>
        <tr>
            <td class="first-col">
                <%= Html.Label("COURSE SECTION:  ")%>
            </td>
            <td class="second-col">
                <%= Html.Label(Model.SectionNumber)%>
            </td>
        </tr>
        <tr>
            <td class="first-col">
                <%= Html.Label("COURSE START DATE:  ")%>
            </td>
            <td class="second-col">
                <%= Html.Label(Model.ActivatedDate)%>
            </td>
        </tr>
    </table>

        <input type="submit" value="Continue" class="course-continue"/>
        <% var wronglink = Url.RouteUrl("EcomEnroll", new { courseid = Model.Id }, "http"); %>
        <span class="wrong-link"><a href="<%=wronglink %>" style="text-decoration:none;">This is not my course.  Join a different course.</a></span>

    <% }
   }
   else //already enrolled in this class
   { %>

   <div>You are already enrolled in this course.</div>

   <% 
    var port = Request.Url.Port; 
    var hostname = Request.Url.Host.ToString(); 
    var link = Url.RouteUrl("CourseSectionHome", new { courseid = Model.Id }, "http" );
    //remove port #
    link = link.Replace ( ":" + Request.Url.Port, "" ); 
%>
    <div style="padding-top:20px;"><a href="<%=link%>" style="text-decoration:none;"><button id="unblock-action">Take me to my course</button></a>  </div>


   <% } %>
</div>
</div>

<div id="PX_HOME_ZONE_3" class="zoneParent"></div>

<script language="javascript" type="text/javascript">
    $(function () {
        $('#banner-image').hide();
    });
</script>

</asp:Content>
