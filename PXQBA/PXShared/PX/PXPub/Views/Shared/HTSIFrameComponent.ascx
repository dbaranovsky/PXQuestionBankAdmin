<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.HTSComponent>" %>
<%
    var srcUrl = String.Format("{0}?questionId={1}&quizId={2}&entityId={3}&enrollmentId={4}&playerUrl={5}", Model.EditorUrl, Model.QuestionId, Model.QuizId, Model.EntityId, Model.EnrollmentId, Model.PlayerUrl);
    if (Model.IsAdvancedConvert)
    {
        srcUrl += string.Format("&convert={0}", Model.IsAdvancedConvert);
    }
%>

<div id="custom-hts-editor" class="custom-hts-editor custom-question-component" rel="<%= srcUrl %>"></div>

<%--<iframe id="custom-hts-editor" src="<%= srcUrl %>" rel="<%= Model.EditorUrl %>" style='width: 100%;
    height: 450px; overflow: auto;' frameborder="0" marginheight="0" marginwidth="0">
</iframe>
--%>
<%--<div id="bh-component-frame-<%= guid %>" class="bh-component" rel="<%= url %>">
</div>--%>
<script type="text/javascript">
<% if (Model.UseAsStandalone != true)
   {
%>
         PxQuizHts.SetHtsRpcHooks();  
    <% } else { %>

    PxQuestionAdmin.SetHtsRpcHooks();
    <% } %>
    </script>
 <script type="text/javascript">
     if (PxPage.switchboard) {
         $(PxPage.switchboard).trigger("htsEditorLoaded");
     }
    </script>
