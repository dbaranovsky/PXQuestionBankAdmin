<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
 <div id="BulkEditor" class="BulkEditorWindow">
<table id="maintable">
        <tr class="MainHeader">
				<td colspan="2">
					<div class="question-bank-title">
						Question Bank Manager for <b><%= ViewData["CourseTitle"]%></b>
                        <%= Html.RouteLink("Logout", "EcomEntitled", null, new { @class = "logout" })%>
					</div>        
				</td>
		   </tr>        
    <tr class="MainHeader">
                <td colspan="2" class="bowheader">
                    
                    <div>
                        <%--<input type="button" id="btn_BOW_Close" value="Close Window" class="QBA-button"/>--%>
                        <span >Bulk Operation</span>
                    </div>
                </td>
    </tr>

<tr class="contentRow">
    <td class="BOW_left">
            <div class="radiolist">
                <input type="radio" name="step1" value="1" class="bowRadio"/> <label>Select Questions</label> <br>
                <input type="radio" name="step2" value="2" class="bowRadio"/> <label>Choose Operation</label><br> 
                <input type="radio" name="step3" value="3" class="bowRadio"/> <label>Operation Details</label><br>
                <input type="radio" name="step4" value="4" class="bowRadio"/> <label>Confirmation</label><br>
                <input type="hidden" value="1" class="wizardState"/>
            </div>
    </td>
    <td class="BOW_ContentArea">
        <div class="cHeader">
            <input type="button" id="btnHeaderNext" value="Next" class="BulkWizardNext"/> 
            <input type="button" id="btnConfirmEdit" value="Confirm" class="BulkWizardConfirm"/>&nbsp;&nbsp;&nbsp;&nbsp;
            <input type="button" id="btnHeaderCancel" value="Cancel" class="BulkWizardCancel"/>
            
        </div>
        
        
        <div class="cBody">
            <div id="pv_1" class="bow_Questionlist bowpartials">Loading Contents .... </div>
            <div id="pv_2" class="bow_operationlist bowpartials"> <% Html.RenderPartial("~/Views/QuestionAdmin/BulkOperationList.ascx"); %> </div>
            <div id="pv_3" class="bow_operationDetails bowpartials"><% Html.RenderPartial("~/Views/QuestionAdmin/BulkOperationDetails.ascx"); %> </div>
            <div id="pv_4" class="bow_confirmation bowpartials"><% Html.RenderPartial("~/Views/QuestionAdmin/BulkConfirmationDialog.ascx"); %></div>
        </div>
        
        
        <div class="cFooter">
            <input type="button" id="btnFooterNext" value="Next" class="BulkWizardNext"/>
            <input type="button" id="btnFooterConfirm" value="Confirm" class="BulkWizardConfirm"/>&nbsp;&nbsp;&nbsp;&nbsp;
            <input type="button" id="btnFooterCancel" value="Cancel" class="BulkWizardCancel"/>
            
        </div>
    
    </td>
</tr>
</table>

</div>
<script type="text/javascript">

    PxQuestionAdmin.BulkEditInitialize();
    

</script>  