<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.EcomerceJoinCourse>" %>

<div id="genericCourse">

 <table class="course-info">
                <%if (!Model.course.CourseSubType.Equals("generic", StringComparison.InvariantCultureIgnoreCase)) { %>
                <tr>
                    <td class="first-col">
                        <%= Html.Label("ACADEMIC TERM:  ")%>
                    </td>
                    <td class="second-col">
                        <%= Html.Label(Model.AcademicTerm)%>
                    </td>
                </tr>
                <%} %>
                <tr>
                    <td class="first-col">
                        <%= Html.Label("COURSE TITLE:  ")%>
                    </td>
                    <td class="second-col">
                        <%= Html.Label(Model.course.Title)%>
                    </td>
                </tr>
                <tr>
                    <td class="first-col">
                        <%= Html.Label("COURSE START DATE:  ")%>
                    </td>
                    <td class="second-col">
                        <%if (Model.course.IsActivated){ %>
                            <%= Html.Label(Model.course.ActivatedDate)%>
                        <%} %>
                    </td>
                </tr>
            </table>
    
    <div id="lmsDiv" class="GenericCourse" style="display:none">
       <div><%= 
            Model.course.LmsIdLabel.IsNullOrEmpty() ? Html.LabelForModel("Students, enter your Campus ID here:"):
                Html.LabelFor(m => m.course.LmsIdLabel,Model.course.LmsIdLabel)%>
       </div>
        <input id="lmsStudentId" name="lmsStudentId" type="text" value="<%= ViewData["lmsStudentId"] == null ? string.Empty : ViewData["lmsStudentId"].ToString() %>" maxlength="128" /><br/>
        <i>(required)</i>
    </div>
    <br/>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function() {
            var lmsIdReqd = <%= Model.course.LmsIdRequired ? "true" : "false" %>;
            if (lmsIdReqd) {
                $('#lmsDiv').show();

                if (!$('#lmsStudentId').val().trim()) {
                    $('#join-course-confirmation').attr('disabled', 'disabled');
                    $('#join-course-confirmation').addClass('join-course-confirmation-disabled');
                }
                
                $('#lmsStudentId').keyup(function () {
                    if ($('#lmsStudentId') == null || !$('#lmsStudentId').val().trim()) {
                        $('#join-course-confirmation').attr('disabled', 'disabled');
                        $('#join-course-confirmation').addClass('join-course-confirmation-disabled');
                    } else {
                        $('#join-course-confirmation').removeAttr('disabled');
                        $('#join-course-confirmation').removeClass('join-course-confirmation-disabled');
                    }
                });

            } else {
                $('#lmsDiv').hide();
                $('#join-course-confirmation').removeAttr('disabled');
                $('#join-course-confirmation').removeClass('join-course-confirmation-disabled');
            }

        });
    }(jQuery));
</script>

</div>
