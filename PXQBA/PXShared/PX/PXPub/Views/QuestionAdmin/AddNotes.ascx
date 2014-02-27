<%@ Control Language="C#"   Inherits="System.Web.Mvc.ViewUserControl<QuestionNote>" %>
<div id="QBA-AddNote">
<div id="QBA-AddNote-Add">
    <button id="QBA-AddNote-Add-Button" class="save QBA-button">Add Note</button>
</div>
<div id="QBA-AddNote-Form">
<p><b>Add Note</b></p>
<% 
    using (Ajax.BeginForm("AddNote", "QuestionAdmin", new AjaxOptions { HttpMethod = "Post", UpdateTargetId = "QBA-Editor-Container", InsertionMode = InsertionMode.Replace, OnSuccess = "PxQuestionAdmin.EditorUnblockUI()", OnBegin = "PxQuestionAdmin.EditorBlockUI()" }, new { @id = "frmAddNote" }))
    { 
%>
    <%= Html.HiddenFor(model => model.QuestionId)%>
    <%= Html.TextAreaFor(model => model.Text, 6, 10, new { @id = "AddNote_txt", @class = "required" })%>
    <br />    
    <button id="QBA-AddNote-Save-Button" class="save QBA-button">Save</button>
    <a href="#" id="QBA-AddNote-Cancel-Button" class="undo-changes">Cancel</a>
<%
     }
%>
</div>
</div>
<script type="text/javascript" language="javascript">
    jQuery(document).ready(function () {
        if (PxQuestionAdmin) {            
            PxQuestionAdmin.AddNoteEvent();
        }
    } (jQuery));   
</script>