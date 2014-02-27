<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<% 
    var link = Url.RouteUrl("CourseSectionHome", new { courseid = Model.Id }, "http");
    link = (!string.IsNullOrEmpty(link)) ? link.Replace(":" + Request.Url.Port, "") : string.Empty;
    var instructorEmail = (ViewData["InstructorEmail"] == null || string.IsNullOrEmpty(ViewData["InstructorEmail"].ToString())) ? string.Empty : ViewData["InstructorEmail"].ToString();
%>
<p class="student-started-dashboard">
    <strong>Get your students started.</strong>
    <br>
</p>
<br>
<p>
    To promote seamless student registration, we've created
    <a href="http://cmg.screenstepslive.com/s/3918/m/11562/l/130677-how-do-students-register-into-my-course">First Day of Class resources</a>.
    You can also adapt, copy, and paste the following student-facing instructions into
    a syllabus, an e-mail, or your course description.
    <br>
    <br>
</p>
<div class="studentemail-dashboard">
    <table>
        <tr>
            <td style="background-color: #ffd; border: 1px solid #ddd; padding: 7px 10px; vertical-align: top; text-align: left; font-size: 12px">
                <br>
                Dear Students,
            <p>
                <br>
            </p>
                <p>
                    My online course is open for student registration--follow the simple steps below to get started.
                </p>
                <br>
                <ol class="studentemail-instruction-dashboard">
                    <li><span style="font-size: 10pt;">Go to <a href="#" class="student-url-display"><strong><%= link %></strong></a></span></li>
                    <li><span style="font-size: 10pt;"><strong>Bookmark </strong>the page to make it easy to return to.</span></li>
                    <li><span style="font-size: 10pt;">If you have an <strong>access code</strong>, 
                    click the button "Register an activation code" in the upper right 
                    corner and follow the instructions.</span></li>
                    <li><span style="font-size: 10pt;">If you don't have an access 
                    code, click the "<strong>Purchase Access</strong>" button.</span></li>
                    <li><span style="font-size: 10pt;">If you have any questions or
                    problems logging in, please <strong>contact Technical Support</strong>. Technical support will need
                    a technical support incident ID if you continue to have trouble, so be sure to save
                    that ID when you report your issue. You can reach a representative:</span></li>
                </ol>
                <blockquote style="margin: 0 0 0 40px; border: none; padding: 0">
                    <ul class="contact-details">
                        <li><span style="font-size: 10pt;">by phone at (877) 587-6534&nbsp;</span></li>
                        <li><span style="font-size: 10pt;">through our <a href="http://support.bfwpub.com/supportform/form.php?View=contact"
                            title="tech support">online form</a></span></li>
                    </ul>
                </blockquote>
                <blockquote class="block-space">
                    <blockquote class="block-space">
                        <p>
                            <strong>Tech Support Hours </strong>(all times EST)
                        </p>
                    </blockquote>
                    <blockquote class="block-space">
                        <p>
                            Monday - Thursday&nbsp;&nbsp; 9:00 AM - 3:00 AM
                        </p>
                    </blockquote>
                    <blockquote class="block-space">
                        <p>
                            Friday&nbsp;&nbsp;9:00 AM - 11:00 PM
                        </p>
                    </blockquote>
                    <blockquote class="block-space">
                        <p>
                            Saturday&nbsp;&nbsp;11:30 AM - 8:00 PM
                        </p>
                    </blockquote>
                    <blockquote class="block-space">
                        <p>
                            Sunday&nbsp;&nbsp;11:30 AM - 11:30 PM
                        </p>
                    </blockquote>
                </blockquote>

                <br>
                System Requirements: <a href="http://www.macmillanhighered.com/sysreq">www.macmillanhighered.com/sysreq</a>
                <br>
                <br>
                <p>
                    Looking forward to seeing you in class!
                </p>

                <br>

                <p>
                    <%= Model.InstructorName %>
                </p>
                <p class="instructor-email">
                    <%= instructorEmail %>
                </p>
            </td>
        </tr>
    </table>
</div>
<br>