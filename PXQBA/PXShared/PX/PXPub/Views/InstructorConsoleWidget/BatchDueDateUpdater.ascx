<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<h2>Batch due date updater</h2>
<hr />
<br />
<div style="font-size:12px;">Use this interface to update any number of due dates all at once.</div>
<br />
<br />

<div style="font-size:13px;font-weight:bold;">Select assignments to update:</div>
<br />
<div id="batchDueDateContainer">

    <div style="float:left;font-size:12px;padding-top:7px;">Assignments due between&nbsp;&nbsp;</div>
    <div class="dueDate">
        <div style="float:left;">
            <input type="text" id="fromDate" readonly="true" class="textbox_calendar_toggle" />
            <input type="button" class="calendar_toggle" id="fromDueDateCal" style="display:none" />
        </div>
    </div>
    <span style="float:left;padding-left:10px;padding-right:5px;padding-top:5px;font-size:12px;">and&nbsp;</span>
    <div class="dueDate">
        <div style="float:left;">
            <input type="text" id="toDate" readonly="true" class="textbox_calendar_toggle" />
            <input type="button" class="calendar_toggle" id="toDueDateCal" style="display:none" />
        </div>
    </div>

    <div style="clear:both" />

    <div style="font-size:12px;padding-top:12px;"><input type="checkbox" id="updateRestrictedDates" />&nbsp;<span>Also move all visibility dates in the range above</span></div>

    <div>    
        <br />
        <div id="totalItemsdiv" >
            <span id="totalItems"></span>
        </div>
    </div>

    <div style="padding-top:20px;">
        <br /><br />
        <div style="font-size:13px;font-weight:bold;">Select a new start date:</div>
        <div style="padding-top:5px;">
            <div class="dueDate newDueDateDivContainer">
                <div>
                    <input type="text" id="newDueDate" readonly="true" class="textbox_calendar_toggle" />
                    <input type="button" class="calendar_toggle" id="newDueDateCal" style="display:none" />
                </div>
            </div>
            <br />
            <div style="padding-top:10px;">
                <span id="totalDaysShifted"></span>
            </div>
            <br />
            <div><input type="button" id= "btnBatchDueDateUpdate" value="Update due dates""/></div>
    
    
        </div>
        

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
