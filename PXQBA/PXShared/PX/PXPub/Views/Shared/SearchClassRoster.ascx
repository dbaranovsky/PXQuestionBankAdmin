<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

    <div id="divAddIndividual" class="divPopupWin divAddIndividual">   
<%--        <h2 id="divAddWinTitle" class="divPopupTitle">
           SEARCH CLASS ROSTER
        </h2>--%>
        <div id="divAddContent" class="divPopupContent">      
        <label id="errorMessage" class="errorMessage" style="color:Red">&nbsp;</label><br /> 
            <span class="field buttons">
                <input id="studentName" name="Name" type="text" class="studentName" onclick="$('.field-validation-error').hide('slow');" />
                <span id="spnNameError" class="field-validation-error px-default-text" >Please enter a name</span>
                <input type="hidden" id="studentId" class="studentId" />         
                <input type="button" value="Cancel" onclick="$('.ui-icon-closethick').click();" />
                <input type="button" value="Add" id="btnAdd" onclick="return PxSettingsTab.OnAdd();"/>
            </span>       
        </div>
    </div>