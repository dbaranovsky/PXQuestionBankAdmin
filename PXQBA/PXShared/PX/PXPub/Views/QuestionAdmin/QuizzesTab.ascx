<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.QuestionAdminSearchPanel>" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Bfw.Common.Pagination" %>

<div id="divQuizzesTab">
	  <%
		if (Model != null)
		{
			//show Pagination here:
				if (Model.Pagination != null && Model.Pagination.TotalItems > 0)
				{
				%>
					<div id="divQuizPagination" >
						<p></p>				  	
						<%: Html.Paging(Model.Pagination) %>
				   
						<%= Html.HiddenFor(model => model.NextPageStartSearchQuestion,
											   new
                                       			{
                                       				@id = "SearchResultNextPageStartSearchQuestion",
                                       				Value = Model.NextPageStartSearchQuestion
                                       			}) %> 
						<%= Html.HiddenFor(model => model.NextPageStartSearchQuiz,
											   new
                                       			{
                                       				@id = "SearchResultNextPageStartSearchQuiz",
                                       				Value = Model.NextPageStartSearchQuiz
                                       			}) %> 
						<p></p>	
					</div>				   
				<%
				}
				%>	
							
				<%
				//List Quizzes here:
				if (Model.QuizSelectList.Any())
				{
					string selectedQuestionId = Model.SelectedQuestionId;
					string selectedQuizId = Model.SelectedQuizId;
				%>
					<input type="hidden" id="SelectedQuestionId" value ="<% = selectedQuestionId%>" />
					
					<input type="hidden" id="SelectedQuizId" value ="<% = selectedQuizId%>" />

					<div class="question-bank-prompt" > <img src="<%= Url.Content("~/Content/images/informationBlue.png") %>" alt="" />

						 This question is included in the quizzes below. Click a search link to view all questions in the quiz record.
						 <p></p>
				   </div>
				   <% foreach (var quiz in Model.QuizSelectList)
					{
						string currentQuizId = quiz.Value; 
					  %>
						   <div class="queston-quiz-item">
		   					   <br/>		       
							   <b><% = quiz.Text %></b>	
			   				
		       					<span style="font: italic blue ">  See all questions in this quiz </span>	

								<input id="<%= currentQuizId %>" type="image" src="<%= Url.Content("~/Content/images/goBackArrowSmall.png") %>"  value="<% = currentQuizId %> " 
								onclick="PxQuestionAdmin.SubmitBackToIndexPage('<%= currentQuizId %>')" alt=""/>
						          						
						   </div>		  
				   <%
					}
					%>
			   <%
				}
			   %>		

     <%
		}
	 %>
	
	

</div>

