<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Quiz>" %>


<%
    string classType = "quizActivity";
    bool isLearningCurveActivity = false;
    if (Model is LearningCurveActivity)
    {
        classType = "learningCurveActivity";
        isLearningCurveActivity = true;
    }
    var rootUrl = Url.GetComponentHash("item", Model.Id, new { mode = ContentViewMode.Questions, renderFne = true });
%>
<div class="quiz-editor <%=classType %>">
    <div class="available-questions">
        <div class="available-questions-header">
            <div class="questions-header-top">
                <div class="default-breadcrumb">
                    <div class="default-bread-crumb-siblings-list" style="display: none;">
                        <ul>
                            <li><a href="javascript:" class="default-sibling-item" id="PX_LOR">Questions by chapter</a></li>
                            <li><a href="javascript:" class="default-sibling-item" id="PX_TOC">Questions by assessment</a></li>
                            <li><a href="javascript:" class="default-sibling-item" id="PX_MY_QUESTIONS">Questions I've created or edited</a></li>
                        </ul>
                    </div>
                </div>
                <input class="default-root-selector" type="hidden" />
                <input class="default-category" type="hidden" />
                <div class="breadcrumb-trail">
                    <span id="question-list-level" style='display:none'>
                        <a href="<%= rootUrl %>">Question Banks</a>
                        <span>&raquo; </span>
                    </span>
                    
                </div>
                <span class="quiz-bank-title">Question banks</span>
                <div class="searchQuestion">
                    <input type="text" id="txtSearchQuiz" size="25" placeholder="Search Questions" />
                    <input type="button" id="btnSearchQuiz" />
                    <input type="hidden" class="search-type" />
                    <span class="search-category"></span>
                </div>
            </div>
            <div class="questions-actions">
                <div class="select-menu"><div class="gearbox"></div></div>
                <%--                <div class="add-btn-wrapper">
                    <div class="add-to-quiz-menu">Add to Assessment</div>
                    <div class="add-menu"></div>
                </div>
                --%>
                <div class="add-quiestion-btn-wrapper">
                    <div class="add-available-question-at-top">Add</div>
                </div>
                <div class="add-to-pool-available-quiestion-btn-wrapper">
                    <div class="add-menu"><div class="gearbox"></div></div>
                </div>
                <div class="collapse-btn-wrapper">
                    <a class="collapse-all-available-question" href="javascript:" style="text-decoration: none;">collapse all</a>
                </div>
                <div class="expand-btn-wrapper">
                    <a class="expand-all-available-question" href="javascript:" style="text-decoration: none;">expand all</a>
                </div>
            </div>
        </div>
        <div class="available-questions-body">
            <div class="children-list"></div>
        </div>

    </div>
    <div class="selected-questions">
        <div class="question-bank-header-right">
            <div class="title">Questions in this assessment</div>
            <div class="questions-actions">
                <%--<div class="select-menu-used"></div>--%>
                <label class="hasborder" style="float: left; margin-left: 7px;">
                    <input type="checkbox" name="selectall" /></label>
                <div class="create-btn-wrapper">
                    <div class="new-question">Create</div>
                    <input type="hidden" id="previous-question-type" />
                    <div class="new-question-menu"></div>
                </div>
                <div class="remove-btn-wrapper">
                    <div class="remove-question">Remove</div>
                </div>
                <div class="browse-question-banks-btn-wrapper">
                    <div class="browse-question-banks">Browse question banks</div>
                </div>


                <%--                <div class="new-question-wrapper">
                    <div class="new-question">New</div> 
                    <input type="hidden" id="previous-question-type" />
                    <div class="new-question-menu"></div>
                </div>
                --%>
            </div>
        </div>
        <div id="show-question" class="question-dialog-text"></div>
        <div class="quiz-editor-questions">
            <% Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/Questions.ascx", Model); %>
        </div>

    </div>

    <div class="edit-question-container" style="display: none;">
        <div class="breadcrumb-trail"></div>
        <div class="edit-question"></div>
        <div class="question-removed-message" style="display: none;">
            <div>
                Blank questions cannot be saved, and so this question has been removed. Please fill out questions before saving.
            </div>
        </div>
    </div>
    <div class="question-pool-container" style="display: none">
        <%= Html.HiddenFor(m => m.Id) %>
        <%= Html.HiddenFor(m => m.ParentId) %>
        <%= Html.HiddenFor(m => m.Type) %>
        <%= Html.HiddenFor(m => m.Url) %>
        <%= Html.HiddenFor(m => m.IsAssignable) %>
        <%= Html.HiddenFor(m => m.Sequence)%>
        <%= Html.HiddenFor(m => m.QuizType) %>
        <input type="hidden" id="question-pool-id" />

        <input type="hidden" class="content-item-id" value="<%=Model.Id %>" />

        <div class="question-pool-inner-container">
            <div class="question-pool-field" id="form">
                <div class="pool-fields" >
                    <label for="txtPoolName">
                        <b>Pool name</b></label>
                    <div>
                        <input id="txtPoolName" name="Title" value="New question pool" type="text"  style="width:95%" onclick="$('#spnPoolTitleError').hide('slow');" />
                        <span id="spnPoolTitleError" style="display: none" class="field-validation-error px-default-text">Please enter the title</span>
                    </div>
                </div>
                <div class="pool-fields">
                    <label for="txtPoolCount">
                        <b>Pull how many questions from this pool?</b></label>
                    <div>
                        <input id="txtPoolCount" name="poolCount" type="text" maxlength="3" style="width: 50px" />
                        out of <span class="poolSize"></span> questions total.               
                            <span id="spnPoolCountError" style="display: none" class="field-validation-error px-default-text">Please enter a valid pool count</span>
                        <span id="spnPoolIntegerError" style="display: none" class="field-validation-error px-default-text">Please enter a numeric value.</span>
                    </div>
                </div>
                <div class="pool-fields">
                    <label for="txtPoolPoints">
                        <b>How many points is each question worth?</b></label>
                    <div>
                        <input id="txtPoolPoints" name="poolPoints" value="1" type="text" maxlength="3" style="width:50px" />
                        <span id="spnPoolPointsError" style="display: none" class="field-validation-error px-default-text">Please enter a valid points value.</span>
                    </div>
                 </div>
            </div>
            <%if (isLearningCurveActivity)
              { %>
            <div id="related-content-editor-pool" style="display: none">
                <!-- This will have the editor and tool tip at runtime-->
            </div>
            <%} %>
            <div class="pool-actions question-nav">
                <input type="button" class="save primary large button savebtn" value="Save" />
                <div class="leftnav">
                    
                    <a href="javascript:" class="undo-question-pool">Revert to saved</a>
                    <%if (isLearningCurveActivity)
                      { %>
                    <a href="javascript:" class="edit-related-content"><span class="icon"></span>Add/Edit Related Content</a>
                    <%} %>
                </div>
                <div class="rightnav" style="display: none">
                    <a href="javascript:" class="back-question"><span class="icon"></span>Back to Pool</a>
                </div>
            </div>
            <span class="clear"></span>
        </div>
    </div>
    <div id="question-pool-dialog-container" style="display: none;">
        <% Html.RenderPartial("~/Views/Shared/DisplayTemplates/QuizPartials/CreateQuestionPool.ascx", Model); %>
    </div>
    <div id="show-question-pool-dialog" class="question-pool-dialog-text"></div>
