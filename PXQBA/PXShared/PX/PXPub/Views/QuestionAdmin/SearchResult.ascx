<%@ Control Language="C#"   Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.QuestionAdminSearchPanel>" %>
<%@ Register Src="DynamicAddQuestion.ascx" TagPrefix="uc" TagName="DynamicAddQuestion" %>
<%@ Register TagPrefix="uc" TagName="DynamicFormatDropDown" Src="~/Views/QuestionAdmin/DynamicFormatDropDown.ascx" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Bfw.Common.Pagination" %>

<div id="SearchResult">
	<table id="QBA-Result-Table">
		
	   <% if (Model.BulkEdit == true)
		  { %>
			 <tr class="body">
				<td colspan="3" id="tdQuestionsListResult">      	            																		
					<%	Html.RenderPartial("~/Views/QuestionAdmin/QuestionsListResult.ascx", Model); %>				
				</td>
			 </tr>
		   <% }
		  else
		  {  // for Normal Search result Mode when BulkEdit = false %> 
		
		<tr class="header">
           
			<td id="paging-info">
				<%if (Model == null || ViewData["TotalCount"] == null)
	  {
				%>
					Displaying <% = ViewData["TotalCount"] ?? 0%> questions.  
				<% 
	  }
	  else if (Model.Pagination != null && Model.Pagination.TotalItems > 0)
	  {
		  var totalItems = 0;

		  if (ViewData["TotalCount"] != null) totalItems = Convert.ToInt32(ViewData["TotalCount"]);

		  int currentPage = Model.Pagination.CurrentPage;

				%>				  	
					<%: Html.Paging(Model.Pagination)%>

					<%= Html.HiddenFor(model => model.Pagination.TotalItems, new { @id = "SearchResultTotalItems", Value = totalItems })%> 
					<%= Html.HiddenFor(model => model.Pagination.CurrentPage, new { @id = "SearchResultCurrentPage", Value = currentPage })%> 				   
					<%= Html.HiddenFor(model => model.NextPageStartSearchQuestion, new { @id = "SearchResultNextPageStartSearchQuestion", Value = Model.NextPageStartSearchQuestion })%> 
					<%= Html.HiddenFor(model => model.NextPageStartSearchQuiz, new { @id = "SearchResultNextPageStartSearchQuiz", Value = Model.NextPageStartSearchQuiz })%> 
								   
				<% 
	  } 
				%> 
				<input id="TotalCount" type="hidden" value="<% =ViewData["TotalCount"]%>"/>   
		   </td>
			   <td id="td1" width="8%"> 
				<button id="btnBulkEdit">Bulk Edit</button>
				
			</td> 
			<td id="tdAddQuestion" width="15%">
				
				<uc:DynamicAddQuestion runat="server" ID="ucDynamicAddQuestion" />
			</td>            
		</tr>
		<tr ><td colspan="2">
				<%
	  bool showHeaderForQuestionsResutl = (ViewData["TotalCount"] != null);

	  bool nothingFound = (ViewData["NothingFound"] != null);


	  if (Model == null | nothingFound | ViewData["TotalCount"] == null)
	  {
					 %>                
						
					 <%
		  if (nothingFound)
		  {
							%>
								<div class="noresult"> No questions found using the search parameters.</div>
							<%
		  }
		  else if (ViewData["TotalCount"] == null)
		  {					 
							%>                   					
								Use the search bar to the left to locate questions. <br />
								Click Search if you want to see all questions in this question bank.										
							<%
		  }
							%>

				<%                                 	           		
	  }
				 %>

				<%--<table id="QBA-QuestionList-Table-header" style="visibility:<% = (showHeaderForQuestionsResutl) ? "visible": "hidden" %>" >																
						<tr  class="headerForQuestionsResutl" >
						   
							<td class="ExerciseNo">Status</td>
							<td class="ExerciseNo">Exercise No</td>
							<td class="QuestionBank">Question Bank</td>
							<td class="ebook">eBook Chapter</td>
							<td class="QuestionTitle">Title</td>
							<td  class="first" id="tdResponseFormat">					                          	
									<uc:DynamicFormatDropDown runat="server" ID="ucDynamicFormatDropDown" />
							</td>
							 
							
						</tr>
				</table>--%>
  
		</td></tr>		


		<tr class="body">
		<%
	  if (showHeaderForQuestionsResutl)
	  {
				%>
				
				<td colspan="3" id="tdQuestionsListResult">      	            																		
					<%	Html.RenderPartial("~/Views/QuestionAdmin/QuestionsListResult.ascx", Model); %>				
				</td>
				<%
	  }
			
			 %>
	</tr>
		<tr class="footer">
			<td colspan="2">                
			</td>
		</tr>


		<% } %>
	</table>
</div>

<script type="text/javascript">

	var add_question = "<%= Url.ActionCache("NewQuestion","QuestionAdmin") %>";
	
	//DynamicFormatDropDown['createDropDownResponseFormat']();
	DynamicAddQuestionDropDown['createDropDownAddQuestion'](add_question);

    $(document).off('click', '#btnBulkEdit').on('click','#btnBulkEdit', function(){
		if ($("#SearchResultTotalItems").val() > 0) {
			var url = "<%= Url.ActionCache("BulkEdit","QuestionAdmin") %>";
			PxQuestionAdmin.OpenFNE(url);
		}
		else
			PxPage.Toasts.Error("Search Questions before Bulk Edit");
	});

</script>   