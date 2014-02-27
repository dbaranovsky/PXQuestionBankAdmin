<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AssignedItem>" %>
<% 
    
    Model.IsItemLocked = ViewData["accessLevel"].ToString() == "instructor" ? false : Model.IsItemLocked;
%>

    <fieldset id="fsGradebook">
       
            <ol>
                <% 
                    Model.IsGradeable = Model.IsGradeable || (Model.Score.Possible > 0);
                    var isShowPoints = (Model.AssignTabSettings.ShowPointsPossible) ? "" : "display:none";
                    var disableGradable = (Model.IsItemLocked && Model.IsGradeable) ? "disabled" : "";
                    var disableGradeExtraCredit = (Model.IsItemLocked && Model.IsGradeable) ? "disabled" : "";
                    var disabledItem = Model.IsItemLocked ? "disabled" : "";
                %>
               
                <%if (Model.AssignTabSettings.ShowMakeGradeable)
                  { %>
                <li>
                    <label id="lblPoints"  style="<%= isShowPoints %>">
                        Points
                    </label>

                    <% if (Model.SourceType.ToLowerInvariant() != "pxunit" && Model.AssignTabSettings.ShowGradebookCategory)
                       { %>
                    <label for="Assignment_Syllabus">
                        Gradebook Category</label>

                    <%} %>
                </li>
                <li>
                    <span id="divGradePoints" style="<%= isShowPoints %>">
                         <% 
                           var pointVal = ""; 
                           if (Model.Score.Possible != 0)
                           {
                               pointVal = Model.Score.Possible.ToString();
                           }
                           %>
                        <input type="text" id="txtGradePoints" value="<%=pointVal %>" <%= disableGradable %> onkeypress="return ContentWidget.AllowNumbersOnly(event);"
                            onkeyup="ContentWidget.IsFormChanged()" />
                        <input type="hidden" id="txtGradePointsHidden" class="txtGradePointsHidden" value="<%=pointVal%>" />

                    </span>
                    <% if (Model.SourceType.ToLowerInvariant() != "pxunit" && Model.AssignTabSettings.ShowGradebookCategory)
                       { %>
                            <%Html.RenderPartial("GradeBookWeightsOptions", Model); %>
                   

                     <% var extracredit = Model.IsShowExtraCreditOption ? "" : "display:none;"; %>
                    <span style="<%=extracredit %>" >                 

                    <input type="checkbox" <%= disableGradeExtraCredit %> id="isAllowExtraCredit" name="isAllowExtraCredit" style="float:none;" <%=Model.IsAllowExtraCredit ? "checked='true'" : ""%> onchange="ContentWidget.IsFormChanged()"/>Extra credit</span>
                     <%} %>
                </li>
                <% } %>
              
                <% if (Model.AssignTabSettings.ShowCalculationType)
                   {%>
             
                <li id="divCalculationType" style="float:left;">
                    <label for="Assignment_CalculationType">
                        Calculation Type</label><br />
                    <select id="selCalculationTypeTrigger" <%= disableGradable %> name="selCalculationTypeTrigger" onchange="ContentWidget.IsFormChanged()">
                        <% 
                            // Changing the order of grade actions so that the default grade action is the first in the list
                            var availableGradeActions = Model.AvailableSubmissionGradeAction.ToArray();
                            if (Model.DefaultGradeAction != null && availableGradeActions[0].Key != Model.DefaultGradeAction.ToString() && availableGradeActions.Length > 1)
                            {
                                for (int i = 0; i < availableGradeActions.Length; i++)
                                {
                                    if (availableGradeActions[i].Key == Model.DefaultGradeAction.ToString() )
                                    {
                                        var temp = availableGradeActions[0];
                                        availableGradeActions[0] = availableGradeActions[i];
                                        availableGradeActions[i] = temp;
                                    }
                                }
                            }
                            foreach (var option in availableGradeActions)
                           {%>
                        <option value="<%=option.Key %>" <%= (option.Key == Model.SubmissionGradeAction.ToString())? "selected" : "" %>>
                            <%=option.Value.ToString().Replace("_", " ") %></option>
                        <% }%>
                    </select>
                </li>
                <%}%>

                  <% if (Model.AssignTabSettings.ShowIncludeScore)
                   { %>
                <%-- ShowIncludeScore , gradereleasedate--%>
          
                <li id="divIncludeGbbScore" style="float:left;">
                   <!--label id="Assignment_IncludeGbbScore">
                        Include score in gradebook:
                    </!--label><br />
                    <select <%= disabledItem %> id="selIncludeGbbScoreTrigger" name="selIncludeGbbScoreTrigger" onchange="ContentWidget.IsFormChanged()">
                        <option <%= (Model.IncludeGbbScoreTrigger == 1 ) ? "selected" : "" %>  value="1">After item is completed, or due date has passed</option>
                        <option <%= (Model.IncludeGbbScoreTrigger == 2 ) ? "selected" : "" %>  value="2">Only after due date has passed</option>
                        <option <%= (Model.IncludeGbbScoreTrigger == 0 ) ? "selected" : "" %> value="0">Always</option>
                    </select-->

                    <input type="checkbox" id="selIncludeGbbScoreTrigger" name="selIncludeGbbScoreTrigger" onchange="ContentWidget.IsFormChanged()" <%= Model.IncludeGbbScoreTrigger == 2 ? "checked" : string.Empty %> />
                    <span id="Assignment_IncludeGbbScore">Hide grade from student until due date has passed</span>
                </li>
                <% } %>

                

                <li id="startDateField" style="display: none;">
                    <label for="Assignment_StartDate">
                        Start Date:</label>
                    <% var ddlabelStart = Model.StartDate.Year == DateTime.MinValue.Year ? "N/A" : Model.StartDate.ToString("dddd MMM d, yyyy"); %>
                    <input type="text" class="readonly" readonly="readonly" id="StartDate" name="StartDate"
                        value="<%= ddlabelStart %>" />
                </li>
            </ol>
    </fieldset>
    <fieldset id="fsMarkAsComplete">
        <% 
            var markAsCompleteChecked = (Model.IsMarkAsCompleteChecked) ? "checked" : "";
        %>
        <ol>

            <li id="liMarkAsComplete" style="display:none;">
                <input type="checkbox" id="chkMarkAsComplete" class="chkMarkAsComplete" <%= markAsCompleteChecked %>  style="display:none;"/>
                <label for="Assignment_ShowMarkAsComplete">
                    Item is completed when the student:</label>
            </li>

        </ol>
    </fieldset>
    