</div>


<div id="validate-question-pool" class="question-validate-text"></div>


<%--<div id="delete-confirm" style="display:none;">
    Are you sure you want to delete this question?<br />
    <input type="checkbox" name="not-again" /><label for="not-again">Never ask again</label>
</div>--%>

<div id="divAddPool" class="divPopupWin">
    <%= Html.HiddenFor(m => m.Id) %>
    <%= Html.HiddenFor(m => m.ParentId) %>
    <%= Html.HiddenFor(m => m.Type) %>
    <%= Html.HiddenFor(m => m.Url) %>
    <%= Html.HiddenFor(m => m.IsAssignable) %>
    <%= Html.HiddenFor(m => m.Sequence)%>
    <%= Html.HiddenFor(m => m.QuizType) %>
    <h2 id="divAddWinTitle" class="divPopupTitle">NEW POOL 
    </h2>
    <div id="divLinkContent" class="divPopupContent">
        <% using (Ajax.BeginForm("AddNewPool", "Quiz", new { parentid = Model.Id }, new AjaxOptions { HttpMethod = "Post", OnSuccess = "PxQuiz.RefreshQuestionList", UpdateTargetId = "" }, null))
           { %>
        <span class="field buttons">
            <label for="QuizTitle">
                Title</label>
            <input id="Title" name="Title" type="text" onclick="$('#spnTitleError').hide('slow');" />
            <span id="spnTitleError" class="field-validation-error px-default-text">Please enter the title</span>
            <br />
            <label for="lblPool">
                Pull how many questions from the pool?</label>
            <input id="PoolCount" name="poolCount" type="text" maxlength="3" onclick="$('#spnCountError').hide('slow');" />
            <span id="spnCountError" class="field-validation-error px-default-text">Please enter a valid pool count</span>
            <span id="spnIntegerError" class="field-validation-error px-default-text">Please enter a numeric value</span>
            <br />
            <input type="submit" name="behavior" value="Save" id="poolBtnQuizSubmit" />
            <input type="button" name="behavior" value="Cancel" id="linkQuizCancel" />
            <%--<a href="#" id="linkQuizCancel"> Cancel </a>--%>
        </span><span class="clear"></span>
        <% } %>
    </div>
</div>


<div id="divRandomSelect" class="divPopupWin">
    <h2 class="divPopupTitle"></h2>
    <div id="divRandomSelectContent" class="divPopupContent">
        <label>
            Randomly select how many questions :</label>
        <input id="txtRandomSelect" name="Name" type="text" />
        <span id="spnNameError" class="field-validation-error px-default-text">Please enter a number.</span>

    </div>
    <div class="randomselect-footer">
        <span>
            <input type="button" value="Select Questions" id="btnRandomSave" />
            | 
            <a href="javascript:" id="linkRandomCancel">Cancel </a>
        </span>
    </div>
</div>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            //$("#e1").click(function () { alert("Selected value is: "+$("#e1").select2("val"));});
            var deps = <%= ResourceEngine.JsonFor("~/Scripts/quiz.js") %>;
            PxPage.Require(deps, function () {
                PxQuiz.Init();
                PxQuiz.FneInit();
                PxQuiz.FneResize();
                
                $('#fne-window').removeClass('require-confirm');
                $(".quiz-editor").LearningCurveMoreResources();
            });
            
            /*
            PxPage.Require(<%= ResourceEngine.JsonFor("~/Scripts/LearningCurve/LearningCurveMoreResources.js") %>, function () {
                $('#fne-window').removeClass('require-confirm');
                $(".quiz-editor").LearningCurveMoreResources();
            });
            */
        });

    } (jQuery));    
</script>


