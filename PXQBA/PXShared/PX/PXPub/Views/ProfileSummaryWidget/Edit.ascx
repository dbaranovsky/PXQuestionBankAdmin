<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.ProfileSummaryWidget>" %>
<%--<div style="padding-top:30px;padding-left:20px; width: 400px;height:400px !important;">--%>
<% Random rand = new Random(); %>
        <%  using (Html.BeginForm("Save", "ProfileSummaryWidget", FormMethod.Post, new { enctype = "multipart/form-data", id = "frmProfileEditorWidget" }))
	{ %>
	<div>
		<input type="hidden" id="userId" name="userId" value="<%= Model.UserProfile.Id %>"/>
		<input type="hidden" id="userRefId" name="userRefId" value="<%= Model.UserProfile.ReferenceId %>"/>


		<ul>
				<li class="editprofile-label">First Name
				<input type="text" id="fn" name="fn" class="textwidth-small" value="<%= Model.UserProfile.FirstName %>"/>
						<span class="eportfoliocourseNamevalidation field-validation-error" id="FirstNameErrMsg" style="width: 200px;display:none">Please provide First Name</span>
						<%= Html.ValidationMessageFor(model => model.UserProfile.FirstName) %>
				</li>
				<li class="editprofile-label">Last Name
				 <input type="text" id="ln" name="ln" class="textwidth-small" value="<%= Model.UserProfile.LastName %>"/>
					<span class="eportfoliocourseNamevalidation field-validation-error" id="LastNameErrMsg" style="width: 200px;display:none">Please provide Last Name</span>
					<%= Html.ValidationMessageFor(model => model.UserProfile.LastName) %>
				</li>
				<li class="editprofile-label">E-mail address
				 <input type="text" id="em" name="em" class="textwidth-small" value="<%= Model.UserProfile.Email %>"/>
					<%= Html.ValidationMessageFor(model => model.UserProfile.Email) %>
					<span class="eportfoliocourseNamevalidation field-validation-error" id="EmailErrMsg" style="width: 200px;display:none">Please provide valid email</span>
				</li>
		</ul>    
		<br/>
        <div>Profile picture</div>
		<ul class="profile-upload">
			<li class="editprofile-input">
           
					<div class="editprofile-img-wrapper">
					    <img src="<%= (!Model.UserProfile.AvatarUrl.IsNullOrEmpty())
		                   	    ? Model.UserProfile.AvatarUrl
		                   	    : Url.Action("Index", "Style", new {path = "images/unknown_user.jpg"}) %>?<%= rand.Next() %>" alt="Image" class="profile-img" style="height: 73px; width:57px" />
                     </div>

                    <div class="editprofile-doc-wrapper">
                            <input id="docFile" name="docFile" type="file" value="browse" size="45"  style="margin-top:21px"/>
                    </div>
			
            </li>
			
		</ul>
        <ul><li class="edit-profile-btns">
			    <input class="linkButton" type="button" value="Save" onclick="return PXProfileSummaryWidget.Save({
					userId: '<%= Model.UserProfile.Id %>',
					userRefId: '<%= Model.UserProfile.ReferenceId %>',
                    firstname: $('#fn').val(),
                    lastname: $('#ln').val(),
                    email: $('#em').val(),
                    access: '<%=Model.AccessLevel.ToString().ToLowerInvariant() %>'
					});" />
				<input class="linkButton" type="button" value="Cancel" onclick="return PXProfileSummaryWidget.Cancel();" />
			</li></ul>
<div id = 'content-item' /></div>
<% } %>

