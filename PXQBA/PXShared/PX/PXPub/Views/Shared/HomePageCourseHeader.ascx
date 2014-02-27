<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.CourseHeader>" %>
<!--[if gte IE 9]>
  <style type="text/css">
    .gradient {
       filter: none !important;}
  </style>
<![endif]-->
<% var IsSharedCourse = ViewData["IsSharedCourse"] != null ? Convert.ToBoolean(ViewData["IsSharedCourse"].ToString()) : false;
   var showCourseLabel = Model.CourseType == Bfw.PX.Biz.DataContracts.CourseType.XBOOK.ToString() ? true : false;
   var instructorName = Model.DisplayedInstructorName;
%>

 <script type="text/javascript">
      jQuery(document).ready(function () {

         <% if ( Model.IsAllowPageEdit ) { %>
         $(".editPageStart").bind("click", function () {
             $(this).hide();
             $(".editPageEnd").show();
             $(".course-title-rename").show();
             $(".course-title").css("padding-top", "10px");
             $(".course-title").show();
             $(".tabbed-course").css("top", "148px");
             $("#main").css("top", "157px");
             $("#main").addClass("editing-homepage");
             $(PxPage.switchboard).trigger("StartPageEdit");
         });


         $(".course-title-rename-anchor").click(function () {
             $(".course-title").hide();
             $(".course-title-save").css("padding-top", "5px");
             $(".course-title-save").show();
             $(".course-title-text-value").focus();
         });

         $(".course-title-text-value").focus(function() { 
            $(this).select(); 
         }); 

         $(".doneRenameCourse").click(function () {
             var courseName = $.trim($(".course-title-text-value").val());
             var characterReg = /^\s*[a-zA-Z0-9_\s-:;$\""\'']+\s*$/;

             if($.trim($(".course-title-text-value").val()) == ""){
                $(".errorTextCourseTitle").text("Please provide course name");
                $(".errorTextCourseTitle").show();
                $(".course-title-text-value").focus();
             }
             else if(!characterReg.test(courseName)) {
                $(".errorTextCourseTitle").text("This field can only accept alphanumeric and -:;'\"$ characters");
                $(".errorTextCourseTitle").show();
                $(".course-title-text-value").focus();
             }
             else {
                 $(".course-title-save").hide();
                 $(".errorTextCourseTitle").hide();
                 $(".course-title").show();
                 $(".course-title").css("padding-top", "20px");
                 $(PxPage.switchboard).trigger("renameCourse", courseName);
             }
         });

         $(".cancelRenameCourse").click(function () {
             $(".course-title-save").hide();
             $(".errorTextCourseTitle").hide();
             $(".course-title").css("padding-top", "10px");
             $(".course-title").show();
             $(".course-title-text-value").val($.trim($(".course-title-lable").text()));
         });
         

         $(".doneEditing").click(function () {
        
             $(".editPageEnd").hide();
             $(".editPageStart").show();
             $(".course-title-save").hide();
             $(".course-title").show();
             $(".course-title-rename").hide();
             $(".course-title").css("padding-top", "10px");
             $(".tabbed-course").css("top", "74px");
             $(".course-title-text-value").val($.trim($(".course-title-lable").text()));
             $(".errorTextCourseTitle").hide();
             $("#main").css("top", "119px");
              $("#main").removeClass("editing-homepage");
             $(PxPage.switchboard).trigger("StopPageEdit");
         });

          $(".editPageStart").show();

        <%} else { %>
            $(".editPageStart").hide();
            $(".doneEditing").hide();
        <%} %>

        <% if(IsSharedCourse) { %>
            //PxEportfolioShare.BindTriggers();
            
            //$(".sharecourse-notes-anchor").click(function() {
            //var sharednotes = $('.sharednotes').html();
            //$(PxPage.switchboard).trigger("eportfoliosharenotes", [sharednotes]);
            //PxEportfolioShare.ShowShareNotesModal(sharednotes);
            //});
        <% } %>



     });
</script>
    
<div class="editPageEnd" style="display:none; width:100%; text-align:center;">
    Click on any widget or navigation item you'd like to edit; drag to repostition. 
    <input type="button" class="doneEditing" value="Done editing" />    
</div>

<% var sharedeportfoliostyle = IsSharedCourse ? "sharedeportfoliocourse" : string.Empty; %>


