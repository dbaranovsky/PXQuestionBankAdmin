<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>
<%@ Import Namespace="Bfw.PX.PXPub.Controllers" %>

<% 
    bool allowSelection = (bool)ViewData["allowSelection"];
    bool allowDrag = (bool)ViewData["allowDrag"];
    bool showExpand = (bool)ViewData["showExpand"];
    bool showAddLink = (bool)ViewData["showAddLink"];
    bool showPoints = (bool)ViewData["showPoints"];
    string extraClass = (string)ViewData["extraClass"];
    var question = (Question)ViewData["question"]; 
    bool isOdd = (bool)ViewData["isOdd"];
    bool isReused = (bool)ViewData["isReused"];
    string questionEditedType = (string)ViewData["questionEditedType"];
    bool isPrimary = (bool)ViewData["isPrimary"];
    var mode = (QuizBrowserMode)ViewData["mode"];   
    bool isQuestionOverview = (bool)ViewData["isQuestionOverview"];
    string mainQuizId = (string)ViewData["mainQuizId"];
    bool isPoolQuestion = (bool)ViewData["isPoolQuestion"];
    string question_Status_visibility = "available2Instructor";
    var questionNumber = (string)ViewData["questionNumber"];
    //if (question.QuestionStatus !=null && (question.QuestionStatus == "0" || question.QuestionStatus == "2"))
    //    question_Status_visibility = "qstatus-hide";
 %>
 
