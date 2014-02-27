<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.QuestionAdminSearchPanel>" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
 

<div id="SearchPanel">
    
<% 
    using (Ajax.BeginForm("SearchResult", "QuestionAdmin", new AjaxOptions { HttpMethod = "Post", UpdateTargetId = "SearchResult" , InsertionMode = InsertionMode.Replace, 
                                                                             OnSuccess = "PxQuestionAdmin.UnblockUI()",
                                                                             OnBegin = "PxQuestionAdmin.ValidateInput" }, new { @id = "frmDone" }))  
   { 
%>

    <table id="QBA-Search-Table" >
        <tr class="header">
            <td>                
                <input type="submit" value="Search" id="cmdSearch"/>&nbsp;&nbsp;
                
                <% 
                    var currentPage = 1;
                    var nextPageStartSearchQuestion=1;
                    var nextPageStartSearchQuiz=1;
                    var totalItems = 0;
                    
                    if (ViewData["TotalCount"]!=null) totalItems = Convert.ToInt32(ViewData["TotalCount"]);

                    if (Model!=null)
                    {
                        if(Model.Pagination!=null)
                        {
                            currentPage = Model.Pagination.CurrentPage;
                            nextPageStartSearchQuestion = Model.NextPageStartSearchQuestion;
                            nextPageStartSearchQuiz = Model.NextPageStartSearchQuiz;						
                        }
                    }				
                 %>
                <%= Html.HiddenFor(model => model.Pagination.TotalItems, new { @id="SearchPanelTotalItems", Value= totalItems } )  %> 
                <%= Html.HiddenFor(model => model.Pagination.CurrentPage, new { @id="CurrentPage", Value=currentPage } )  %> 
                <%= Html.HiddenFor(model => model.NextPageStartSearchQuestion, new { @id="SearchPanelNextPageStartSearchQuestion", Value=nextPageStartSearchQuestion } )  %> 
                <%= Html.HiddenFor(model => model.NextPageStartSearchQuiz, new { @id="SearchPanelNextPageStartSearchQuiz", Value=nextPageStartSearchQuiz } )  %> 

                <a href="#" id="lnkClearSearch" class="link" >Clear</a>

            </td>            
        </tr>
        <tr class="body">
            <td>          	
                 <%= Html.TextBoxFor(model => model.SearchKeyword, new { @id = "txtSearchKeyword",  @autocomplete = "true", @class = "Search-Textbox", @readonly = "readonly" })%>               
           </td>
        </tr>
        <tr class="body">
            <td>
                <span class="input-label">Response Format</span>
                <br />
                <%--<%= Html.DropDownListFor(model => model.FormatSelectedValues, new SelectList(Model.FormatSelectList, "Value", "Text"), new { @id = "FormatList", @multiple = "multiple", @class = "Format-Listbox" })%>--%>

                <%--<%= Html.CheckBox("FormatSelectedValues", item.Text == "Any" ? true : false, new { Value = item.Value }) + item.Text%>  --%>
                <% foreach (var item in Model.FormatSelectList) 
                   { %>
                        <div>
                            <% if (item.Text == "Any")
                               { %>
                                <input type="checkbox" value="<%= Html.Encode(item.Value)%>" name="FormatSelectedValues" checked="checked"/><span><%= item.Text %></span> 
                            <% }  %>
                            <% else
                               {%>
                                <input type="checkbox" value="<%= Html.Encode(item.Value)%>" name="FormatSelectedValues" /> <span><%= item.Text %></span>
                            <%  
                               } %>
                        </div>
                        <% } %>
            </td>
        </tr>
         <%--<tr class="body">
            <td>
                <span class="input-label">Status</span>
                <br /> 							 
                <% = Html.DropDownListFor(model => model.StatusSelectedValues, new SelectList(Model.StatusSelectList, "Value", "Text"), new { @id = "StatusList", @class = "Status-Listbox"})%>                                
                        
            </td>
        </tr>--%>


         <tr class="body">
            <td>
                <span class="input-label">Flagged</span>
                <br /> 							 
                <div id="QBA_question_type">
                    <input type="radio" id="FlaggedState_Any"  value="0" name="FlagSelectedValues" checked="checked" /><label for="FlaggedState_Any">Any</label>
                    <input type="radio" id="FlaggedState_Flagged"  value="1" name="FlagSelectedValues" /><label for="FlaggedState_Flagged">Flagged</label>
                    <input type="radio" id="FlaggedState_NotFlagged" value="2" name="FlagSelectedValues" /><label for="FlaggedState_NotFlagged">Not Flagged</label>
                </div>
                        
            </td>
        </tr>
                 <tr class="body">
            <td>
                <span class="input-label">Status</span>
                <br />
                <div id="QBA_question_status">
                    <ul id="question-status-container">
                        <li>
                            <input type="checkbox" id="Available_to_Instructor" name="StatusSelectedValues" value="1"/><label for="Available_to_Instructor">Available
                                to Instructor</label>
                        </li>
                        <li>
                            <input type="checkbox" id="In_Progress" name="StatusSelectedValues" value="0"/><label for="In_Progress">In Progress</label>
                        </li>
                        <li>
                            <input type="checkbox" id="Deleted" name="StatusSelectedValues" value="2"/><label for="Deleted">Deleted</label>
                        </li>
                    </ul>
                </div>
                        
            </td>
        </tr>
         <tr class="body">
            <td>
                <span class="input-label">eBook Chapter</span>
                <br />
                <div>
                    <%= Html.DropDownListFor(model => model.ChapterSelectedValues, new SelectList(Model.ChapterSelectList, "Value", "Text"), new { @id = "ChapterList_new", @multiple = "multiple", @class = "Chapter-Listbox", @default = "0" })%>
                </div>
            </td>
        </tr>

        <tr class="body">
                <td>
                <span class="input-label">Question Bank</span>
                <br />	
                            
                    <div id="divQuizSelectList_new" >													
                        <% 	
                            //ViewData["ControlName"] = "QuizList_new";
                            Html.RenderPartial("QuizListDropDown", Model); 					
                        %>
                    </div>
            </td>
        </tr>

        
         <%--<tr style="background-color:Gray">
            <td>
                    <span > OLD Style Controls	</span> <br />
                    <br />
            </td>
        </tr>

        <tr class="body">
            <td>
                <span class="input-label">Response Format</span>
                <br />
                <%= Html.DropDownListFor(model => model.FormatSelectedValues, new SelectList(Model.FormatSelectList, "Value", "Text"), new { @id = "FormatList", @multiple = "multiple", @class = "Format-Listbox" })%>
            </td>
        </tr>
        <tr class="body">
            <td>
                <span class="input-label">eBook Chapter -- OLD --</span>
                <br />
                <%= Html.DropDownListFor(model => model.ChapterSelectedValues, new SelectList(Model.ChapterSelectList, "Value", "Text"), new { @id = "ChapterList", @multiple = "multiple", @class = "Chapter-Listbox" , @default="0"})%>                                
            </td>
        </tr>
        <tr class="body">
            <td>
                <span class="input-label">Question Bank -- OLD --</span>
                <br />	
                            
                <div id="divQuizSelectList" >													
                    <% 	
                        ViewData["ControlName"] = "QuizList";
                        Html.RenderPartial("QuizListDropDown", Model); 					
                    %>
                </div>

            </td>
        </tr>--%>
        
        
       

    </table>

<%
   }
%>

</div>

<script type="text/javascript">

    $(document).ready(function () {
        $("#QBA_question_type").buttonset();
        $("#ChapterList_new").select2({ placeholder: "Search / Browse Chapters", allowClear: true });
        $("#QuizList").select2({ placeholder: "Search / Browse Question Bank", allowClear: true });
    });
</script> 

