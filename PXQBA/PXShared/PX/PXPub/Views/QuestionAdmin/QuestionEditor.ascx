<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.QuestionAdminSearchPanel>" %>
    <div id="QBAEditor">
        <table id="maintable" >
            <tr class="MainHeader">
                <td>
                    <div>
                        <input type="button" id="QBA-DoneEditing" value="Done Editing" class="QBA-button"/>
                        <span>Question Editor</span>
                        <input id="SelectedQuestionId" type="hidden" value='<%= ViewData["questionId"] %>' />
                        <input id="SelectedQuestionType" type="hidden" value='<%= ViewData["questionType"] %>' />
                        <input id="SelectedQuestionUrl" type="hidden" value='<%= ViewData["questionCustomUrl"] %>' />
                        <input id="SelectedQuestionEntityId" type="hidden" value='<%= ViewData["questionEntityId"] %>' />
                        <input id="PreviewEntityId" type="hidden" value='<%= ViewData["previewEntityId"] %>' />
                    </div>
                </td>
            </tr>
            <tr class="Navigation">
                <td>
                    <div id="NavigationTabs">
                        <ul class="QBA-Editor-Tabs">
                            <li><a href="#" id="questiontablink" class="RouteTab maintab showalert"  route="<%= Url.Action("QuestionEditorTab",
	   	                                new
	   	                                	{
	   	                                		questionId = ViewData["questionId"],
	   	                                		quizId = ViewData["quizId"]
	   	                                	}) %>">
                                Question</a></li>
                            <li><a href="#" id="historytablink" class="RouteTab" route="<%= Url.Action("HistoryTab",
	   	                                new
	   	                                	{
	   	                                		questionId = ViewData["questionId"],
	   	                                		quizId = ViewData["quizId"]
	   	                                	}) %>">
                                History</a></li>
                            <li><a href="#" id="notestablink" class="RouteTab" route="<%= Url.Action("NotesTab",
	   	                                new
	   	                                	{
	   	                                		questionId = ViewData["questionId"],
	   	                                		quizId = ViewData["quizId"]	   	                                		
	   	                                	}) %>">
                                Notes</a></li>
                            <%--<li><a href="#" id="quizzestablink" class="RouteTab" route="<%= Url.Action("QuizzesTab",
	   	                                new
	   	                                	{
	   	                                		questionId = ViewData["questionId"],
	   	                                		quizId = ViewData["quizId"]
	   	                                	}) %>">
                                Quizzes</a></li>   --%>
                                <li><a href="#" id="metadatatablink" class="RouteTab" route="<%= Url.Action("MetaDataTab",
	   	                                new
	   	                                	{
	   	                                		questionId = ViewData["questionId"],
	   	                                		quizId = ViewData["quizId"],
                                                chapter = ViewData["chapter"]
	   	                                	}) %>">
                                Metadata</a></li>   
                                <li class="special2"><a href="#" id="previewtablink" class="RouteTab" route="<%= Url.Action("PreviewTab",
	   	                                new
	   	                                	{
	   	                                		questionId = ViewData["questionId"],
	   	                                		quizId = ViewData["quizId"]
	   	                                	}) %>">Preview Question</a></li>
                                             
                            <li class="special unflagtab">
                                <%= Ajax.ActionLink("Unflag Question", "RemoveFlag", new {questionId = ViewData["questionId"]},
	   	                                   new AjaxOptions { HttpMethod = "Post", OnComplete = "PxQuestionAdmin.RemoveFlagCompletedEvent", OnSuccess = "PxQuestionAdmin.EditorUnblockUI()", OnBegin = "PxQuestionAdmin.EditorBlockUI()" })%></li>
                            
                        </ul>
                    </div>
                </td>
            </tr>
            <tr class="MainBody" >
                <td id="TabContainer">
                    <div id="QBA-Editor-Container">
                        
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">       
    var deps = ['<%= Url.ContentCache("~/Scripts/Common/DialogHelper.js")%>'].concat(<%= ResourceEngine.JsonFor("~/Scripts/questionBankAdmin.js") %>);
    PxPage.Require(deps,function() {        
        jQuery(document).ready(function() {             
            <% 
                if (ViewData["AdminFlag"] == null)
                {
            %>
                    PxQuestionAdmin.HideUnflagTab();
            <%
                }
            %>
            PxQuestionAdmin.InitQuestionEditorPage();                   
        });
    });
    </script>   
