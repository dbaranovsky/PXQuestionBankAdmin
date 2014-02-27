<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.QuestionAdminSearchPanel>" %>
 
 <% 
	 if (Model != null && Model.QuizSelectList != null)
	 {
         var controlID = "QuizList";
         //if ( ViewData["ControlName"] != null && !string.IsNullOrEmpty(ViewData["ControlName"].ToString()))
         // controlID = ViewData["ControlName"].ToString();
         
			if ( Model.QuizSelectedValues != null)
			{
		    %>		
				<% = Html.ListBoxFor(model => model.QuizSelectedValues,
												   new MultiSelectList(Model.QuizSelectList, "Value", "Text",
																	   Model.QuizSelectedValues.ToArray()),
                                                                   new { @id = controlID, @multiple = "multiple", @class = "Quiz-Listbox" })%>                                
		    <%
			}
			else if ( Model.ChapterSelectedValues != null)
			{
	        %>				
				<%--<% = Html.ListBoxFor(model => model.QuizSelectedValues,
												   new MultiSelectList(Model.QuizSelectList, "Value", "Text",
															   Model.QuizSelectList.Select(i => i.Value != "0" ? i.Value : null).ToArray()),
                                                                   new { @id = controlID, @multiple = "multiple", @class = "Quiz-Listbox" })%>          
                                                                   --%>
                    
                    <% = Html.DropDownListFor(model => model.QuizSelectedValues,
												   new SelectList(Model.QuizSelectList, "Value", "Text"),
                                                                   new { @id = controlID, @multiple = "multiple", @class = "Quiz-Listbox" })%>     
                    
                    
                                                                                         
		    <%
			}
			else 
			{
	        %>		
				<% = Html.DropDownListFor(model => model.QuizSelectedValues,
														new SelectList(Model.QuizSelectList, "Value", "Text"),
                                                                        new { @id = controlID, @multiple = "multiple", @class = "Quiz-Listbox" })%>                                
	        <%
			}
	 }
%>