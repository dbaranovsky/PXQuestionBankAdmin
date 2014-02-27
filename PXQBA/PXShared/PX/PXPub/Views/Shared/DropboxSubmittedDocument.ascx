<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Dropbox>" %>
<%
    var fileName =  Model.StudentSubmittedFilename;
    var fileType = "nofile";
    string fileImageTitle = string.Empty;
    if (!Model.StudentSubmittedFilename.IsNullOrEmpty())
    {
        fileType = Model.StudentSubmittedFilename.Substring(Model.StudentSubmittedFilename.LastIndexOf(".") +1).ToLowerInvariant();
        fileImageTitle = fileType;
        if (fileType != "doc" && fileType != "docx" && fileType != "pdf" && fileType != "rtf" && fileType != "txt" && fileType != "jpeg" && fileType != "jpg" && fileType != "png" && fileType != "tif" && fileType != "tiff" && fileType != "gif" && fileType != "xls" && fileType != "xlsx")
        {
            fileType = "unknown";
        }
    }
    var fileSize = Model.StudentSubmittedFileSize;
    var editUrl = Model.StudentSubmittedFileEditUrl;
    var editTitle = "Edit";
    var submissionStatus = Model.StudentSubmitStatus;
    var allowReSubmission = Model.AllowReSubmission;
    var pointsAssigned = Model.PointsAssigned;
    var pointsPossible = Model.PointsPossible;
    var teacherComment = Model.TeacherComment;
    var percentageScore = Math.Round((Convert.ToDouble(pointsAssigned) / Convert.ToDouble(pointsPossible)) * 100,1);
    var teacherAttachments = (List<Bfw.PX.Biz.DataContracts.Attachment>)ViewData["TeacherAttachments"];
    var teacherAttachmentFileSize = Model.TeacherAttachmentFileSize;

    var timeZone = ViewData["timeZone"] == null ? string.Empty : ViewData["timeZone"].ToString();
%>

<%if (submissionStatus != null && submissionStatus != "")
  {%>
    <h3>Submission</h3>
    <div class="dropboxsubmission">

        <div id="dropboxSubmission">
            <div id="dropboxSubmissionImage">
                <div class="<%= fileType %>" title="<%= fileImageTitle %>"></div>
            </div>
            <div id="dropboxSubmissionDesc">
                <%if (!Model.SubmitedDocTitle.IsNullOrEmpty())
                  { %>
                    <div class="title"><%= Html.RouteLink(Model.SubmitedDocTitle, "DownloadDropBoxDocument", new { id = Model.Id, name = fileName }, new { target = "_blank" })%> </div>
                <%} %>
                <%else
                  { %>
                    <div class="title"><%= Model.SubmitedDocTitle%> </div>
                <%} %>
                <div class="submission">Submitted <%= String.Format("{0} {1}", Convert.ToDateTime(Model.SubmitedDocDate).GetCourseDateTime().ToString("MM/dd/yyyy hh:mm"), timeZone)%></div>
                <div class="submissioncomment"><%= Model.SubmitedDocComment%></div>
                <div class="submissionlinks">
                <input type="hidden" id="gridSavedSubmissionAjaxUrl" value="<%=Url.Action("GetStudentDropboxDocuments","Assignment",new {id=Model.Id,isAllowSubmission = Model.IsAllowSubmission}) %>" />
                <%if (!Model.SubmitedDocTitle.IsNullOrEmpty())
                  { %>
                    <div class="downloadLink"><%= Html.RouteLink("Download", "DownloadDropBoxDocument", new { id = Model.Id, name = fileName }, new { target = "_blank" })%> <%="(" + fileSize + ")"%></div>
                <%} %>
                    <%if (submissionStatus == "graded" && allowReSubmission)
                      {
                          editTitle = "Retry";
                      }
                      else if (submissionStatus == "graded" && !allowReSubmission)
                      {
                          editTitle = string.Empty;
                      }
                        
                         %>
                         <%if (Model.IsAllowSubmission)
                           { %>
                            <a href="#" id="lnkUploadAssignment" class="uploadandsubmit-link"><%= editTitle%></a>
                         <%} %>
                    <div id="uploadDocForm">
                        <%
                            var retainOriginalFile = false;
                            if (!fileName.IsNullOrEmpty())
                            {
                                retainOriginalFile = true;
                            }
                        %>
                    </div>
                </div>
            </div>
        </div>
        <div id="dropboxGrade">
            <div class="gradecontainer"><span>Grade:</span></div>
            <%if (submissionStatus != "graded")
              {%>
                <div class="gradecomments"><span>This submission has not yet been graded.</span></div>
            <%} %>
            <%else{ %>
                <div class="gradecomments"><span><%= pointsAssigned%> out of <%=  pointsPossible%> points (<%=percentageScore%>%)</span></div>
                <%if (teacherComment != null && teacherComment != "")
                  { %>
                    <div class="gradecontainer"><span>Feedback:</span></div>
                    <div class="gradecomments"><%= teacherComment%></div>
                <%} %>

                <div class="gradecomments">
                    <%foreach (Bfw.PX.Biz.DataContracts.Attachment attachment in teacherAttachments)
                      { %>
                        <div class="gradeattachments">
                            <div class="attachmentname"><%= attachment.Name %></div>
                            <div class="attachmentlink"><%= Html.RouteLink("Download", "DownloadDropBoxTeacherDocument", new { id = Model.Id, name = attachment.Name }, new { target = "_blank" })%> <%="(" + teacherAttachmentFileSize + ")"%></div>
                        </div>
                    <%} %>
                </div>
            <%} %>
        </div>
    </div>
<%} %>


