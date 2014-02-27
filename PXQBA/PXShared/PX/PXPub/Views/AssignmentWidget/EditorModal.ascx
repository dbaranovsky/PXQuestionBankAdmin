<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Bfw.PX.PXPub.Models.AssignedItem>>" %>

<% 
    int assignmentcount = int.Parse(ViewData["AssignmentCount"].ToString()); 
   %>

<div class="customEditWidget" dialogwidth="600" dialogheight="600" dialogminwidth="600"
    dialogminheight="600" dialogtitle="Assignments" style="float:left;">
    <div class="assignmentmodal-prompt">
        Always show these important assignments:
    </div>

    <style type="text/css">
        .errorAssignment
        {
            color: Red;
            padding-left: 10px;
            padding-right: 10px;
            display: none;
        }
        .errorSummary
        {
            color: Red;
            padding-top: 10px;
            padding-bottom: 10px;
            float: left;
            clear: both;
            display: none;
        }
        
        .watermarkAssignment
        {
            color: Gray;
        }
        
        .errorNoAssignments
        {
            color: Red;
            padding-top: 10px;
            padding-bottom: 10px;
            float: left;
            clear: both;
        }
        
    </style>
    
    <div class="AssignmentGroup">
        <%
            foreach (var assignedItem in Model)
            { 
        %>
        <div class="AssignmentContainer">
            <div style="float: left;">
                <input type="text" class="AssignmentName" value="<%= assignedItem.Title.Trim() %>" /></div>
            <div class="errorAssignment">
                *</div>
            <a class="removeAssignment" title="Remove from important assignments"></a>
            <input type="hidden" class="AssignmentID" value="<%= assignedItem.Id %>" />
        </div>
        <% }                       
           %>
        <% 
            string textstyle = string.Empty;
            if (assignmentcount == 0)
            {
                textstyle =  "disabled='disabled'";

            }%>
        <% if (Model.Count == 0)
              { %>
        <div class="AssignmentContainer">
            <div style="float: left;">
                <input type="text" class="AssignmentName watermarkAssignment" value="Start typing an existing assignment name..." <%= textstyle %>/></div>
                <div class="errorAssignment">*</div>
                <% if (assignmentcount != 0)
                   {%>
                    <a class="removeAssignment" title="Remove from important assignments"></a>
              <% } %>
            <input type="hidden" class="AssignmentID" />
        </div>
        <%  } %>
    </div>

    <input type="hidden" name="removedAssignments" class="removedAssignments InputForControllerAction" value=""/>
    <input type="hidden" name="importantAssignments" class="importantAssignments InputForControllerAction" value=""/>
    <input type="hidden" name="isFormValid" class="isFormValid" value="false" />
    <input type="hidden" name="assignmentcount" class="assignmentcount" value="<%= assignmentcount.ToString() %>" />           
    

    <% if (assignmentcount != 0)
       {%>
    <div class="addAssignment" style="float:left;clear:both;cursor:pointer;color:Blue;padding-top:10px;padding-bottom:10px;">
        + Include another important assignment
    </div>
    <% } %>

    <% if (assignmentcount == 0)
       {%>
    <div class="errorNoAssignments">
        You have no assigned content to mark "Important".
    </div>
    <% } %>

    <div class="errorSummary">
        Please select correct assignments to mark them as important.
    </div>
    <script type="text/javascript" language="javascript">
        (function ($) {

            PxPage.Require(['<%= Url.ContentCache("~/Scripts/AssignmentWidget/AssignmentWidget.js") %>'], function () {
                jQuery(document).ready(function () {
                    PxAssignmentWidget.BindEditModalControls();
                });
            });

        } (jQuery));    
    </script>
</div>
