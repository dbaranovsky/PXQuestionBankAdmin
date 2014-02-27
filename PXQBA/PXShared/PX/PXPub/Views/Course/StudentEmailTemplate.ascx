<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%
    var link = Url.RouteUrl("CourseSectionHome", new { courseid = Model.Id }, "http");
    link = (!string.IsNullOrEmpty(link)) ? link.Replace(":" + Request.Url.Port, "") : string.Empty;
    var school = (ViewData["School"] == null || string.IsNullOrEmpty(ViewData["School"].ToString())) ? string.Empty : ViewData["School"].ToString();
%>
<style type="text/css">
    p {margin:0 0 0 0; font-family: Tahoma;font-size: 12px;}
    table,td,ol,li,span,div,a { font-family: Tahoma;}
    .block-space {margin: 0 0 0 0; border: none; padding: 0;}
    table { padding: 0; border-spacing: 0; border-collapse: collapse; }
</style>
<div style="font-family: Tahoma; margin:0 0 0 0">
    <table align="center" bgcolor="#ffffff" border="1" bordercolor="#D3D3D3" width="600">
        <tbody>
            <tr>
                <td>
                    <p align="center">
                        <img border="0" src="http://image.mail.bfwpub.com/lib/feed1c737d6c03/m/1/mhe285px.png" title="mac-higheredW">
                    </p>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%">
                        <tbody>
                            <tr>
                                <td width="10">
                                    <img alt="" src="http://image.mail.bfwpub.com/lib/ffcf14/m/1/spacer.gif" height="1"
                                        width="10" border="0" style="display: block;">
                                </td>
                                <td>
                                    <table width="100%">
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <img alt="" src="http://image.mail.bfwpub.com/lib/ffcf14/m/1/spacer.gif" height="10"
                                                        width="1" border="0" style="display: block;">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" valign="top">
                                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                                        <tbody>
                                                            <tr>
                                                                <td>
                                                                    <table bgcolor="#ffffff" width="100%">
                                                                        <tbody>
                                                                            <tr>
                                                                                <td style="font-size: 13px">
                                                                                    <br>
                                                                                    <p>
                                                                                        Dear <%= Model.InstructorName %>:<br>
                                                                                    </p>
                                                                                    <br>
                                                                                    <p>
                                                                                        Your course has been activated and is ready for you and
                                                                                        your students. Your unique course URL is below, along with language you can use
                                                                                        to invite students to join (and cut down on the chances of students using you
                                                                                        for technical support). Visit <a href="http://macmillanhighered.com/getsupport">macmillanhighered.com/getsupport</a> to sign up for training or a demo.<br>
                                                                                        <br>
                                                                                    </p>
                                                                                    <p>
                                                                                        <strong>Your course URL:</strong>&nbsp;<%=link %><br>
                                                                                        <br>
                                                                                    </p>
                                                                                    <p>
                                                                                        <strong>School:</strong>&nbsp;<%= school %>
                                                                                    </p>
                                                                                    <p>
                                                                                        <strong>Course Title:</strong>&nbsp;<%= Model.Title %>
                                                                                    </p>
                                                                                    <p>
                                                                                        <strong>Course Number:</strong>&nbsp;<%=Model.CourseNumber %>
                                                                                    </p>
                                                                                    <p>
                                                                                        <strong>Course Section:</strong>&nbsp;<%= Model.SectionNumber %>
                                                                                    </p>
                                                                                    <br>
                                                                                    <br>
                                                                                    <% Html.RenderPartial(@"~/Views/Course/StudentInstructions.ascx", Model, ViewData); %>
                                                                                    <br>
                                                                                    <br>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                                <td width="10"></td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
</div>