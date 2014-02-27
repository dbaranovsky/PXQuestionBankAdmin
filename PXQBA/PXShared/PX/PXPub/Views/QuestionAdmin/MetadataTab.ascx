<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.QuestionMetadata>" %>
<% 

    QuestionCardData qcd = new QuestionCardData();
    IEnumerable<SelectListItem> selectedItem = new List<SelectListItem>();
    List<SelectListItem> theSelectedList = new List<SelectListItem>();
    SelectListItem selected = new SelectListItem();
    
%>
<style type="text/css">
    #meta-data-container
    {
        width: 800px;
        margin: 0 auto;
    }
    
    #meta-data-container p
    {
        margin: 0px 0px 10px 0px;
        word-break: break-all;
    }
    #meta-data-container .meta-labels
    {
        width: 200px;
        float: left;
        font-weight: bold;
    }
    
    #meta-data-container input[type="text"], #meta-data-container select
    {
        width: 100%;
    }
    
    
    #meta-data-container .meta-textbox
    {
        height: 25px;
    }
    
    #meta-data-container tr
    {
        width: 100%;
        float: left;
        border-bottom: 1px solid #E7E7E7;
        margin-top: 15px;
    }
    #meta-data-container tr:last-child
    {
        width: 100%;
        float: left;
        border-bottom: none;
        margin-top: 15px;
    }
    
    #meta-data-container td.left
    {
        width: 200px;
        float: left;
    }
    #meta-data-container td.right
    {
        width: 580px;
        float: left;
        min-height: 41px;
    }
    
    #meta-data-container .right .edit-button
    {
        color: #2D65D0;
        font-weight: bold;
    }
    #meta-data-container .select2 {
        width: 100%;
        padding-bottom: 2px;
    }
    
    #meta-data-container .select2 .select2-choice #free-edit
    {
        overflow: hidden !important;
    }
    #free-edit #free-edit-textarea
    {
        width: 100%;
        float: left;
        height: 100%;
        font-size: 14px;
    }
    
    #read-only-values-container
    {
        width: 800px;
        margin: 0 auto;
        background-color: #F1F1F1;
        margin-top: 10px;
    }
    #read-only-values-container table
    {
        width: 100%;
        border: none;
    }
    
    h3.read-only-title
    {
        background-color: #D2D3D5;
        padding: 5px;
        font-weight: bold;
        margin: 0px;
    }
    
    
    #read-only-values-container tr
    {
        width: 100%;
        float: left;
        border-bottom: 1px solid #E7E7E7;
        padding: 5px;
        margin-top: 0px;
    }
    
    #read-only-values-container td.left
    {
        float: left;
        width: 30%;
        height: 30px;
        font-weight: bold;
    }
    .free-text-view-all-button
    {
        color: #2D65D0;
        font-weight: bold;
        cursor: pointer;
        width: 100%;
        float: left;
    }
    
    .hidden-selected-results
    {
     display:none;   
        
    }
    
    #MetadataTab table
    {
     width:100%;   
    }
    .select2-container
    {
        width:100%;  
    }
    
    .link-format
    {
        color: #2D65D0;
        text-decoration: none;
    }
    
    .hide-view-all
    {
     display:none;   
    }
