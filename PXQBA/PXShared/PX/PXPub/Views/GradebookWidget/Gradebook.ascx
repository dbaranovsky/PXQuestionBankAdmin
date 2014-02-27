<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% 
    var url = Url.Action("ExportAssignedReport", "GradebookWidget");
    var mode = ViewData["mode"];
%>
<div id="gbcontrol"  mode="<%=mode %>">
    <div id="gbheader">
        <div id="gbScoreFilter">
            <div class="scores-table-nav">
            <div class="icon"></div>
                <div class="filterWrapper">   
                    <div id="radio">
                        <input type="radio" id="assignedScoresTab" name="scoreview" checked="checked" ref="<%= Url.GetComponentHash("gradebook", "assignedScores") %>"/><label for="assignedScoresTab">Assigned Scores</label>
                        <input type="radio" id="otherScoresTab" name="scoreview" ref="<%= Url.GetComponentHash("gradebook", "otherScores") %>"/><label for="otherScoresTab">Other Scores</label>
                    </div>
                    <span class="classAvg">Class average for all assignments: </span><span class="classAvg classAvgPercent"></span>
                </div>
                <div class="gridButtons">
                        <button id="key" type="button">Key</button>
                        <a class="allAssignmentReportBtn" href="<%= Url.Action("ExportAllAssignmentsReport", "GradebookWidget") %>">
                            <span class="export">Export</span>
                        </a>
                        <a class="allAdditionalItemReportBtn" href="<%= Url.Action("ExportAllAdditionalItemReport", "GradebookWidget") %>" style="display:none">
                            <span class="export">Export</span>
                        </a>
                </div>
            </div> 
        </div>
    </div>
    
    <div id="gradebookContent">
    </div>
</div>
<script type="text/javascript">
(function ($) {
    PxPage.OnReady(function () {
        PxPage.Require(['<%= Url.ContentCache("~/Scripts/jquery/jquery.dataTables.min.js") %>',
                        '<%= Url.ContentCache("~/Scripts/DataTablesExtras/FixedColumns.min.js") %>'], function () {
                            if ($('#gbcontrol').attr('mode') === 'Instructor') {
                                PxGradebook.InitInstructor();
                            } else {
                                PxGradebook.InitStudent();
                            }
                        });
    });
} (jQuery));
</script>