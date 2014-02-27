<%@ Page Title="" Language="C#" MasterPageFile="~/Views/QuestionAdmin/QuestionAdmin.Master" Inherits="System.Web.Mvc.ViewPage<Bfw.PX.PXPub.Models.QuestionAdminSearchPanel>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Question Bank Manage - Search
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderAdditions" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CenterContent" runat="server">
    <% Html.RenderPartial("FneWindow"); %>
	<div id="QBA">
		<table id="maintable">
		   <tr class="MainHeader">
				<td colspan="2">
					<div class="question-bank-title">
						Question Bank Manager for <b><%= ViewData["CourseTitle"]%></b>
                        <%= Html.RouteLink("Logout", "EcomEntitled", null, new { @class = "logout" })%>
					</div>        
				</td>
		   </tr>        
		   <tr class="MainBody" id="MainBody">
				<td class="left">
					<% Html.RenderAction("SearchPanel", Model); %>
				</td>
				<td id="tdSearchResultPanel">
					<% 
					Html.RenderAction("SearchResult", Model);

					 %>
				</td>
		   </tr>
		</table>
	</div>

	
	<script type="text/javascript" language="javascript">
		
		var select_chapter_route = "<%= Url.ActionCache("SelectChapter","QuestionAdmin") %>";

		var reset_search_result_route = "<%= Url.ActionCache("SearchResult","QuestionAdmin") %>";	
		
		var quizzes_tab_route = "<%= Url.Action("QuizzesTab", "QuestionAdmin" )%>";
	    var QBA_show_quiz_route = '<%= Url.ActionCache("ShowQuiz", "Quiz") %>';
        var update_question_metadata = "<%= Url.Action("UpdateMetadata", "QuestionAdmin" )%>";
 	    
        var QBA_search_route = reset_search_result_route;
        var QBA_BulkEdit = "<%= Url.Action("BulkEditConfirm", "QuestionAdmin" )%>";

 	 
		jQuery(document).ready(function() {
			
			PxQuestionAdmin.Init();
			
			//DynamicFormatDropDown['initDropDownResponseFormat']();			
			DynamicAddQuestionDropDown['initDropDownAddQuestion']();

		});

	</script>

</asp:Content>


