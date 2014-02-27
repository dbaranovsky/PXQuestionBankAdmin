<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>

<% if ((Model.Questions != null) && (Model.Questions.FirstOrDefault().AdminFlag == null) || (Model.Questions.FirstOrDefault().AdminFlag == false))
   {
%>

<div id="QuestionFlagContainer" >

        <div id="FlagContents" class="second">
            <div id="QBA-Flag-Container" style="display:none">
                    <% Html.RenderPartial("~/Views/QuestionAdmin/AddFlag.ascx", new QuestionNote() {QuestionId = Model.Id}); %>
            </div>
        </div>

        <%
            if (Model.Questions.FirstOrDefault().AdminFlag == false)
            {
        %>
        <div class="footer">
                <div class="preview-button-bar" style="display:none">   
                    <button id="QBA-Flag-Button" class="QBA-button">Flag Question</button>
                </div>
        </div>
        <%
            }
        %>

</div>

<% } %>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
                PxQuestionAdmin.FlagEvent();
        });
    } (jQuery));    
</script>
