<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<div class="activate-course-fne-window">
<div id="activatecourse">
    <%= Html.Hidden("courseFneTitle", "Activating your " + Html.DisplayFor(m=>m.CourseProductName))%>
    <%  var isEdit = ViewData["edit"] != null;
        var contentUrl = Url.Action("Index", ""); %>
    <script type="text/javascript">
        (function ($) {
            PxPage.OnReady(function () {
                if ('<%= isEdit %>' == 'False') {
                    PxPage.SetFneCreateCourseTitle();
                    PxPage.FneCloseHooks['activate-course-fne-window'] = function () {
                        window.location = '<%= contentUrl %>';
                    };

                    $("#fne-title-action-right").hide();
                    PxPage.Loading("waitingdialog");
                    $("#btnActivate").click();
                }
                else {
                    window.location = '';
                    return false;
                }
            });
        } (jQuery));        
    </script>
    <% if (!isEdit)
       { %>
        <%  var fnSuccess = "PxPage.SetFneCreateCourseTitle";
            using (Ajax.BeginForm("ActivateCourse", "Course", new AjaxOptions() { UpdateTargetId = "activatecourse", OnSuccess = fnSuccess }, new { id = "form-activate" }))
            {%>
        <%= Html.ValidationSummary(true)%>
        <div id="waitingdialog"></div>
    
        <div class="important" style="height:200px;">
            <span class="important-alert">
               Activating your <span class="creator-title"><%=Model.CourseProductName%></span> , you will be unable to select a different teaching style
            </span>
            By not allowing the interface to change we reduce student confusion.
            By selecting a teaching style you can specify an interface that most appropriately
                supports how you teach.
        </div>
    
    
        <div class="activation-btns">
            <% if (!Model.IsActivated)
               {%>
                <input type="submit" value="Activate" class="submit" onclick="PxPage.OnFormSubmit('Activating Course')" id="btnActivate" />
            <% } %>
                <input type="button" class="submit" value="Cancel" onclick="$.unblockUI();" />
        </div>
        <%} %>
    <%}
       else
       {%>
         <div class="important" style="height:200px;">
            <span class="important-alert">
               You successfully edited your course: <span class="creator-title"><%=Model.Title%></span>
            </span>
        </div>   
     <%} %>
    </div>
</div>