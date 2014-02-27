<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>
<div class="question-display quiz" id="<%= Model.Id %>">
    <% if (!Model.Questions.IsNullOrEmpty())
       { %>
    <table class="table-homework">
        <thead>
            <tr>
                <th class="homework-question-title">
                    Question Title
                </th>
                <th class="homework-question-attempts">
                    Attempts(click to review)
                </th>
                <th class="homework-question-points">
                    Points
                </th>
                <th class="homework-question-show">
                </th>
            </tr>
        </thead>
        <tbody>
            
        <% 
           var fneClasses = "fne-link";
           if (Model.CourseInfo.CourseType == CourseType.FACEPLATE)
           {
               fneClasses += " faceplatefne ";
           }
            
            
           var ordinal = 0;
           int questionNumber = 0;
           foreach (var question in Model.Questions)
           {
               IList<QuestionAttempt> qa;
               SubmissionAttempt submissionAttempt;
               var attempts = 0;
               string attemptDetails = "";
               var totalAttempts = 0;
               double pointsEarned = 0;

               string attemptText = string.Empty;
               bool showButton = !(Model.DueDate < DateTime.Now);
               string buttonText = "Attempt";               
               questionNumber++;
                %>
                <tr class="homework-question-row">
                    <td class="homework-question-title"><%= questionNumber%>) <%= question.Text %> </td>
                    <td class="homework-question-attempts"> 

                        <% if (question.Type.ToUpper() != "BANK")
                           {
                                
                                    if (Model.QuestionAttempts != null && Model.QuestionAttempts.TryGetValue(question.Id, out qa))
                                    {
                                        foreach (var qr in qa)
                                        {
                                            if (qr.PointsComputed != null && qr.PointsPossible != null)
                                            {
                                                attemptDetails = qr.PointsComputed + "/" + qr.PointsPossible ;
                                                double d1 = 0;
                                                Double.TryParse(qr.PointsComputed, out d1);
                                                if (d1 > pointsEarned)
                                                {
                                                    pointsEarned = d1;
                                                }
                                                attempts++;
                                                %>
                                                <%= Html.ActionLink(attemptDetails, "SubmissionHistory", "Quiz", new { quizId = Model.Id, version = qr.PartId }, new { @class = fneClasses })%>
                                                <%                                                
                                            }                                            
                                        }                                        
                                %>     
                        <div class="attempts-remaining">
                                <%}

                               if (question.Attempts != null && question.Attempts != "0")
                               {
                                   if (int.TryParse(question.Attempts, out totalAttempts))
                                   {
                                       int attemptsRemain = totalAttempts - (int)attempts;
                                       if (attemptsRemain == 0)
                                       {
                                           attemptText = string.Format("No attempts remain");
                                           showButton = false;
                                       }
                                       else if (attemptsRemain == 1)
                                       {
                                           attemptText = string.Format("{0} attempt remains", attemptsRemain);
                                       }
                                       else
                                       {
                                           attemptText = string.Format("{0} attempts remain", attemptsRemain);
                                       }
                                   }
                               }
                               else if (Model.AttemptLimit > 0 && question.Attempts != "0")
                               {
                                   int attemptsRemain = Model.AttemptLimit - (int)attempts;
                                   if (attemptsRemain == 0)
                                   {
                                       attemptText = string.Format("No attempts remain");
                                       showButton = false;
                                   }
                                   else
                                   {
                                       attemptText = string.Format("{0} attempts remain", attemptsRemain);

                                       if (Model.SubmissionAttempts != null && Model.SubmissionAttempts.TryGetValue(question.Id, out submissionAttempt))
                                       {
                                           if (!(submissionAttempt.LastSave.IsNullOrEmpty() || (submissionAttempt.ToContinue == false)))
                                           {
                                               buttonText = "Resume";
                                           }

                                       }
                                       if (attemptsRemain == 0)
                                       {
                                           showButton = false;
                                       }
                                   }
                               }
                               else
                               {
                                   attemptText = "Unlimited attempts";
                               }
                                  
                                  %>                      
                                <%= attemptText%>
                        <%} %>
                        </div>
                    </td>
                    <td class="homework-question-points"><%= string.Format("{0:0}", pointsEarned)%> / <%= string.Format("{0:0} {1}", question.Points, question.Points > 1 ? " pts" : " pt") %> </td>                       
                    <td class="homework-question-show">   
                        <%if(showButton)
                              
                        {
                            fneClasses += " linkButton attempt-homework";%>                                

                            <%= Html.ActionLink(buttonText, "ShowQuiz", "Quiz", new { enrollmentId = Model.EnrollmentId, id = Model.Id, ordinal = ordinal }, new { @class = fneClasses, title = Model.Title })%>
                        <%}%>
                    </td>                    
      
               </tr>
                <% 
                ++ordinal;
            } %>
           
            <%                        
            //for pools the question attempts should be shown in different ways..
            var containsBank = from c in Model.Questions where c.Type.ToUpper() == "BANK" select c;
            Dictionary<int, int> QuestionsAttempted = new Dictionary<int, int>();
            if (containsBank.Count() > 0)
            {
                if (Model.QuestionAttempts != null)
                {
                    foreach (KeyValuePair<string, IList<QuestionAttempt>> questionAttempts in Model.QuestionAttempts)
                    {
                        int attemptedTimes = questionAttempts.Value.Count();
                        if (QuestionsAttempted.ContainsKey(attemptedTimes))
                        {
                            QuestionsAttempted[attemptedTimes] = QuestionsAttempted[attemptedTimes] + 1;
                        }
                        else
                        {
                            QuestionsAttempted.Add(attemptedTimes, attemptedTimes);
                        }
                    }
                }
            }
            if (QuestionsAttempted.Count > 0)
            {
            %>
   <tr> <td>
            <ul>
               <li> <span>Question Pools Attempted Questions:</span></li>
                <%
            foreach (KeyValuePair<int, int> keypair in QuestionsAttempted)
            {
                %>
                <li>
                    <%=keypair.Key%>
                    questions are attempted
                    <%= keypair.Value %> 
                    times. </li>
                <%} %>
            </ul> 
            </td></tr>            
    <%         
        }
       } %>
         </tbody>
    </table>
</div>