<div class="homepage-course-info <%= sharedeportfoliostyle %>">
    <%  
    if (Model.IsAllowPageEdit)
    { %>
        <div class="editpagebtnwrp"> <a class="editPageStart" href="#"></a></div>
    <%  
    } 
    %>

    <%--the following code needs to be readdressed according to http://jira.bfwpub.com/browse/PXDESIGN-172 --%>

    <div class="course-banner-logo"></div>
    <div class="product-banner-logo"></div>
    <%  
    if (Model.CourseType == Bfw.PX.Biz.DataContracts.CourseType.PersonalEportfolioPresentation.ToString())
    {   
        Html.RenderPartial("~/Views/PresentationCourse/PresentationActionsBar.ascx", Model); 
    }
    else if (Model.CourseType == Bfw.PX.Biz.DataContracts.CourseType.PersonalEportfolioDashboard.ToString() ||
                Model.CourseType == Bfw.PX.Biz.DataContracts.CourseType.EportfolioDashboard.ToString())
    { 
    %>
        <div class="course-title">
        <div class="course-title-lable">
            My Dashboard
        </div>
            <div class="instructor-name"><a href="mailto:<%= Model.InstructorEmail%>">Owner: <%= instructorName %></a></div>
            <%-- <div class="course-author-info"><i>Universe</i> : Freedman, Geller, Kaufmann</div>--%>
        </div>
    <%
    }
    else
    {    
    %>        
        <%--<div class="course-group">
            <h1><%= Model.CourseDescription %></h1>
            <h2><%= Model.CourseAuthor %></h2>
        </div>--%>
        <div class="course-title">
        <% if (showCourseLabel)
           { %>
        <div class="course-title-lable-label">Course: </div>
        <% } %>
         <% if (Model.CourseDisciplineAbbreviation.IsNullOrEmpty())
               { %>
            <div class="course-title-lable">
                <%= Model.CourseTitle %> 
            </div>
            <% }
               else
               { %>
               <div class="course-title-lable-text">
                   <span><%=Model.CourseDisciplineAbbreviation%></span><%--<span style='font-weight:bold;'>Portal</span>--%>
               </div>
            <% } %>
        <div class="course-title-rename">
            <a class="course-title-rename-anchor">(Rename)</a> 
        </div> 
    <% if (Model.CourseType == Bfw.PX.Biz.DataContracts.CourseType.FACEPLATE.ToString())
    { %>
            <div class="course-description"><%= Model.CourseDescription%></div>
            <div class="course-author">Instructor: <%= instructorName %></div>
    <%}
      else if (Model.CourseType == Bfw.PX.Biz.DataContracts.CourseType.XBOOK.ToString())
        {       
    %> 
            <div class="course-author-label">Instructor: </div><div class="course-author"><%= instructorName %></div>
    <% 
        }
       else if (IsSharedCourse)
       {  %>
        <div class="sharedcourse-notes">
            <span> Shared by: <%= instructorName %> |</span>
            <a class="sharecourse-notes-anchor">Notes</a>
        </div>
    <% } 
       else if(!string.IsNullOrEmpty(Model.CourseSubType) && Model.CourseSubType.Equals("generic", StringComparison.OrdinalIgnoreCase))
       { %>
            <div class="instructor-name"> Instructor: None </div>
       <% }
       else if (!Model.InstructorEmail.IsNullOrEmpty())
       { %>
          <div class="instructor-name"><a href="mailto:<%= Model.InstructorEmail%>">Instructor: <%= instructorName %></a></div>
    <%} %>
    <%-- <div class="course-author-info"><i>Universe</i> : Freedman, Geller, Kaufmann</div>--%>
    </div>
    <%
      }
    %>
    <%--end of http://jira.bfwpub.com/browse/PXDESIGN-172 --%>
    <div class="course-title-save">
        <div class="course-title-text" >
            <input type="text" maxlength = "50" size = "50" value = "<%= Model.CourseTitle%>" class= "course-title-text-value"/>
            <div class="errorTextCourseTitle"></div>
        </div>
        <div class="course-title-save-buttons">
            <input type="button" class="doneRenameCourse" value="Save" /> |
            <a class="cancelRenameCourse">Cancel</a> 
        </div>
    </div>
     
    <%ViewData["CourseType"] = Model.CourseType; %>
    <% Html.RenderPartial("CourseTimeZoneData"); %>
    <%= Html.HiddenFor(m => m.CourseId) %>
     <div id="brandbanner" class="home-banner">
       <% Html.RenderPartial("BackToBlackBoard"); %>
         <%// To address the fix for right-banner not showing up in LearningCurve product only%>
        <div id="rightbanner"><%
                                  if (Model.CourseType == CourseType.LearningCurve.ToString() || Model.IsProductCourse==false) { Html.RenderPartial("BaseHeader"); } 
                                  %></div>
     </div>
    
    <% if (IsSharedCourse)
       {%>
    <div class="showNotesDialog" style="display: none;" title="Notes for Shared e-Portfolio: <%= Model.CourseTitle %>">
    </div>
    <% } %>

 </div>