<li class="question <%= extraClass %> <%= isOdd ? "odd" : "" %> <%= isReused ? "reused" : "" %> entityId-<%= question.EntityId %>  <%=question_Status_visibility%>" id="<%= question.Id %>">
    <input type="hidden" value="<%= question.Id %>" class="question-id" />
    <input type="hidden" class="question-type" value="<%= question.Type %>" />                            
    <input type="hidden" class="question-edited-type" value="<%= questionEditedType %>" /> 
    <input type="hidden" class="question-custom-url" value="<%= question.CustomUrl %>" />                             
    <input type="hidden" class="question-entity-id" value="<%= question.EntityId %>" />
    <input type="hidden" class="allowSelection" value="<%= allowSelection %>" />
    <input type="hidden" class="allowDrag" value="<%= allowDrag %>" />  
    <input type="hidden" class="showExpand" value="<%= showExpand %>" />  
    <input type="hidden" class="showAddLink" value="<%= showAddLink %>" />  
    <input type="hidden" class="showPoints" value="<%= showPoints %>" />  
    <input type="hidden" class="extraClass" value="<%= extraClass %>" />  
    <input type="hidden" class="isOdd" value="<%=isOdd %>" />  
    <input type="hidden" class="isReused" value="<%= isReused %>" />  
    <input type="hidden" class="isPrimary" value="<%= isPrimary %>" />   
    <input type="hidden" class="isQuestionOverview" value="<%= isQuestionOverview %>" />   
    <input type="hidden" class="mainQuizId" value="<%= mainQuizId %>" /> 
    <input type="hidden" class="mode" value="<%= mode %>" />   
    <input type="hidden" class="quizId" value="<%= Model.Id %>" />   
    <input type="hidden" class="isPoolQuestion" value="<%= isPoolQuestion %>" />  
    <input type="hidden" class="showReused" value="<%= Model.ShowReused %>" /> 
    <% if (allowSelection)
        { %>                 
        <div class="select">
            <input name="selected" type="checkbox" />
            <% if (allowDrag)
                { %>
                <a href="javascript:" class="drag-indicator"><div class="drag-indicator"></div></a>
            <% } %>
        </div>
    <% } %>
    <div class="description <%= question.Type.ToLower() %>">


           <%
                    string questionTitle = string.IsNullOrEmpty(question.Title) ? question.PreviewText : question.Title;
                    if (Model.Type == "CUSTOM" && questionTitle.IsNullOrEmpty())
                    {
                        questionTitle = "Advanced Question";
                    }
                    string formattedTitle = Regex.Replace(questionTitle, "<[^>]*(>|$)", string.Empty).Trim();
                %>
        <div class="title">
            <div class="expand-link">
                <span class="numbering">
                    <span class="number"><%=questionNumber%></span>
                    ) </span>
                <a class="expand-link" href="javascript:"><span class="text truncated"><% if (isPrimary)
                                                                                { %>
                    <img class="primaryMarker" src="<%= Url.Content("~/Content/images/fullStar.png") %>" alt="Primary question" />
                                                    <% } %><%= string.IsNullOrEmpty(formattedTitle)?"New Question" : formattedTitle %></span></a>

            </div>     
            <% string displayType = (question.Type == "CUSTOM") ? Question.QuestionTypeUrl(question.CustomUrl) : Question.QuestionType(question.Type); %> 
                                                        
        </div>
        
        <div class="points-container">
            <% if (question.Type == "BANK")
                { %>
                <% if (question.BankCount > 0)
                    { %>
                    <span class="bank-label randomselect">Randomly select <span class="bank-use"><%= question.BankUse %></span> of <span class="bank-count"><%= question.BankCount %></span> questions (<span class="bank-points"><%= question.Points.ToString()%> </span> <%=((question.Points == 1) ? " point" : " points")  %> each)</span>
                <% }
                    else
                    { %>
                    <span class="empty-pool">This question pool is empty.</span> 
                    <span class="bank-count" style="display: none;"><%= question.BankCount %></span>
            <% }
                } %>
               
          
                      
            <% if (showPoints)
                { %>
                <span class="points-line">
                                        
                    <a id="lnkPoints" class="link-points" href="javascript:">
                        <% if (question.Type.ToLowerInvariant() != "bank")
                           { %>
                            <span class="point-label">
                                <%if (!isPoolQuestion)
                                  { %>
                                    <%= String.Format("{0} {1}", question.Points, question.Points > 1 ? " pts" : " pt")%>
                                    <%}
                                  else
                                  { %> -- pt (s) <% } %>
                            </span>
                        <% } %>
                    </a>
                    <span class="point-textbox" style="display: none;"><input type="text" class="questions-points" value="<%= question.Points %>" /><input type="button" class="save-bank-points" value="Save" /> </span>                                       

                    
                       
                </span>
                
                

            <% } %>
            <div class="key-detail-metadata pool-question-type"><%= displayType %></div>

            <% 
               if (!Model.CourseInfo.QuestionFilter.FilterMetadata.IsNullOrEmpty()) {
                 foreach (var metadata in Model.CourseInfo.QuestionFilter.FilterMetadata.Where(f => f.Filterable))
                 {
                   if (question.SearchableMetaData != null && question.SearchableMetaData.ContainsKey(metadata.Name) && !question.SearchableMetaData[metadata.Name].IsNullOrEmpty())
                   {
                     var data = question.SearchableMetaData[metadata.Name];
            %>

                    <span class="metadata-label <%= data.Length > 20? "isLink": ""  %>" name="<%=metadata.Friendlyname%>" data="<%=data.Replace("|","<br />")%>"><%= data.Length > 20 || data.Contains("|") ? metadata.Friendlyname : data %></span>
            <%     }
                 }
               } 
            %>
            

        </div>
        <% if (Model.ShowReused)
                { %>
                <div class="question-edit-wrapper"> 
                    <% if (question.Type == "BANK")
                        { %>   
                        <a class="questionlist-itemmenu" href="javascript:"><span class="show-current-questions">Show questions</span></a>
                        <a class="questionlist-itemmenu" href="javascript:"><span class="hide-current-questions">Hide questions</span></a>
                    <% } %>
                    <%
                        else
                        {
                            if (showExpand)
                            { %>
                            <a class="questionlist-itemmenu" href="javascript:"><span class="expand-available-question">Expand</span></a>
                    <% }
                        } %>
                    <% if (question.Type != "BANK")
                        { %>   
                        <a class="questionlist-itemmenu" href="javascript:"><span class="preview-available-question">Preview</span></a>
                    <% } %>                                    
                    <% if (showAddLink)
                        { %>
                        <%--<a class="questionlist-itemmenu" href="#"><span class="add-available-question">Add</span></a>--%>
                        <a class="questionlist-itemmenu" href="javascript:"><div class="add-to-pool-available-question"><div class="gearbox">Add to this assessment</div></div></a>
                    <% } %>
                    <!-- TODO: When in Qestion Picker Mode, filter out the current editable quiz from the Assigned Quizzes list and change label accordingly -->
                    <% if (Model.ShowReused && question.AssignedQuizes.Count > 0 && question.AssignedQuizes.Any(q => q.Id != mainQuizId))
                        { %>
                        <a  class="questionlist-itemmenu" href="javascript:"><span class="question-in-use is-used"><%= mode == QuizBrowserMode.Resources ? "&#10004 In Use &#x25BC;" : "&#10004 Used Elsewhere &#x25BC;"%></span></a>
                    <% }
                        else
                        { %>
                        <a class="questionlist-itemmenu" href="javascript:" ><span class="question-in-use"><%= mode == QuizBrowserMode.Resources ? "Use &#x25BC;" : "Use Elsewhere &#x25BC;"%></span></a>

                    <% } %>


              
                </div>                                
            <% } %>
                <div style="display: none; height: 1px;">
                        <span id="question-tool-tip" >
                            <% ViewData["questionId"] = question.Id;
                               Html.RenderPartial("DisplayTemplates/QuizPartials/UsedElseWhere"); %>
                        </span>
                    </div>
            <% if (!Model.ShowReused)
                {
                    //bool isPrimary = false;
                    //if (!question.LearningCurveQuestionSettings.IsNullOrEmpty())
                    //{
                    //    isPrimary = (from c in question.LearningCurveQuestionSettings where c.QuizId == Model.Id select c.PrimaryQuestion).FirstOrDefault();
                    //}
                    //var visibilityClass = isPrimary ? "show-primary-icon" : "";
            %> 
                <%--</br>  </br>  --%>
                <% if (isQuestionOverview)
                    { %>
                    <div class="question-analysis-container">
                        <span class="question-analysis">
                            <% if (question.Analysis != null)
                                {
                                    if (question.Analysis.Attempts != null)
                                    {
                                        string submittedText = question.Analysis.Attempts > 1 ? "students submitted" : "student submitted";
                            %>
                                            
                                    <span class="attempt-analysis"> <%= question.Analysis.Attempts %> <%= submittedText %> </span>  
                                <% }
                                    if (question.Analysis.PercentCorrect != null)
                                    { %>
                                    <span class="attempt-analysis" title="Unsubmitted questions are not included in the average score">&nbsp; Avg. Score:
                                        <%= question.Analysis.PercentCorrect %>% 
                                    </span>
                                <% }
                                    if (question.Analysis.AverageTime != null)
                                    { %>
                                    <span class="attempt-analysis">&nbsp; Avg. time:
                                        <%= question.Analysis.AverageTime %>
                                    </span>
                            <% }
                                } %>
                            <%
                                else if (question.Type != "BANK")
                                { %>
                                        
                                <span>No. students submitted: 0, Avg. score: —, Avg. time: —</span>
                            <% } %>
                        </span>
                    </div>
                <% } %>

                <div class="question-edit-wrapper"> 
                    <% if (question.Type == "BANK")
                        { %>   
                        <a class="questionlist-itemmenu" href="javascript:"><span class="show-current-questions">Show questions</span></a>
                        <a class="questionlist-itemmenu" href="javascript:"><span class="hide-current-questions">Hide questions</span></a>
                    <% } %>
                    <% if (question.Type != "BANK")
                        { %>   
                        <% if (isQuestionOverview)
                            { %>
                            <a class="questionlist-itemmenu" href="javascript:"><span class="expand-current-question">Expand</span></a>
                        <% } %>
                        <a class="questionlist-itemmenu" href="javascript:"><span class="preview-current-question">Preview</span></a>
                    <% } %>
                    <% if (!isQuestionOverview)
                        { %>
                        <a class="questionlist-itemmenu" href="javascript:"><span class="edit-current-question">Edit</span></a>
                    <% } %>
                    <%
                        else
                        { %>
                        <% string linkUrl = Url.GetComponentHash("item", mainQuizId, new { id = mainQuizId, mode = ContentViewMode.Questions, renderFne = true }); %>
                        <a class="questionlist-itemmenu" href="<%= linkUrl %>"><span class="edit-current-question">Edit</span></a>
                    <% } %>
                    <a class="questionlist-itemmenu" href="javascript:"><div class="move-current-question"><div class="gearbox">Move</div></div></a>
                    <a class="questionlist-itemmenu" href="javascript:"><span class="delete-current-question">Remove</span></a>
                                    
                    <%--<a style="float:left;" href="#"><span class="primary-question"><div class="primary-question-image <%=visibilityClass %>" title="Primary Question"></div></span></a>--%>
                                     
                </div>                                
            <% } %>

                                        
            <div class="question-text collapsed preview">
                <div class="question-container">
                    <% if (question.Type != "BANK")

                       {
                           if (!string.IsNullOrEmpty(question.Title))
                           {
                    %> 
                            <span class="question-meta-title">

                                <%= question.Title.Trim() %>
                            </span>

                           <%
                           }
                           if (question.Type != "CUSTOM")
                           {
                           %>
                        
                        <span class="question-inner-title">

                            <%= Model.CourseInfo != null ?
                                    question.Text.Trim().Replace("[~]/", "/brainhoney/Resource/" + Model.CourseInfo.Id + "/[~]/") : question.Text.Trim() %>
                        </span>

                    <%      }
                            
                       } %>
                    <% if (question.Type == "MC")
                        {
                            var re = new Regex(@"\s+");
                            if (!question.Choices.IsNullOrEmpty())
                            {
                                string a = question.Answer != null ? re.Replace(question.Answer, "").ToLower() : "No answer provided"; %>
                            <ul>
                                <% foreach (QuestionChoice choice in question.Choices)
                                    {
                                        string c = re.Replace(choice.Text, "").ToLower();
                                        string cid = re.Replace(choice.Id, "").ToLower();
                                %>
                                    <li>
                                        <input disabled="disabled" type="radio" <%= (a == c || a == cid) ? "checked='checked'" : "" %> /><span
                                                                                                                                                class="option-text"><%= choice.Text %></span></li>
                                <% } %>
                            </ul>
                        <% }
                            else
                            { %>
                            This question has no options.
                    <% }
                        }
                        else if (question.Type == "A")
                        {
                            var re = new Regex(@"\s+");
                            string xmlDoc = question.QuestionXml;
                            var answers = new List<string>();
                            
                            if (xmlDoc != null)
                            {
                                if (xmlDoc.Contains("&nbsp;"))
                                    xmlDoc = xmlDoc.Replace("&nbsp;", " ");
                                XDocument xDoc = XDocument.Parse(xmlDoc);
                                XElement xml1 = xDoc.Element("question").Element("answer");
                                if (xml1 != null)
                                {
                                    foreach (XElement answerValue in xml1.Elements("value"))
                                    {
                                        answers.Add(answerValue.Value.ToLower());
                                    }    
                                }
                                
                            } %>
                        <%-- <span class="text"><%= question.Text%></span>--%>
                        <ul>
                            <% if (question.Choices != null)
                                {
                                    foreach (QuestionChoice choice in question.Choices)
                                    {
                                        string c = re.Replace(choice.Text, "").ToLower();
                                        string cid = re.Replace(choice.Id, "").ToLower();
                                        string answerChecked = answers.Contains(cid) ? "checked=checked" : "";
                            %>
                                    <li>
                                        <input disabled="disabled" type="checkbox" <%= answerChecked %>/>
                                        <span class="option-text"><%= choice.Text %></span></li>
                            <% }
                                } %>
                        </ul>

                    <%
                        }
                        else if (question.Type == "BANK")
                        { %>
                        <span style="display: none">Number of Items: <span>
                                                                            <%= question.BankUse %>
                                                                        </span>
                            <input type="text" class="questions-count" value="<%= question.BankUse %>" style="display: none" />
                            of <span class="bank-question-count">
                                    <%= question.BankCount %></span>
                            <br />
                            <input type="button" class="save-bank-count" value="Save" style="display: none" /></span>
                        <div class="question-pool">
                            <%
                            if (!question.Questions.IsNullOrEmpty())
                            {
                                   Html.RenderAction("QuestionList", "Quiz", new RouteValueDictionary(new {quizId = question.Id, mainQuizId, isQuestionOverview}));
                            } %>
                        </div>
                    <% }
                        else if (question.Type == "CUSTOM")
                        { %>
                        <div id="custom_preview_<%= question.Id %>" class="custom_preview custom_preview_<%= question.Id %> custom_inline_preview">
                            Generating preview...</div>
                    <% }
                        else if (question.Type == "E")
                        {
                            
                        }
                        else if (question.Type == "TXT")
                        {
                             string answer;
                            // Only display the answer. The question is already displayed above in
                            // if (question.Type != "BANK") ...
                            if (question.AnswerList != null && question.AnswerList.Count() > 1)
                            {
                                answer = string.Format("Correct answers: {0}", string.Join(", ", question.AnswerList));
                            }
                            else
                            {
                                answer = String.Format("Correct answer: {0}", question.Answer);
                            }
                            %>
                                <span class="text"><%= answer%></span>                                                                            
                            <%                                             
                        }
                        else if (question.Type == "MT")
                        {
                            string xmlDoc = question.QuestionXml;
                            var answers = new List<string>();
                            if (xmlDoc != null)
                            {
                                if (xmlDoc.Contains("&nbsp;"))
                                    xmlDoc = xmlDoc.Replace("&nbsp;", " ");
                                XDocument xDoc = XDocument.Parse(xmlDoc);
                                XElement xml1 = xDoc.Element("question").Element("interaction");
                                foreach (XElement answerChoice in xml1.Elements("choice"))
                                {
                                    answers.Add(answerChoice.Element("body").Value + " = " + answerChoice.Element("answer").Value);
                                }
                            }
                            else
                            {//When searching for questions from SOLR, we dont have access to the Question XML
                                if (!question.Choices.IsNullOrEmpty())
                                {
                                    foreach (var answerChoice in question.Choices)
                                    {
                                        answers.Add(answerChoice.Text);
                                    }
                                }
                            } %>
                            <%-- <span class="text"><%= question.Text%></span>--%>
                            <ul>
                                <% foreach (string answer in answers)
                                   {
                                %>
                                    <li>                                                            
                                        <span class="option-text"><%= answer %></span>                                                              
                                    </li>
                                <% } %>
                            </ul>
                            <% 
                        }
                        else
                        { %>
                        Can't show details for this type of question.
                    <%
                        }
                    %>
                </div>
                <div class="question-metadata" style="display: none;"><%= !question.SearchableMetaData.IsNullOrEmpty() ? @Html.Raw(Json.Encode(question.SearchableMetaData)) : MvcHtmlString.Empty %>
                </div>
               <div class="questioncard-container"></div>
            </div>
    </div>
    <div class="clear"></div>
</li>