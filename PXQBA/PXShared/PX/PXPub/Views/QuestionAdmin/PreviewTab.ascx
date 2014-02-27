<%@ Control Language="C#"   Inherits="System.Web.Mvc.ViewUserControl<Question>" %>
<%@ Import Namespace="System.Diagnostics" %>
<table id="QBA-Preview-Table">
<tr class="main">
<td id="questionContents" class="first">
<div id="PreviewTab">
   <% 
       if (Model.Type.ToLower() == "custom")
        {
            Html.RenderAction("GetCustomInlinePreviewGeneric", "Quiz", new { entityId = Model.EntityId, questionId = Model.Id, customUrl = Model.CustomUrl });
        }
        else
        {
   %>
     
     <div class="greybackground"> 
        <input type="button" class="DonePreview QBA-button" value="Done"/>
        <span>Question Preview Mode</span>
     </div>
     <br />

        <div class="question-text collapsed preview">
        <% var question = Model; %>
                                    <% if (question.Type != "BANK")
                                       {
                                           if(!string.IsNullOrEmpty(question.Title))
                                           {
                                            %> 
                                            <span class="question-meta-title">
                                                <%=question.Title.Trim() %>
                                            </span>
                                           <%}%>
                                            <span class="question-inner-title">
                                                <%=question.Text.Trim()%>
                                            </span>
                                    <%} %>
                                <% if (question.Type == "MC")
                                   {
                                       var re = new Regex(@"\s+");
                                       if (!question.Choices.IsNullOrEmpty())
                                       {
                                            var a = question.Answer != null ? re.Replace(question.Answer, "").ToLower() : "No answer provided"; %>
                                            <ul>
                                                <% foreach (var choice in question.Choices)
                                                   {
                                                       var c = re.Replace(choice.Text, "").ToLower();
                                                       var cid = re.Replace(choice.Id, "").ToLower();
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
                                                List<string> answers = new List<string>();
                                                if (xmlDoc.Contains("&nbsp;"))
                                                    xmlDoc = xmlDoc.Replace("&nbsp;", " ");
                                                XDocument xDoc = XDocument.Parse(xmlDoc);
                                                XElement xml1 = xDoc.Element("question").Element("answer");
                                                foreach (var answerValue in xml1.Elements("value"))
                                                {
                                                    answers.Add(answerValue.Value.ToLower());
                                                } %>
                                               <%-- <span class="text"><%= question.Text%></span>--%>
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
                                                        <% }
                                                       }%>
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
                                        <div class="question-pool"></div>
                                <% }
                                   else if (question.Type == "CUSTOM")
                                   { %>
                                        <div id="custom_preview_<%= question.Id %>">
                                        Generating preview...</div>
                                <% }                                   
                                   else if (question.Type == "E")
                                   {
                                       var questionText = question.Text;
                                        %>
                                            <span class="text"><%= questionText%></span>                                                                            
                                        <%                                               
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
                                        List<string> answers = new List<string>();
                                        if (xmlDoc.Contains("&nbsp;"))
                                            xmlDoc = xmlDoc.Replace("&nbsp;", " ");
                                        XDocument xDoc = XDocument.Parse(xmlDoc);
                                        XElement xml1 = xDoc.Element("question").Element("interaction");
                                        foreach (var answerChoice in xml1.Elements("choice"))
                                        {
                                            answers.Add(answerChoice.Element("body").Value + " = " + answerChoice.Element("answer").Value);

                                        } %>
                                       <%-- <span class="text"><%= question.Text%></span>--%>
                                        <ul>
                                            <% foreach (var answer in answers)
                                                {                                                                                                  
                                                    %>
                                                    <li>                                                            
                                                    <span class="option-text"><%= answer%></span>                                                              
                                                    </li>
                                            <% } %>
                                        </ul>

                                    <% }  
                                    else
                                    { %>
                                            Can't show details for this type of question.
                                    <%                                              
                                    }    
                                %>
                            </div>
   <%
        }
    %>
</div>
</td>
<td id="FlagContents" class="second">
<div id="QBA-Flag-Container" style="display:none">
     <% Html.RenderPartial("AddFlag", new QuestionNote() { QuestionId = Model.Id }); %>
</div>
</td>
</tr>


<% 
    if (Model.AdminFlag == false)
    {
%>
<tr class="footer">
<td colspan="2">
<div class="preview-button-bar">   
    <button id="QBA-Flag-Button" class="QBA-button">Flag Question</button>
</div>
</td>
</tr>
<%
    }
%>


</table>
<script type="text/javascript" language="javascript">
    jQuery(document).ready(function () {
        if (PxQuestionAdmin) {
            PxQuestionAdmin.FlagEvent();
        }
    } (jQuery));   
</script>