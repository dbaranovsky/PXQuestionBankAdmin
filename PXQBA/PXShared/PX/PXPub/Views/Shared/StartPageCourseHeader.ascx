<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.CourseHeader>" %>
<!--[if gte IE 9]>
  <style type="text/css">
    .gradient {
       filter: none !important;
    }
  </style>
<![endif]-->
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
             $("#main").addClass("editing-homepage");
             $(PxPage.switchboard).trigger("StartPageEdit");

             if ($.trim($('.facePlate-start_welcome_msg').html()) == '') {
                $('.facePlate-start_welcome_msg').html('Click here to edit');
             }
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
             $("#main").removeClass("editing-homepage");
             $(PxPage.switchboard).trigger("StopPageEdit");

             if ($.trim($('.facePlate-start_welcome_msg').html()) == 'Click here to edit') {
                $('.facePlate-start_welcome_msg').html('');
             }
         });

          $(".editPageStart").show();

        <%} else { %>
            $(".editPageStart").hide();
            $(".doneEditing").hide();
        <%} %>
     });
</script>
    
     <% Html.RenderPartial("CourseTimeZoneData"); %>
    <%= Html.HiddenFor(m => m.CourseId) %>
    <div class="editPageEnd" style="display:none; width:100%; text-align:center;">
        Click any widget you'd like to edit; drag to repostition. 
        <input type="button" class="doneEditing" value="Done editing" />    
    </div>

<div class="homepage-course-info">
        <% Html.RenderPartial("BackToBlackBoard"); %>

        <div class="editpagebtnwrpr">
    <% 
    if (Model.IsAllowPageEdit)
    { 
    %>
            <a class="editPageStart" href="#">Edit Page</a>
    <% 
    } 
    %>
        </div>
 </div>