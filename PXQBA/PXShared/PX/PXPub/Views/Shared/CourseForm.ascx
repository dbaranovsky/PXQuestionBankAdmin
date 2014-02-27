<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>"  %>
                
        <% 
            var rdLink = string.Empty;
            var rdUpload = string.Empty;
            var displayLink = string.Empty;
            var displayFile = string.Empty;
            var isUploadValid = false;
            var linkType = string.Empty;    
            
            if (Model.SyllabusType == "Url")
            {
                rdLink = "checked";
                displayLink = "block";
                displayFile = "none";
                linkType = AboutCourseLinkType.Url.ToString();        
            }
            else
            {
                rdUpload = "checked";
                displayLink = "none";
                displayFile = "block";
                isUploadValid = true;
                linkType = AboutCourseLinkType.File.ToString();        
            }
            var lmsIdLabel = System.Web.HttpUtility.HtmlEncode(Model.LmsIdLabel);      
        %>

   <% 
      using (Ajax.BeginForm("Update", null, new AjaxOptions() { OnBegin = "PxPage.Loading(\"fne-content\");", OnSuccess = "PxPage.Loaded(\"fne-content\");", UpdateTargetId = "instructor-console-wrapper" }, new { id = "courseInformationForm" }))
      { %>
        <%= Html.ValidationSummary(true)%>
        <%= Html.Hidden("View", ViewData["View"] == null ? "General" : ViewData["View"].ToString()) %>

        <%= Html.HiddenFor(model => model.Id) %>
        <%= Html.HiddenFor(model => model.ProductName) %>        
        <%= Html.HiddenFor(model => model.CourseType) %>

        <div>
            <%= Html.LabelFor(model => Model.AcademicTerm) %><br />
            <%= Html.DropDownListFor(model => model.AcademicTerm, new SelectList(Model.PossibleAcademicTerms, "Id", "Name", Model.AcademicTerm))%>
        </div>

        

        <div>
            <%= Html.LabelFor(m => m.Title) %><br />
            <%= Html.TextBoxFor(m => m.Title, new { @class = "create-course-title", size = 100 }) %>
            <span id="errorTitle" class="errorMessage"></span>
        </div>

        

        <div id="course-number">
            <%= Html.LabelFor(m => m.CourseNumber) %><br />
            <%= Html.TextBoxFor(model => model.CourseNumber, new { @class = "create-course-number", size = 50 }) %>
        </div>

        

        <div id="section-number">
            <%= Html.LabelFor(m => m.SectionNumber) %><br />
            <%= Html.TextBoxFor(model => model.SectionNumber, new {@class = "create-section-number", size = 50}) %>
        </div>

       

        <div>
            <%= Html.LabelFor(m => m.CourseUserName)%><br />
            <%= Html.TextBoxFor(model => model.CourseUserName, new { @class = "create-course-title", size = 100 })%>
            <span id="errorCourseUserName" class="errorMessage"></span>
        </div>

        

        <div>
            <%= Html.LabelFor(m => m.CourseTimeZone) %><br />
            <%= Html.DropDownListFor( model => model.CourseTimeZone, new SelectList( TimeZoneInfo.GetSystemTimeZones(), "Id", "DisplayName", TimeZoneInfo.Local.StandardName )) %>
        </div>

        

        <div>
            <%= Html.LabelFor(m => m.OfficeHours) %><br />
            <%= Html.TextBoxFor(model => model.OfficeHours, new { size = 50, @class = "officeHours" })%>
        </div>

        

        <div>
            <%= Html.Label("Contact info displayed to students") %><br />
            <div id="dynamicElementTarget">

            <%--<% if (Model.ContactInformation.Count == 0)
               { %>
                <div id="contact_0" class="contactInfoValue" style="margin-top:0px">
	                <select class="contactInfoType" name="contactInfoType_0" id="contactInfoType_0">
                        <option>Email</option>
                        <option>Phone</option>
                        <option>Fax</option>
                        <option>Other</option>
                    </select>
                    <input id="contactInfoValue_0" name="contactInfoValue_0" type="text" size="50" class="contactInfoInput" />
                    <a href="#" class="removeField" onclick="removeField('#contact_0')"></a>
                    <span id="errorcontactInfoValue_0" class="errorMessage"></span>
                </div>
                <% } %>--%>
                <% 
                    int i = 1;
                    
                    foreach (var contact in Model.ContactInformation)
                    {
                %>
                    <div id="contact_<%= i %>" style="margin-top:0px">
                        <select class="contactInfoType" name="contactInfoType_<%= i %>" id="contactInfoType_<%= i %>">
                            <option <%= contact.ContactType == "Email" ? "selected" : string.Empty %>>Email</option>
                            <option <%= contact.ContactType == "Phone" ? "selected" : string.Empty %>>Phone</option>
                            <option <%= contact.ContactType == "Fax" ? "selected" : string.Empty %>>Fax</option>
                            <option <%= contact.ContactType == "Other" ? "selected" : string.Empty %>>Other</option>
                        </select>

                        <%= Html.TextBox(string.Format("contactInfoValue_{0}", i), contact.ContactValue, new { size = 50, @class = "contactInfoInput" })%><a href='#' class="removeField" onclick="removeField('#contact_<%= i %>')"></a>
                        <span id="errorcontactInfoValue_<%= i %>" class="errorMessage"></span>
                    </div>
                <%                    
                        i++;
                    }                    
                %>
            </div>
            <div id="addcontact" class="addremove"><a href="#" id="addContact">+ New point of contact</a></div>
        </div>

        <div id="lmsDiv">
    
            <label class="dashboard-labels" for="LmsIdRequired">Student LMS ID: <a id="lmsWhatsThisShow" class="addremove" href="#"> what's this?</a></label>
            
            <div id="lmsWhatsThisBlock">
                The LMS (Learning Management System) ID is usually a school-generated Identifying number that
                allows instructors to track students by a unique number between multiple online platforms. Having
                students enter their LMS ID's into our platform allows instructors to more easily transfer grades
                and other information into to the gradebook of their school's Learning Management System. 
                Macmillan will not use or distribute student ID's entered into this platform for any reason.<br/>
                <br/>
                If you do not select this option now, you can return and change your selection at a later date.
            </div>
            <a id="lmsWhatsThisHide" class="dashboard-labels addremove" href="#">hide</a>

            <input type="radio" id="LmsIdRequiredFalse" name="LmsIdRequired" value="false" />
            Do NOT Prompt students to provide their campus LMS ID<br/>
            <input type="radio" id="LmsIdRequiredTrue" name="LmsIdRequired" value="true" />
            Prompt students to provide their campus LMS ID<br/>
            <div id="lmsIdRequiredPrompt">
                <div>
                    If you select this option, your students will NOT be able to enroll or
                    enter into your class until they provide their campus LMS ID.
                </div>
                <br/>
                <span> Your message to students:<br/></span>
                <input id="LmsIdLabel" name="LmsIdLabel" type="text" maxlength="128" value="<%= lmsIdLabel %>"/><br/>
                <i>(Edit to change the message you use to prompt students for their LMS ID.)</i>
            </div>

        </div>
        <br/>

        <div id="syllabus">
            <%= Html.Label("Syllabus:") %>
            <br />
            <input type="radio" class="rdlink" name="SyllabusType" id="SyllabusType" value="Url" <%= rdLink %> />&nbsp;<span>Link to my syllabus</span><br />
            <input type="radio" class="rdupload" name="SyllabusType" id="SyllabusType" value="File" <%= rdUpload %> />&nbsp;<span>Upload my syllabus</span> 

           <span id="UploadResults"><%= Model.SyllabusFileName %></span> 

            <div id="syllabusblock" style="display: <%= displayLink %>">
                <input class="InputForControllerAction" type="text" id="SyllabusURL" name="SyllabusURL" value="<%= Model.SyllabusUrl %>" />
                <span id="urlError" class="errorMessage"></span>
            </div>
    <% } %>

            <div id="uploadblock" style="display: <%= displayFile %>">
                <form id="fileUploadForm" method="post" action="<%= Url.Action("UploadSyllabus", "InstructorConsoleWidget") %>" enctype="multipart/form-data">
                    <input type="file" id="postedFile" name="postedFile"/>
                    <input type="button" id="uploadSubmit" value="Upload" />
                </form>
            </div>
                  
            <div class="removeSyllabus" style="display:none">Remove</div>
            <input type="hidden" class="isUploadValid" value="<%= isUploadValid.ToString().ToLowerInvariant() %>" name="isUploadValid" id="isUploadValid" />            
            
            <input type="hidden" class="InputForControllerAction" name="RefUrlFilePath" id="RefUrlFilePath" value="<%= Model.SyllabusUrl %>" />   
            <input type="hidden" class="InputForControllerAction" name="RefUrlFileName" id="RefUrlFileName" value="<%= Model.SyllabusFileName %>" />         
            <input type="hidden" name="isFormValid" class="isFormValid" value="false" />   

            
        </div>

<script type="text/javascript">
    (function ($) {
        PxPage.OnProductLoaded(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/CourseForm/CourseForm.js") %>'], function () {
                CourseForm.Init('<%= Model.ContactInformation.Count %>');
            });
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/InstructorConsoleWidget/InstructorConsole.js") %>'], function () {
                PxInstructorConsoleWidget.Init();
            });

            var lmsIdReqd = <%= Model.LmsIdRequired ? "true" : "false" %>;
            var $radios = $('input:radio[name=LmsIdRequired]');
            if (lmsIdReqd) {
                $radios.filter('[value=true]').prop('checked', true);
                $('#lmsIdRequiredPrompt').show();
            } else {
                $radios.filter('[value=false]').prop('checked', true);
                $('#lmsIdRequiredPrompt').hide();
            }

        });
    }(jQuery));
</script>