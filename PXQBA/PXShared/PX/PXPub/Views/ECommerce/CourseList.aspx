<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/ProductMinimal.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.ECommerceInfo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    CourseList
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CenterContent" runat="server">

<% 
    var courseId = ViewData["ProductCourseId"] != null ? ViewData["ProductCourseId"].ToString() : string.Empty;    
    var hasMultipleDomains = (bool)ViewData["userHasMultipleDomains"];
    var linkClass = "fne-link fixed";

    if (!hasMultipleDomains)
    {
        linkClass = "fixed";
    }
    
     %>
<div id="PX_HOME_ZONE_2" class="ecom">
<div class="ecom-access-wrapper"><h2>Hi, <%=Model.FirstName %> <%=Model.LastName %></h2>
    <div class="logout-wrapper">(<input id="logout" type="button" title="Log Out" value="Log Out" onclick="location.href='<%= Url.Action("LogOut", "Account") %>'" />)</div>


<div class="accessprompt"> You have access to the following <%= ViewData["ProductName"].ToString() %> courses.</div>
</div>

<div class="create-course-wrapper"> 
    <input id="createCourse" class="<%= linkClass %>" type="button" title="Create a Course" value="Create a Course" />
    <%= Html.ActionLink("Create a Course", "ShowCreateCourse", "Course", new { CourseId = courseId }, new { @class = linkClass, ID = "createCourseLink", style = "display: none" })%> 
    </div>
<ul class="ecom-courselist">

<% var courselist = (List<Course>)ViewData["CourseList"];

   if (courselist.Count() > 0)
   {
       foreach (var course in courselist)
       {
           var link = Url.RouteUrl("CourseSectionHome", new { courseid = course.Id }); 
%>        
        <li class="ecom-courselist-items">
            <span class="course-number"><%=course.CourseNumber %> <%=course.SectionNumber %></span>

            <a class="product-name" href="<%=link%>"><%=  course.Title%></a><br/>
            <span class="instructor-name">(<%=course.InstructorName %>)</span>
            
        </li>
<% 
        }
   }   
 
 %>

 </ul>
    
 </div>

<% Html.RenderAction("RenderFne","Course"); %>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {

            var deps = ['<%= Url.ContentCache("~/Scripts/Common/PxPage.LargeFNE.js") %>', '<%= Url.ContentCache("~/Scripts/ContentWidget/ContentWidget.js") %>', '<%= Url.ContentCache("~/Scripts/FacePlate/PxFacePlate.js") %>'];

            PxPage.Require(deps, function () {

                PxPage.LargeFNE.Init();

                $('#createCourse').bind('click', function () {
                    if ($("#createCourseLink").hasClass("fne-link")) {
                        $("#createCourseLink").click();
                    }
                    else {
                        window.location = $("#createCourseLink").attr("href");
                    }
                });
            });

        });
    } (jQuery));    
</script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderAdditions" runat="server">

</asp:Content>