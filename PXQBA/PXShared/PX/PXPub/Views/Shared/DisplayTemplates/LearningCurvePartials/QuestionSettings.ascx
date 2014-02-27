<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.LearningCurveQuestionSettings>" %>

<% using (Html.BeginForm("LearningCurveQuestionSettings", "Quiz", FormMethod.Post, new { id = "edit-settings-form-learningCurve" }))
   { %>
    <%: Html.HiddenFor(x => x.Id) %>
    <%: Html.HiddenFor(x => x.QuizId) %>
    <%: Html.HiddenFor(x => x.EntityId) %>
    <%: Html.HiddenFor(x => x.QuestionType) %>
    
    <%
        var difficultyLevels = new List<KeyValuePair<string, string>>();
        difficultyLevels.Add(new KeyValuePair<string,string>("1","1(easy)"));
        difficultyLevels.Add(new KeyValuePair<string,string>("2","2(medium)"));
        difficultyLevels.Add(new KeyValuePair<string,string>("3","3(hard)"));
        //difficultyLevels.Add(new KeyValuePair<string,string>("4","4(very hard)"));
    
     %>
    <div class="question-settings-form-lc">
        <%: Html.LabelFor(x => x.QuestionType) %>: <span class="settings-right"><%: Question.QuestionType(Model.QuestionType) %></span> 
    </div>
    <%//Temporary hide this settings until it's fixed %>
    <div class="question-settings-form-lc" style="display:none">
           <span class="settings-right">
           <%: Html.CheckBoxFor(x => x.NeverScrambleAnswers) %><span>Never scramble answers</span></span>
            <%--<input type="checkbox" value="<%=Model.NeverScrambleAnswers%>" />--%> 
    </div>

    <div class="question-settings-form-lc">
        <%: Html.LabelFor(x => x.DifficultyLevel) %>: 
        <span class="settings-right">
        <%: Html.DropDownListFor(x => x.DifficultyLevel, new SelectList(difficultyLevels,"Key","Value",Model.DifficultyLevel)) %>
       <%-- <select class="lc_question_difficultyLevel"  name="ddlDifficultyLevel">
                <option <%= (string.IsNullOrEmpty(Model.DifficultyLevel) || Model.DifficultyLevel=="1") ? "selected='selected'" : "" %> value="1">1(easy)</option>
                <option <%= (Model.DifficultyLevel=="2") ? "selected='selected'" : "" %> value="1">2(normal)</option>
                <option <%= (Model.DifficultyLevel=="3") ? "selected='selected'" : "" %> value="1">3(hard)</option>
                <option <%= (Model.DifficultyLevel=="4") ? "selected='selected'" : "" %> value="1">1(very hard)</option>
         </select>--%>
         </span>
    </div>
    <%//Temporary hide this settings until it's fixed %>
    <div class="question-settings-form-lc" style="display:none">
        <%: Html.CheckBoxFor(x => x.PrimaryQuestion)%><span>Primary Question</span></span>        
    </div>
       
<% } %>