</style>
<table id="QBA-Metadata-Table">
    <tr class="main">
        <td class="first">
            <div id="MetadataTab">
                <div id="meta-data-container" data-qba-questionid="<%= Model.QuestionData.Id %>"
                    data-qba-entityid="<%= Model.QuestionData.EntityId %>">
                    <table>
                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    Title</label>
                            </td>
                            <td class="right">
                                <input type="text" class="meta-textbox dirtyField" id="QBA-Metadata-Title" value="<%=Model.QuestionData.Title %>" />
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    Exercise number</label>
                            </td>
                            <td class="right">
                                <input type="text" class="meta-textbox dirtyField" id="QBA-Metadata-exercise-number" value="<%= Model.QuestionData.ExcerciseNo %>" />
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    Question bank</label>
                            </td>
                            <td class="right">

              
              <%if (Model.Metadata.QuizSelectList != null)
                {

                    theSelectedList = Model.Metadata.QuizSelectList.ToList();

                    if (theSelectedList != null)
                    {
                        selected = theSelectedList.Find(i => i.Value == Model.QuestionData.QuestionBank);

                    
                    if (selected != null)
                    {
                        selected.Selected = true;

                    }

                    }
                    %>

                                <%= Html.DropDownList("QBA-Metadata-question-bank", theSelectedList, new { @class = "select2 single dirtyField" })%>


                                       <%} %>                                 
                            </td>
                        </tr>

                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    eBook Section</label>
                            </td>
                            <td class="right">
                            <%if (Model.Metadata.ChapterSelectList != null)
                {

                    theSelectedList = Model.Metadata.ChapterSelectList.ToList();

                    if (theSelectedList != null)
                    {
                        if (!string.IsNullOrEmpty(Model.QuestionData.eBookChapter))
                            selected = theSelectedList.Find(i => i.Text == Model.QuestionData.eBookChapter);
                        else
                        { // only questionBank was selected while creating New Question from QBA
                            selected = theSelectedList.Find(i => i.Value == Model.Metadata.ChapterSelectedValues.SingleOrDefault());
                        }
                    
                    if (selected != null)
                    {
                        selected.Selected = true;

                    }

                    }
                    %>

                                <%= Html.DropDownList("QBA-Metadata-ebook-section", theSelectedList, new { @class = "select2 single dirtyField" })%>


                                       <%} %>  

                            </td>
                        </tr>

                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    Difficulty</label>
                            </td>
                            <%
                                qcd = new QuestionCardData();
                                qcd = Model.QuestionCardData.Find(dasd => dasd.FriendlyName.ToLowerInvariant() == "difficulty");
                                if (qcd != null && qcd.QuestionValues.Count > 0)
                                {
                                    selectedItem = Model.QuestionCardData.Find(dasd => dasd.FriendlyName.ToLowerInvariant() == "difficulty").SelectedQuestionValues.Where(i => i.Text == Model.QuestionData.Difficulty);
                                    if (selectedItem.Any())
                                    {
                                        selectedItem.FirstOrDefault().Selected = true;
                                    }
                            %>
                            <td class="right">
                                <%=
                            Html.DropDownList("QBA-Metadata-difficulty", Model.QuestionCardData.Find(dasd => dasd.FriendlyName.ToLowerInvariant() == "difficulty").SelectedQuestionValues, new { @id = "QBA-Metadata-difficulty", @class = "select2 single dirtyField" })
                                %>
                            </td>
                            <%}%>
                        </tr>
                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    Cognitive level</label>
                            </td>
                            <%      
                                qcd = new QuestionCardData();
                                qcd = Model.QuestionCardData.FirstOrDefault(dasd => dasd.FriendlyName != null && dasd.FriendlyName.ToLowerInvariant() == "cognitive level");

                                if (qcd != null && qcd.QuestionValues.Count > 0)
                                {
                                    selectedItem = Model.QuestionCardData.Find(dasd => dasd.FriendlyName.ToLowerInvariant() == "cognitive level").SelectedQuestionValues.Where(i => i.Value == Model.QuestionData.CongnitiveLevel);

                                    if (selectedItem.Any())
                                    {
                                        selectedItem.FirstOrDefault().Selected = true;
                                    }
                            %>
                            <td class="right">
                                <%= Html.DropDownList("QBA-Metadata-cognitivie-level", Model.QuestionCardData.Find(dasd => dasd.FriendlyName.ToLowerInvariant() == "cognitive level").SelectedQuestionValues, new { @class = "select2 single dirtyField" })%>
                                <%}%>
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    Bloom's domain</label>
                            </td>
                            <td class="right">
                                <%
                                    qcd = new QuestionCardData();
                                    qcd = Model.QuestionCardData.FirstOrDefault(dasd => dasd.FriendlyName != null && dasd.FriendlyName.ToLowerInvariant() == "bloom's level");

                                    if (qcd != null && qcd.QuestionValues.Count > 0)
                                    {

                                        selectedItem = Model.QuestionCardData.Find(dasd => dasd.FriendlyName.ToLowerInvariant() == "bloom's level").SelectedQuestionValues.Where(i => i.Value == Model.QuestionData.BloomsDomain);

                                        if (selectedItem.Any())
                                        {
                                            selectedItem.FirstOrDefault().Selected = true;
                                        }
                                %>
                                <%= Html.DropDownList("QBA-Metadata-blooms-domain", Model.QuestionCardData.Find(dasd => dasd.FriendlyName.ToLowerInvariant() == "bloom's level").SelectedQuestionValues, new { @class = "select2 single dirtyField" })%>
                                <%}%>
                            </td>
                        </tr>
                         <% string hideViewAllButtonClass = "hide-view-all"; %>
                         
                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    Guidance</label>
                            </td>
                             
                            <td class="right">
                                <p id="QBA-Metadata-guidance" data-qba-full-text="<%= Model.QuestionData.Guidance%>">
                                    <%= Model.QuestionData.Guidance != null && Model.QuestionData.Guidance.Count() > 300 ? Model.QuestionData.Guidance.Substring(0, 300): Model.QuestionData.Guidance%> </p>
                                    <% if (Model.QuestionData.Guidance != null && Model.QuestionData.Guidance.Length > 300)
                                       {
                                           hideViewAllButtonClass = "";

                                       } %>
                                <a class="free-text-view-all-button <%= hideViewAllButtonClass %>">View All</a>
                                <a class="edit-button">Edit</a>
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    Free response question</label>
                            </td>
                            <td class="right">
                                <p id="QBA-Metadata-free-response" data-qba-full-text="<%= Model.QuestionData.FreeResponseQuestion%>">
                                    <%= Model.QuestionData.FreeResponseQuestion != null && Model.QuestionData.FreeResponseQuestion.Count() > 300 ? Model.QuestionData.FreeResponseQuestion.Substring(0, 300) : Model.QuestionData.FreeResponseQuestion%></p>
                                   <% 
                                       hideViewAllButtonClass = "hide-view-all";
                                       if (Model.QuestionData.FreeResponseQuestion != null && Model.QuestionData.FreeResponseQuestion.Length > 300)
                                       {
                                           hideViewAllButtonClass = "";

                                       } %>
                                
                                <a class="free-text-view-all-button <%= hideViewAllButtonClass %>" >View All</a>
                                <a class="edit-button">Edit</a>
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    Suggested use</label>
                            </td>
                            <td class="right">
                                <%
                                    qcd = new QuestionCardData(); 
                                    qcd = Model.QuestionCardData.FirstOrDefault(dasd => dasd.FriendlyName != null && dasd.FriendlyName.ToLowerInvariant() == "suggested use");

                                    if (qcd != null && qcd.QuestionValues.Count > 0)
                                    {
                                %>
                                <%= Html.DropDownListFor(questionCard => questionCard.QuestionCardData.Find(dasd => dasd.FriendlyName == "Suggested Use").QuestionValues,
                          new SelectList(Model.QuestionCardData.Find(dasd => dasd.FriendlyName == "Suggested Use").SelectedQuestionValues, "Value", "Text"),
                                                                                  new { @id = "QBA-Metadata-Suggested-use", @class = "select2 multiple" })
                  
                                %>
                                <%}

                                
                                %>
                                  <%if (Model.SelectedSuggestUse != null)
                                    { %>
                           <%= Html.DropDownList("QBA-Metadata-selected-suggested", Model.SelectedSuggestUse, new { @class = "hidden-selected-results" })%>
                          <%} %>
                            </td>
                          
                        </tr>
                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    Learning Objective</label>
                            </td>
                            <td class="right">
                                <%
                                    qcd = new QuestionCardData(); 
                                    qcd = Model.QuestionCardData.FirstOrDefault(dasd => dasd.FriendlyName != null && dasd.FriendlyName.ToLowerInvariant() == "learning objectives");


                                    if (qcd != null && qcd.QuestionValues.Count > 0)
                                    {
                                    
                                    
                                %>
                                <%= Html.DropDownListFor(questionCard => questionCard.QuestionCardData.Find(dasd => dasd.FriendlyName.ToLowerInvariant() == "learning objectives").QuestionValues,
                                                                                                        new SelectList(Model.QuestionCardData.Find(dasd => dasd.FriendlyName == "learning objectives").SelectedQuestionValues, "Value", "Text"),
                                                                                                                            new { @id = "QBA-Metadata-learning-objective", @class = "select2 multiple" })
                  
                                %>
                                <%}
                                %>
                                    <%if (Model.SelectedLearningObjectives != null)
                                    { %>
                           <%= Html.DropDownList("QBA-Metadata-selected-learningobjectves", Model.SelectedLearningObjectives, new { @class = "hidden-selected-results" })%>
                              <%} %>
                            </td>
                           
                        </tr>



                        <tr>
                            <td class="left">
                                <label class="meta-labels">
                                    Status </label>
                            </td>
                            
                            
                            <td class="right">

                            <%
                                var statusList = new SelectList(new[]{
                                  new SelectListItem{ Text="-1", Value="Not Set"},
                                  new SelectListItem{ Text="0", Value=ExtensionMethods.GetEnumDescription(QuestionStatusType.InProgress)},
                                  new SelectListItem{ Text="1", Value=ExtensionMethods.GetEnumDescription(QuestionStatusType.AvailabletoInstructor)},
                                  new SelectListItem{ Text="2", Value=ExtensionMethods.GetEnumDescription(QuestionStatusType.Deleted)},
                                }, "Text", "Value", Model.QuestionStatus);
                            
                            %>
                                <%= Html.DropDownList("QBA-QuestionStatus", statusList, new { @class = "select2 single" })%>
                                    
                            </td>

                        </tr>


                        <tr>
                            <td>
                                <button id="QBA-Metadata-Button" class="QBA-button">
                                    Save</button>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="read-only-values-container">
                    <h3 class="read-only-title">
                        Read Only Values</h3>
                    <table>
                        <tr>
                            <td class="left">
                                Used in quiz
                            </td>
                            <td>
                                <%= string.IsNullOrEmpty(Model.UsedIn) == true ? Model.QuestionBankText : Model.UsedIn %>
                                &nbsp;&nbsp;<a id="SimilarQuestions" href="#" class="link-format" data-qba-quiz-id="<%= Model.QuestionData.QuestionBank %>"> Show other questions in this quiz </a>
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                                Type
                            </td>
                            <td>
                            <%                        
                                var questionType = Model.QuestionData.Type.ToUpper();
                                if (questionType == "CUSTOM" && Model.QuestionData.CustomUrl.ToUpper() == "HTS")
                                    questionType = "Advanced Question";
                                else
                                    if (questionType == "CUSTOM" && Model.QuestionData.CustomUrl.ToUpper() == "FMA_GRAPH")
                                    questionType = "Graph Exercise";
                                    else
                                        questionType = Question.QuestionType(questionType);
                            %>   

                                <%= questionType %>
                            </td>
                        </tr>
                    </table>
                </div>
        </td>
        <td class="second">
            <%--<div id="QBA-Flag-Container">
     <% Html.RenderPartial("AddFlag", new QuestionNote() { QuestionId = Model.Id }); %>
</div>
            --%>

           
        </td>
    </tr>
    <input type="hidden" value="<%=Model.QuestionData.QuestionBank %>" id="oldQuizId" />
</table>

<script type="text/javascript" language="javascript">
    jQuery(document).ready(function () {
        if (PxQuestionAdmin) {
            PxQuestionAdmin.MetadataEvent();
            PxQuestionAdmin.TurnDropDownToSelect2();
        }
    } (jQuery));   
</script>
