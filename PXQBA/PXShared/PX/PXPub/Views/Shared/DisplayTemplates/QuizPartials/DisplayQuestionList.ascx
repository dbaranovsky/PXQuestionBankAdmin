<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>


<span style="display: none;" class="QuizType">
    <%=Model.QuizType%></span>
<div class="question-list"  id="questions-<%=Model.Id %>" style="display: none">
    <% Html.RenderAction("QuizTemplateId", "Quiz"); %>
    <div id="show-question" class="question-dialog-text"></div>
    <div class="question-container">
        <h2 class="quiz-title">
            <%= Model.Title %></h2>
        <% if (Model.QuizPaging != null)
           {
               string startIndex = null;
               string lastIndex = null;
               startIndex = Model.QuizPaging.StartIndex ?? null;
               lastIndex = Model.QuizPaging.LastIndex ?? null;
        %>
            <%= Html.HiddenFor(m => m.QuizPaging.StartIndex) %>
            <%= Html.HiddenFor(m => m.QuizPaging.TotalCount) %>
        <% } %>
        <ul class="questions table-layout">
            
            <% if (!Model.Questions.IsNullOrEmpty())
               {
                   var isOdd = true;
                   var isReused = false;

                   foreach (var question in Model.Questions)
                   {
                       isReused = false;
                       var extraClass = question.Type.ToLower();
                       if (extraClass == "bank")
                       {
                           extraClass += " quiz";
                       }
                       if (Model.ShowReused && question.AssignedQuizes.Count > 0)
                       {
                           isReused = true;
                       }
                      
                       var attempts = question.Attempts != null ? question.Attempts : Model.AttemptLimit.ToString();
                       var attemptsText = attempts == "0" ? "Unlimited" : attempts;
                       var review = question.Review != null ? question.Review : "Off";
                       var scrambled = question.Scrambled != null ? question.Scrambled : "Not";
                       var hints = question.Hints != null ? question.Hints : "Off";
                       var score = question.Score != null ? question.Score : Model.SubmissionGradeAction;
                       var timelimit = question.TimeLimit != null ? question.TimeLimit : Model.TimeLimit.ToString();
                       var timelimitText = timelimit == "0" ? "Unlimited" : timelimit + ":00";
                       var displayType = (question.Type == "CUSTOM") ? question.CustomUrl : question.Type;
                       
            %>
                    <li class="question row-layout table-row <%= extraClass %> <%= isOdd ? "odd" : "" %> <%= isReused ? "reused" : "" %> entityId-<%= question.EntityId %>"
                        id="<%= question.Id %>" px-question-type="<%= question.Type %>">
                        <div class="main-question-wrapper">
                            <input type="hidden" value="<%= question.Id %>" class="question-id" />
                            <input type="hidden" class="question-type" value="<%= question.Type %>" />
                            <input type="hidden" class="question-custom-url" value="<%= question.CustomUrl %>" />
                            <input type="hidden" class="question-entity-id" value="<%= question.EntityId %>" />         
                            <div class="select">
                                <input name="selected" type="checkbox" />
                                <%--<a href="#" class="drag-indicator"><div class="drag-indicator"></div></a>--%>
                            </div>
                            <div class="description <%= question.Type.ToLower() %>">
                                <%
                                    var questionTitle = string.IsNullOrEmpty(question.Title) ? question.PreviewText : question.Title;
                                    if (question.Type == "CUSTOM" && questionTitle.IsNullOrEmpty())
                                    {
                                        questionTitle = "Advanced Question";
                                    }
                                %>
                                <div class="expandsection title">
                                    <a class="expand-link" href="#">
                                        <span class="numbering">
                                            <span class="number"></span>
                                        )</span>
                                        <span class="text truncated"><%= questionTitle %></span>
                                    </a>
                                </div>
                                <div>
                            <% if (question.Type == "BANK")
                                {
                                    if (question.BankCount > 0)
                                    { %>
                                    <span class="bank-label randomselect">Randomly select <span class="bank-use"><%= question.BankUse%></span> of <span class="bank-count"><%= question.BankCount%></span> questions</span>
                                <%  }
                                    else
                                    { %>
                                    <span class="empty-pool">This question pool is empty.</span> 
                                    <span class="bank-count" style="display: none"> <%= question.BankCount%></span>
                                <%  }
                                } %>                            
                            
                                </div>
                                <div class="question-analysis-container">
                                    <span class="question-analysis">
                                        <% 
                                            if (question.Analysis != null)
                                            {
                                                if (question.Analysis.Attempts != null)
                                                {
                                                    var submittedText = question.Analysis.Attempts > 1 ? "students submitted" : "student submitted";
                                        %>
                                        <span class="attempt-analysis"> <%= question.Analysis.Attempts%> <%= submittedText%> </span>  
                                        <%
                                                }
                                                if (question.Analysis.PercentCorrect != null)
                                                { %>
                                                <span class="attempt-analysis" title="Unsubmitted questions are not included in the average score">&nbsp; Avg. Score:
                                                    <%= question.Analysis.PercentCorrect%>% 
                                                </span>
                                            <%  }
                                                if (question.Analysis.AverageTime != null)
                                                { %>
                                                <span class="attempt-analysis">&nbsp; Avg. time:
                                                    <%= question.Analysis.AverageTime%>
                                                </span>
                                            <%  }
                                            }
                                            else if (question.Type != "BANK")
                                            { %>
                                        
                                            <span>No. students submitted: 0, Avg. score: —, Avg. time: —</span>
                                        <%  } %>
                                    </span>
                                </div>
                                <div class="question-edit-wrapper"> 
                                    <% if (question.Type == "BANK")
                                       {%>   
                                        <a style="float:left;" href="#"><span class="show-current-questions">Show questions</span></a>
                                        <a style="float:left;" href="#"><span class="hide-current-questions">Hide questions</span></a>
                                    <%} 
                                      else
                                      { %>                                     
                                        <a style="float:left;" href="#"><span class="expand-current-question">Expand</span></a>
                                        <a style="float:left;" href="#"><span class="preview-current-question">Preview</span></a>

                                        <div style="display: none; height: 1px;">
                                            <span id="question-tool-tip" >
                                                    <% ViewData["questionId"] = question.Id;
                                                    Html.RenderPartial("DisplayTemplates/QuizPartials/UsedElseWhere"); %> 

                                            </span>
                                        </div>
                                    <%
                                    } 
                                     var linkUrl = Url.GetComponentHash("item", Model.Id, new { mode = ContentViewMode.Questions, renderFne  = true }); 
                                     %>
                                    <a style="float:left;" href="<%= linkUrl %>"><span class="edit-current-question">Edit</span></a>
                                    <a style="float:left;" href="#"><span class="delete-current-question">Remove</span></a>
                                </div>   
                                <div class="question-text collapsed preview">
                                    <div class="question-container">
                                        <% 
                                        if (question.Type != "BANK")
                                        {
                                           if(!string.IsNullOrEmpty(question.Title))
                                           {
                                           %> 
                                            <span class="question-meta-title">

                                              <%=question.Title.Trim() %>

                                          </span>

                                          <%}
                                           if (question.Type != "CUSTOM")
                                           {
                                          %>
                                            
                                            <span class="question-inner-title">
                                                <%= Model.CourseInfo != null ?
                                                        question.Text.Trim().Replace("[~]/", "/brainhoney/Resource/" + Model.CourseInfo.Id + "/[~]/") : question.Text.Trim() %>
                                            </span>
                                      <%
                                           }
                                        }
                                        if (question.Type == "MC")
                                        {
                                            var re = new Regex(@"\s+");
                                            if (!question.Choices.IsNullOrEmpty())
                                            {
                                                var a = question.Answer != null ? re.Replace(question.Answer, "").ToLower() : "No answer provided"; %>
                                            <ul>
                                             <% 
                                                foreach (var choice in question.Choices)
                                                {
                                                    var c = re.Replace(choice.Text, "").ToLower();
                                                    var cid = re.Replace(choice.Id, "").ToLower();
                                            %>
                                                <li>
                                                    <input disabled="disabled" type="radio" <%= (a == c || a == cid) ? "checked='checked'" : "" %> />
                                                    <span class="option-text"><%= choice.Text %></span>
                                                </li>
                                             <% } %>
                                            </ul>
                                         <%}
                                            else
                                            { %>
                                                This question has no options.
                                         <%}
                                        }
                                        else if (question.Type == "A")
                                        {
                                            var re = new Regex(@"\s+");
                                            string xmlDoc = question.QuestionXml;
                                            List<string> answers = new List<string>();
                                            if (xmlDoc.Contains("&nbsp;"))
                                                xmlDoc = xmlDoc.Replace("&nbsp;", " ");
                                            XDocument xDoc = XDocument.Parse(xmlDoc);
                                            XElement xml1 = xDoc.Element("question").Element("answer");
                                            foreach (var answerValue in xml1.Elements("value"))
                                            {
                                                answers.Add(answerValue.Value.ToLower());
                                            } %>
                                            <ul>
                                            <% if (question.Choices != null)
                                                {
                                                    foreach (var choice in question.Choices)
                                                    {
                                                        var c = re.Replace(choice.Text, "").ToLower();
                                                        var cid = re.Replace(choice.Id, "").ToLower();
                                                        var answerChecked = answers.Contains(cid) ? "checked=checked" : "";                                         
                                            %>
                                                    <li>
                                                        <input disabled="disabled" type="checkbox" <%=answerChecked%>/>
                                                        <span class="option-text"><%= choice.Text%></span></li>
                                            <%      }
                                                }%>
                                            </ul>

                                        <%
                                        }
                                        else if (question.Type == "BANK")
                                        { %>
                                            <span style="display: none;">Number of Items: <span>
                                                                                             <%= question.BankUse %>
                                                                                         </span>
                                                <input type="text" class="questions-count" value="<%= question.BankUse %>" style="display: none" />
                                                of <span class="bank-question-count">
                                                       <%= question.BankCount %></span>
                                                <br />
                                                <input type="button" class="save-bank-count" value="Save" style="display: none" />
                                            </span>
                                        <% 
                                        }
                                        else if (question.Type == "CUSTOM")
                                        {
                                            string qTitle = string.IsNullOrEmpty(question.Title) ? (string.IsNullOrEmpty(question.Text) ? "Advanced Question" : question.Text) : question.Title;
                                        %>
                                            <span class="text"><%if (question.CustomUrl != "HTS")
                                                                 {%> <%= qTitle%> <%} %></span>     
                                            <div id="custom_preview_<%= question.Id %>" class="custom_preview custom_preview_<%= question.Id %> custom_inline_preview">
                                                Generating preview...</div>
                                        <% 
                                        }   
                                        else if (question.Type == "TXT")
                                        {
                                            string answer ;
                                            // Only display the answer. The question is already displayed above in
                                            // if (question.Type != "BANK") ...
                                            if (question.AnswerList != null && question.AnswerList.Count() > 1)
                                            {
                                                answer = string.Format("Correct answers: {0}", string.Join(", ",question.AnswerList));
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
                                            List<string> answers = new List<string>();
                                            if (xmlDoc.Contains("&nbsp;"))
                                                xmlDoc = xmlDoc.Replace("&nbsp;", " ");
                                            XDocument xDoc = XDocument.Parse(xmlDoc);
                                            XElement xml1 = xDoc.Element("question").Element("interaction");
                                            foreach (var answerChoice in xml1.Elements("choice"))
                                            {
                                                answers.Add(answerChoice.Element("body").Value + " = " + answerChoice.Element("answer").Value);

                                            } %>
                                                <ul>
                                                    <% foreach (var answer in answers)
                                                        {                                                                                                  
                                                    %>
                                                        <li>                                                            
                                                            <span class="option-text"><%= answer%></span>                                                              
                                                        </li>
                                                    <% } %>
                                                </ul>

                                        <% 
                                        }
                                        else if (question.Type == "E")
                                        { }
                                        else
                                        { %>
                                            Can't show details for this type of question.
                                        <%         
                                        }    
                                        %>
                                    </div>
                                    <div class="question-metadata" style="display: none;"><%= @Html.Raw(Json.Encode(question.SearchableMetaData))%></div>
                                    <div class="questioncard-container"></div>
                                </div>                             
                            </div>
                            
                            <div class="question-total-wrapper">
                                <!-- Code For Type -->
                                <div class="question-type">
                                    <%= (question.Type == "CUSTOM") ? Question.QuestionTypeUrl(question.CustomUrl) : Question.QuestionType(question.Type) %>
                                </div>
                                <!-- Code For Points-->
                                <div class="total-points">
                                    <a style="float:left;" href="#">
                                        <span class="point-label">
                                            <%= String.Format("{0} {1}", question.Points, question.Points > 1 ? " pts" : " pt")%>
                                        </span>
                                    </a>
                                    <span class="default-point-label" style="display: none;">--</span>
                                    <span class="point-textbox" style="display: none;">
                                        <input type="text" class="questions-points" value="<%= question.Points %>" />                                
                                        <input type="hidden" class="questions-points-original" value="<%= question.Points %>" />  
                                    </span>
                                </div>
                                <!-- Code For Attempts-->
                                <% if (Model.QuizType == QuizType.Homework)
                                   {
                                %>
                                    <div class="total-attempts">
                                        <a id="lnkAttempt" class="link-attempt" href="#">
                                            <span class="attempt-label">
                                                <%if (attemptsText.ToLowerInvariant() != "unlimited")
                                                  { %>
                                                    <%= String.Format("{0} {1}", attemptsText, Convert.ToInt32(attemptsText) > 1 ? " attempts" : " attempt")%>
                                                <%} %>
                                                <%else if (attemptsText.ToLowerInvariant() == "unlimited")
                                                  { %>
                                                    <%= String.Format("{0} attempts", attemptsText)%>
                                                <%} %>
                                            </span> 
                                        </a>
                                        <span class="default-attempt-analysis" style="display: none;">-- </span>
                                        <span class="attempt-textbox" style="display: none;">
                                            <%--<input type="text" class="questions-attempts" value="<%=attemptsText %>" />--%>                                
                                            <select name="questions-attempts" class="questions-attempts">
                                                <% if (Model != null)
                                                   {
                                                       for (int i = -1; i <= 10; i++)
                                                       {
                                                           var isSelected = Convert.ToInt32(attempts) == i;
                                                           var showThisOption = i > -1 || isSelected;
               
                                                           if (showThisOption)
                                                           {
                                                               var text = (i == 0) ? "Unlimited" : (i == -1) ? "" : i.ToString();
                                                %>
                                                            <option value="<%= i %>" <% if(isSelected) { %>selected="selected"<% } %> ><%= text %></option>
                                                <% 
                                                            }
                                                        }
                                                    } %>
                                            </select>
                                        </span>
                                    </div>
                                <%
                                    }                                                                
                                %>

                            </div>
                    
                            <div class="clear">
                            </div>
                        </div>
                        <div class="question-pool-questions">
                            <div class="question-pool">
                            </div>
                        </div>
                    </li>
            <% isOdd = !isOdd;
                   }
               } %>
        </ul>
        <div class="clear">
        </div>       
    </div>
</div>
<div id="edit-settings-dialog">
</div>
<div id="edit-settings-dialog2">
</div>
<% if ((Model.CourseInfo != null))
        { %>
<div id="question-card-template" style="display:none;"><div class="question-card-layout"><% = Model.CourseInfo.QuestionCardLayout %></div></div>
    <% } %>



       <script type="text/javascript">
           PxPage.OnReady(function() {
               PxPage.Require(<%= ResourceEngine.JsonFor("~/Scripts/quiz.js") %>, function () {
                   PxQuiz.Init("questions-<%=Model.Id %>");
               });
           });

       </script>
