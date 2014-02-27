<%@ Control Language="C#"   Inherits="System.Web.Mvc.ViewUserControl<QuestionNote>" %>
<div id="QBA-AddFlag-Form">
<b>Flag Question</b>
<br />
    <br />
<span>
    You have indicated that there is something wrong with the question by flagging it. Please give us a brief description of the problem.
</span>
<br />
    <br />
<% 
    using (Ajax.BeginForm("AddFlag", "QuestionAdmin",null, new AjaxOptions { HttpMethod = "Post", OnComplete = "PxQuestionAdmin.AddFlagCompletedEvent", OnSuccess = "PxQuestionAdmin.EditorUnblockUI()", OnBegin = "PxQuestionAdmin.EditorBlockUI()" }, new { @id = "frmAddFlag" }))
    { 
%>
    <%= Html.HiddenFor(model => model.QuestionId)%>
    <%= Html.TextAreaFor(model => model.Text, new { @id = "AddFlag_txt", @class = "required" })%>
    <br />
    <br />
    <div class="Flag-Button-Bar">
        <button id="QBA-AddNote-Save-Button" class="save QBA-button">Submit</button>
        <a href="#" id="QBA-AddFlag-Cancel-Button" class="undo-changes">Cancel</a>
    </div> 
<%
     }
%>
</div>
<div id="QBA-AddFlag-Confirmation" >
<b>Flag Question</b>
<br />
    <br />
<span>
    You flag note is successfully saved. Thank you for flagging the question.
</span>
</div>
<script type="text/javascript" language="javascript">
    jQuery(document).ready(function () {
        if (PxQuestionAdmin) {            
            PxQuestionAdmin.AddFlagEvent();
        }
    } (jQuery));   
</script>