<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ECommerceInfo>" %>

<h2 class="student-find-course-header">
    Find a course</h2>
<%= Html.Hidden("courseId",ViewData["Courses"].ToString()) %>
<%= Html.Hidden("switchFormCourseId",Model.SwitchEnrollFromCourse.ToString()) %>
<div class="course-item">
    <% 
        using (Html.BeginForm("Join", "ECommerce"))
        {
            
    %>
    <table class="course-info">
        <tr>
            <td class="first-col">
                <%= Html.Label("School:") %>
            </td>
            <td class="second-col">
                <% var domains = (IEnumerable<Domain>)ViewData["domains"];
                   if (domains.Count() > 0)
                   { %>
                <%=Html.DropDownList("domain", new SelectList(domains, "Id", "Name")) %>
                <% } %>
            </td>
        </tr>
        <tr>
            <td class="first-col">
                <%=Html.Label("Academic Term:") %>
            </td>
            <td class="second-col">
                <select id="terms" name="terms" disabled="disabled">
                </select>
            </td>
        </tr>
        <tr>
            <td class="first-col">
                <%=Html.Label("Instructor:") %>
            </td>
            <td class="second-col">
                <select id="instructors" name="instructors" disabled="disabled">
                </select>
            </td>
        </tr>
        <tr>
            <td class="first-col">
                <%=Html.Label("Course:") %>
            </td>
            <td class="second-col">
                <select id="courses" name="courses" disabled="disabled">
                </select>
            </td>
        </tr>
    </table>
       
    <div id="enroll-course-selection">
        <div class="enroll-continue" style="padding-bottom: 10px;">
            <input type="button" value="Continue" class="find-course-continue-disabled" id="find-continue-button" disabled="disabled" />
            <span class="wrong-link">If you cannot find your course please contact your instructor.</span>
            <% if (Model.GenericCourseSupported)
               { //string link = Url.RouteUrl("EcomEntitled", new { courseid = Model.GenericCourseId, switchEnrollFromCourse = Model.SwitchEnrollFromCourse }, Request.Url.Scheme);
            %>
            <div class="enroll-generic-course">
                
                <% // temporary launchpad code
                    var courseType = ViewData["CourseType"].ToString();
                   if (courseType == "LaunchPad")
                   {
                       %>
                        If you can't find your School or Instructor or would like to access <%= courseType %> resources without joining a course, leave the menus above blank and  
                    <%= Html.ActionLink("CLICK HERE", "Entitled", new { courseid = Model.GenericCourseId, switchEnrollFromCourse = Model.SwitchEnrollFromCourse }, new { id = "generic-course-id" })%>                                                       
                <% }
                   else
                   { %>
                        Can't find your instructor?
                        <%= Html.ActionLink("Enroll in general course.", "Entitled", new {courseid = Model.GenericCourseId, switchEnrollFromCourse = Model.SwitchEnrollFromCourse}, new {id = "generic-course-id"}) %>
                <% } %>
            </div>
            <%} %>
        </div>
    </div>
    <% } %>
</div>
<script type="text/javascript" >
    $(function () {
        $('#domain').unbind('change').bind('change',
            function () {
                var domainId = this.value;

                $('select#terms').html('');
                $('select#instructors').html('');
                $('select#courses').html('');

                var html = "<option value=''>--Select your Term--</option>";

                PxPage.Loading(".course-item");
                $.getJSON(PxPage.Routes.GetEnrollmentTerms + "?id=" + domainId, function (results) {
                    for (k = 0; k < results.length; k++) {
                        html += ("<option value='" + results[k].Id + "'>" + results[k].Name + "</option>");
                    }

                    $('select#terms').html(html).removeAttr("disabled");
                    enableContinue(false);
                    PxPage.Loaded(".course-item");
                });
            });

        $('#terms').unbind('change').bind('change',
            function () {
                var termid = this.value;

                $('select#instructors').html('');
                $('select#courses').html('');

                var domainid = $("select#domain option:selected").attr("value");

                PxPage.Loading(".course-item");
                $.getJSON(PxPage.Routes.GetEnrollmentInstructor + "?id=" + domainid + "&term=" + termid, function (results) {
                    var html = "<option value=''>--Select your Instructor--</option>";

                    for (k = 0; k < results.length; k++) {
                        html += ("<option value='" + results[k].Id + "'>" + results[k].LastName + ", " + results[k].FirstName + "</option>");
                    }

                    $('select#instructors').html(html).removeAttr("disabled");
                    enableContinue(false);
                    PxPage.Loaded(".course-item");
                });
            });

        $('#instructors').unbind('change').bind('change',
            function () {
                var instructorid = this.value;
                var domainid = $("select#domain option:selected").attr("value");
                var termid = $("select#terms option:selected").attr("value");

                $('select#courses').html('');

                PxPage.Loading(".course-item");

                $.getJSON(PxPage.Routes.GetEnrollmentCourses + "?id=" + domainid + "&term=" + termid + "&userid=" + instructorid, function (results) {

                    var html = "<option value=''>--Select your Course--</option>";

                    enableContinue(false);

                    for (var k = 0, len = results.length; k < len; k ++) {
                        if (len == 1) {
                            html += ("<option value='" + results[k].Id + "' selected>" + results[k].CourseNumber + " " + results[k].SectionNumber + " " + results[k].Title + "</option>");
                            $('#courses').trigger('change');

                        }
                        else {
                            html += ("<option value='" + results[k].Id + "'>" + results[k].CourseNumber + " " + results[k].SectionNumber + " " + results[k].Title + "</option>");
                        }
                    }

                    $('select#courses').html(html).attr('size', 4).removeAttr("disabled");
                    PxPage.Loaded(".course-item");
                });

            });

        $('#courses').unbind('change').bind('change',
            function () {
                if ($("#courses option:selected").text() != "--Select your Course--") {
                    enableContinue(true);
                } else {
                    enableContinue(false);
                }
            });
        
        $('#find-continue-button').unbind('click').bind('click', function (event) {
            var courseSelected = $('.course-item #courses').val();
            if (courseSelected == null || courseSelected == "") {
                return false;
            } else {
                PxPage.Loading(".course-item");
                var segments = PxPage.Routes.GetCoursesDetails.split('/');
                var url = '';
                
                for (var i = 0; i < segments.length; i++) {
                    if (i == 3) {
                        segments[i] = courseSelected;
                    }

                    if (segments[i].length > 0) {
                        url += '/' + segments[i];
                    }
                }

                PxPage.Loaded(".course-item");
                document.location = url;
            }
        });

        function enableContinue (enable) {
            if (enable) {
                $('#find-continue-button').removeClass("find-course-continue-disabled").addClass("find-course-continue");
                $('#find-continue-button').removeAttr('disabled');
            } else {
                $('#find-continue-button').removeClass("find-course-continue").addClass("find-course-continue-disabled");
                $('#find-continue-button').attr('disabled', 'disabled');
            }
        }
    });
</script>