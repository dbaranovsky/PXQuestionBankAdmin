<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.QuestionAdminSearchPanel>" %>



   


<table id="QBA-QuestionList-Table" class="questions">
    
    
            <tr  class="headerForQuestionsResutl" >
                   <%--for QBA Bulk Edit operation requirement--%>
                            <% if (Model.BulkEdit == true) { %>
                                    <td class="chkboxSelection">
                                        <input type="checkbox" id='chkAll_Questions'  />
                                    </td>
                            <% } %>
                            
                            
                            <td class="Qstatus">Status</td>
							<td class="ExerciseNo">Exercise No</td>
				 			<td class="QuestionBank">Question Bank</td>
                            <td class="ebook">eBook Chapter</td>
                            <td class="QuestionTitle">Title</td>
                            <td  class="first" >Type</td>
						</tr>
    
    	
			<%
			  if (Model!=null && Model.Quiz!=null && Model.Quiz.Questions!=null && Model.Quiz.Questions.Count > 0)

               { 
				  %>               
  		  				  				  
			       <%                        
                   foreach (var question in Model.Quiz.Questions)
				   {
				   	string quizId = question.AssignedQuizes.ToArray()[0].Id;		
                    var questionType = question.PreviewText;

                    var Qstatus = string.Empty;
                    if (string.IsNullOrEmpty(question.QuestionStatus))
                        Qstatus = "";
                    else
                    {
                        switch (question.QuestionStatus)
                        {
                            case "0":
                                Qstatus = ExtensionMethods.GetEnumDescription(QuestionStatusType.InProgress);
                                break;
                            case "1":
                                Qstatus = ExtensionMethods.GetEnumDescription(QuestionStatusType.AvailabletoInstructor);
                                break;
                            case "2":
                                Qstatus = ExtensionMethods.GetEnumDescription(QuestionStatusType.Deleted);
                                break;
                        }
                    }
                       
                    if (question.Type.ToUpper() == "CUSTOM" && question.CustomUrl.ToUpper() == "HTS")
                        questionType = "Advanced Question";
                    else
                        if (question.Type.ToUpper() == "CUSTOM" && question.CustomUrl.ToUpper() == "FMA_GRAPH")
                        questionType = "Graph Exercise";
                        else
                        questionType = Question.QuestionType(question.Type);

                    var qID = question.Id;
                       
                       
                       
                       
                    %>                       
                        <tr data-qba-question-id="<%= question.Id %>" data-qba-quiz-id="<%= quizId %>" data-qba-chapter-id="<%= question.AssignedChapter %>" data-qba-entityid="<%= question.EntityId %>" class="<%=Qstatus.Replace(" ", string.Empty) %>">

                            <% if (Model.BulkEdit == true) { %>
                                <td class="chkboxSelection"> 
                                        <input type="checkbox" name='<%=quizId %>' value='<%= qID %>' class="bowQBA" />
                                </td>
                            <% } %>


                            <td class="Qstatus"><%= Qstatus %></td>
                            <td class="ExerciseNo"><%= question.ExcerciseNo %></td>
                            <td class="QuestionBank"><%= question.QuestionBank%></td>
                            <td class="ebook"><%= question.AssignedChapter%></td>
                            <td class="QuestionTitle">
                            <a class="EditorLink" ref="<%= Url.Action("QuestionEditor","QuestionAdmin", new { id= question.Id, quizId = quizId, chapter = question.AssignedChapter}) %>">
							<span class="text truncated">
                                    <%if (!question.Title.IsNullOrEmpty())
                                      { %>
                                          <b><%=question.Title%></b><br />
                                    <% }
                                      else
                                      { %>
                                            <b>No Title Specified</b><br />
                                       <%} %>
                                <%= string.IsNullOrEmpty(question.PreviewText) ? "This question has no data." : question.PreviewText %></span>							
                            </a>
                            </td>
                                
                        	<td class="first">
                                <%= questionType %>                            
                            </td>
                        
                        
                        <%-- 
                            <td class="second">
                            <a class="EditorLink" ref="<%= Url.Action("QuestionEditor","QuestionAdmin", new { id= question.Id, quizId }) %>">
							<span class="text truncated">
                            <%if (!question.Title.IsNullOrEmpty())
                              { %>
                              <b><%=question.Title %></b><br />
                            <% } %>
                            <%= string.IsNullOrEmpty(question.PreviewText) ? "This question has no data." : question.PreviewText %></span>							
                            </a> 
							 
						                         	
							<%	
							   Model.SelectedQuestionXml = question.QuestionXml;
							   XDocument data = Model.Serialize("root");
									 
							using (Ajax.BeginForm("QuestionEditor", "QuestionAdmin", new AjaxOptions { HttpMethod = "Post", UpdateTargetId = "MainBody", InsertionMode = InsertionMode.Replace }, new { @id = "frmQuestion" }))
							{
							%>

								<% = Html.HiddenFor(m => m.SelectedData, new {@id = "selectedData", @value = Html.Encode(data.ToString())}) %>
							<%
							}
			
						   %>   
							
						</td>     --%>
                        <td id="main-action-container">
                        <div class="duplicate-button-container">

                        <div id="duplicate-button">
                        <button id="gear-button" class="pxicon pxicon-gear"></button>
                        <ul id="menu-container">
                        <li class="action" data-gear-menu-action="duplicate-question" id="duplicate-action">Duplicate</li>
                        </ul>
                        </div>
                        </div>
                        </td>
                      </tr>
                <% 
                   }
               } 
               %>        
			

  </table>
   
<script type="text/javascript" language="javascript">
    jQuery(document).ready(function () {
        if (PxQuestionAdmin) {
            PxQuestionAdmin.CleanUpSearchResult();
        }
    } (jQuery));   
</script>