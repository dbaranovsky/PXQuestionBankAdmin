
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AboutCourse>" %>
<div class="customEditWidget" dialogwidth="400" dialogheight="600" dialogminwidth="400"
    dialogminheight="600" dialogtitle="Course Information">

<div class="courseinformationwidget">
<style type="text/css">
    .addremove a { text-decoration: none; }

    .feildtext {
        float: left;
        width: 50px;
    }

    .field { float: left }

    .subtractfield a {
        text-decoration: none;
        font-size: 2em;
    }
    
    .removeContactInfo
    {
        cursor: pointer;
    }

    .ddlContactType {
        float: left;
        width: 70px
    }
    
    .water_mark
    {
        color: #CCC;
    }
</style>
<%:Html.ValidationSummary(true)%>
<form id="courseinformationform">
<% if (Model.ShowCourseTitle)
   {%>
    <div class="editor-label">
        * Course Name:
    </div>
    <div class="editor-field">
        <input type="text" name="CourseName" id="CourseName" class="InputForControllerAction" value="<%=Model.CourseTitle%>" />
    </div>
<% } %>

<% if (Model.ShowInstructorName)
   { %>
    <div class="editor-label">
        * Instructor Name:
    </div>
    <div class="editor-field">
        <input type="text" name="InstructorName" id="InstructorName" class="InputForControllerAction" value="<%=Model.InstructorName%>" />
    </div>
    <% } %>

    <% if (Model.ShowOfficeHours)
       { %>
    <div class="editor-label">
        Office Hours:
    </div> 
    <div class="editor-field">
        <input type="text" name="OfficeHours" id="OfficeHours" class="InputForControllerAction" value="<%=Model.OfficeHours%>" />
    </div>
    <% } %>

    <div id="aboutEditContents">
       <div class="editor-label">
       Contact:
    </div> 
        <% 
            if (Model != null && Model.ContactInfo.Count() > 0)
            {
                int current = 0;
                foreach (AboutCourseContactInfo c in Model.ContactInfo)
                {   %>
  
                <% if (c.Info.IsNullOrEmpty()) { continue; } %>
        <div id="<%:current%>" class="contactInfoHolder">
            <div id="ContactType_<%:current%>">
                <div id="ddlContactTypeContainer_<%:current%>" class="fieldtext">
                    <select class=" contactInfoType"  id="ContactInfo_<%:current%>_Type" name="ContactInfo[<%:current%>].Type">
                        <% string[] c_types = new string[4] {"Phone", "Email", "Fax", "Other"};
                    for (int i = 0; i < c_types.Length; i++)
                    {
                        if (c_types[i] == c.Type)
                        {%>
                        <option class="" selected="selected" value="<%=c_types[i]%>"><%=c_types[i]%></option>     
                                            
                        <% }
                        else
                        { %>
                        <option class="" value="<%=c_types[i]%>"><%=c_types[i]%></option>
                        <% }
                    } %>
                    </select>
                </div>
                <input class="txtContactInfo <%= c.Type %>" type="text" id="ContactInfo_<%:current%>_Info" name="ContactInfo[<%:current%>].Info" value="<%:c.Info%>" />
                <div id="subtract_contact_<%:current%>" class="subtractfield">
                    <a class="removeContactInfo">-</a>
                </div>
            </div>
        </div>

            <% current++;%>
            <% }
            }
            else
            { %>
        <div id="0" class="contactInfoHolder">
            <div id="ContactType_0">
                <div id="ddlContactTypeContainer_0" class="fieldtext">
                    <select class="contactInfoType" id="ContactInfo_0_Type" name="ContactInfo[0].Type">
                        <option>Phone</option>
                        <option>Email</option>
                        <option>Fax</option>
                        <option>Other</option>
                    </select>
                </div>
                <input class="txtContactInfo Phone" type="text" id="ContactInfo_0_Info" name="ContactInfo[0].Info" />
                <div class="subtractfield">
                    <a class="removeContactInfo">-</a>
                </div>
            </div>
        </div>
        <% }%>

    </div>
            <div id="dynamicElementTarget"></div>
    <div id="addcontact" class="addremove"><a href="javascript:;" onclick=" addFields('this'); ">+ Add another point of contact</a></div>
    

   

    <%= Html.Hidden("hdnContactInfoCount",Model.ContactInfo.Count, new { @class = "hdnContactInfoCount" }) %>

</form>

<% 
    var rdLink = string.Empty;
    var rdUpload = string.Empty;
    var displayLink = string.Empty;
    var displayFile = string.Empty;
    var isUploadValid = false;
    var linkType = string.Empty;    
    if (Model.SyllabusLinkType == AboutCourseLinkType.Url || Model.SyllabusLinkType == AboutCourseLinkType.None)
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
   
    %>

<div class="editor-field urlform">
    <div class="editor-label">
      Syllabus:
    </div> 
    <div class="syllabuswrapper" style="width:80%">
    <input type="radio" class="rdlink" name="group1" value="Link" <%= rdLink %> onclick=" showLinkBox() "/>&nbsp;<span>Link to my syllabus</span>
    <input type="radio" class="rdupload" name="group1" value="Upload" <%= rdUpload %> onclick=" showUpload() "/>&nbsp;<span>Upload my syllabus</span> 

    <span id="UploadResults"><%= Model.SyllabusFileName %></span>

    <div id="syllabusblock" style="display:<%= displayLink %>">
        <input class="InputForControllerAction" id="SyllabusURL" name="SyllabusURL" value="<%=Model.SyllabusUrl%>" onblur="AddWaterMark();" onfocus="ClearWaterMark();" />
    </div>

    <div id="uploadblock" style="display:<%= displayFile %>">
        <form id="fileUploadForm" method="post" action="<%= Url.Action("UploadSyllabus", "AboutCourseWidget") %>" enctype="multipart/form-data">
            <input type="file" id="postedFile" name="postedFile"/>
            <input type="submit" class="uploadsubmit" value="Upload" />
        </form>
    </div>
    </div>
    
    <div class="removeSyllabus">Remove</div>
    <input type="hidden" class="isUploadValid" value="<%= isUploadValid.ToString().ToLowerInvariant() %>" name="isUploadValid" id="isUploadValid" />
    <input type="hidden" class="InputForControllerAction" name="aboutCourseLinkType" id="aboutCourseLinkType" value="<%= linkType %>"/> 
    <div class="contactInfoSubmit"></div>
    <input type="hidden" class="InputForControllerAction" name="RefUrlFilePath" id="RefUrlFilePath" value="<%=Model.SyllabusUrl%>" />   
    <input type="hidden" class="InputForControllerAction" name="RefUrlFileName" id="RefUrlFileName" value="<%= Model.SyllabusFileName %>" />         
    <input type="hidden" name="isFormValid" class="isFormValid" value="false" />   
</div>


<script type="text/javascript">
    (function ($) {

        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/AboutCourseWidget/AboutCourseWidget.js") %>'], function () {
                PxAboutCourseWidget.BindControls();
            });
        });

    } (jQuery));    
</script>


</div>

</div>