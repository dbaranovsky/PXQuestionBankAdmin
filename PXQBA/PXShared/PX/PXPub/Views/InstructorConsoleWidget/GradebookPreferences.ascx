<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>

<div class="instructor-console-wrapper" id="instructor-console-wrapper">

<div class="title section">
    <h2>Gradebook Preferences</h2>
</div>

    <div class="section">
        <div class="section-content">
            <%= Html.LabelFor(o => o.PassingScore) %>
            <%= Html.TextBoxFor(o => o.PassingScore, new { size = "2", maxlength = "5", id = "passingScore" }) %>
            <input type="button" id="savePassingScore" name="savePassingScore" value="OK" style="display:none" />
            <ul style="display: none" id="passingScoreExplanation" class="explanation">
                <li>
                    Grades lower than this value will be colored red in the gradebook.
                </li>
            </ul>
        </div>

        <div class="section-whatsthis">
            [<a href='#' id='whatIsPassingScore'>What's this?</a>]
        </div>
        <div style="clear:both" />
    </div>

    <div class="section">
        <div class="section-content">
            <%= Html.CheckBoxFor(o => o.UseWeightedCategories, new { id = "useWeightedCategories" })%>
            <%= Html.LabelFor(o => o.UseWeightedCategories) %>
        <ul style="display: none" id="weightedExplanation" class="explanation">
            <li>
                If "Use weighted categories" is
                <b>off</b>
                , grades are calculated by multiplying each assignment's percent grade times the assignment's gradebook points, then summing these points across all assignments. The final grade equals the total points earned divided by the total points possible. In this scheme, categories serve solely to organize assignments in the gradebook
            </li>
            <li>
                If "Use weighted categories" is
                <b>on</b>
                , you will enter a weight for each category. To calculate grades, gradebook points for assignments within each category are first summed, and a percent of points earned for the category is calculated. Then each category's percent grade is multiplied by the category's weight to determine each category's points, which are summed. The final grade equals the total category points earned divided by the sum of the category weights.
            </li>
        </ul>
      
        </div>
        <div class="section-whatsthis">
            [<a href='#' id='whatIsWeighted'>What's this?</a>]
        </div>
        <div style="clear:both" />        
    </div>

    <div class="section">
        <div class="section-content">
            <span>Gradebook Categories and Assignments</span>
            <a href='javascript:' id="showAssignments">Show Assignments</a>
               <ul style="display: none" id="categoriesExplanation" class="explanation">
            <li>
                Click on a category title to edit the title. Click the category's "Drop Lowest" value to edit the number of assignments in the category that will be dropped from the final grade. If "Use weighted categories" is on, click the category's "Weight" value to edit it. Click "Remove" to remove the category altogether (any assignments in the category will be moved to the "Uncategorized" category; this "Uncategorized" category cannot be renamed or removed).
            </li>
            <li>
                Click "Show Assignments" to show assignments within each category. Extra credit assignments are indicated by an asterisk (*) after the assignment's Points value. Click an assignment title to view the assignment's settings screen, where you can update its points, move it to a different category, or designate it for extra credit.
            </li>
            <li>
                Change the order in which categories appear in the gradebook, or the order in which assignments appear within each category, using the drop-down menus.
            </li>
        </ul>

        </div>
        <div class="section-whatsthis">
            [<a href='#' id="whatIsCategories">What's this?</a>]
        </div>
        <div style="clear:both" />
     
    </div>

<div id="gradebookCategoriesList">
    <% Html.RenderAction("GradebookCategoriesList", new { model = new Bfw.PX.PXPub.Models.GradebookPreferences() }); %>
</div>

</div>
<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/InstructorConsoleWidget/InstructorConsole.js") %>'], function () {
                PxInstructorConsoleWidget.Init();
            });
        });
    } (jQuery));    
</script>