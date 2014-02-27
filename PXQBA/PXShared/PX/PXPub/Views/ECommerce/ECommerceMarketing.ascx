<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ECommerceInfo>" %>
<div id="PX_HOME_ZONE_2" class="zoneParent">


<% if (Model.Authenticated)
   { %>
   <div id="ecom-greeting">
   <% if (!Model.IsEntitled)
      {
   %>
        <div class="ecom-not-entitled">
            <div class="ecom-not-entitled-message">
            Hi, <%= Model.FirstName %> <%= Model.LastName %><br/>
            You do not have access to this product.<br/>
            Students: Purchase access or register an access code.<br/>
            Instructors: Click Request Instructor Access and complete the registration form.<br/>
            </div>
        </div>
        <div id="ecom-marketing">
            
            <%=Model.MarketingInfo%>
        </div>
   <% }
      else
      {
          var lnkHtml = string.Empty;
          var linkStyles = "creation-button";

          if (Model.InMultipleDomains)
          {
              linkStyles = "creation-button fne-link fixed";
          }

          if (string.IsNullOrEmpty(Model.GettingStartedInfo))
          {
              var createButtonHtml = "<div class=\"homepageheader\"><div class=\"create-step1\"> </div> <div class=\"create-instructions\"><p>Welcome " + Model.FirstName + " " + Model.LastName + "</p><p>Your access to this product is approved.</p><p>Click here to get started.</p></div></div>";
              lnkHtml = Html.ActionLink("Create a Course", "ShowCreateCourse", "Course", null, new {@class = linkStyles, title = ""}).ToHtmlString();
              lnkHtml = lnkHtml.Replace("Create a Course", createButtonHtml);
          }
          else
          {
              var createUrl = Url.Action("ShowCreateCourse", "Course", null);
              lnkHtml = Model.GettingStartedInfo.Replace("[[createcourse]]", createUrl);
              lnkHtml = lnkHtml.Replace("[[createcoursestyles]]", linkStyles);

              var viewCoursesUrl = Url.RouteUrl("Dashboard", null);
              lnkHtml = lnkHtml.Replace("[[viewcourses]]", viewCoursesUrl);

              if (Model.EnrollmentCount > 0)
              {
                  lnkHtml = lnkHtml.Replace("view-course-button-hidden", "");
              }

              if (Model.AllowQuestionBankAdmin)
              {
                  //Show button QuestionBank
                  var viewQuestionBankUrl = Url.Action("Index", "QuestionAdmin", null);
                  lnkHtml = lnkHtml.Replace("[[questionbank]]", viewQuestionBankUrl);
                  lnkHtml = lnkHtml.Replace("view-questionbank-button-hidden", "");
              }


              if (Model.AllowEditSandboxCourse)
              {
                  var editSandboxCourseUrl = Url.Action("EditSandboxCourse", "Sandbox", null);
                  lnkHtml = lnkHtml.Replace("[[editsandbox]]", editSandboxCourseUrl);
                  lnkHtml = lnkHtml.Replace("view-editsandbox-button-hidden", "");
              }
          }
   %>

    <% var hasCourses = (bool) ViewData["has_derived_courses"]; %>

    <% if (hasCourses)
       {
           lnkHtml = lnkHtml.Replace("view-courses-button-hidden", "");
       } %>


        <%= lnkHtml %>
        
   <% } %>
</div>
<% }
   else
   {
%>
    <div id="ecom-marketing">
        <% if (!Model.IsProduct)
           { %>
           <div id="course-border">
                <!-- need to have the course/section - title from the derivative available here-->
                <span style="font-weight:bold;"><%= Model.CourseTitle%><br/><%=ViewData["course_info"].ToString()%></span><br />            
           </div>        
        <% }  %>
 
        <%=Model.MarketingInfo%>
   
    </div>
<% } %>
</div>
