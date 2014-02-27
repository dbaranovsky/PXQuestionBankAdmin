<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

 <table class="bow_step3">
    <tr>
        <td colspan="1"><input type="checkbox" value="1" class="bowOperationStatus"/> <label>Status</label></td>
        
         <td class="bowDropdown" style="width:80%;">

                            <%
                                var statusList = new SelectList(new[]{
                                  new SelectListItem{ Text="-1", Value= string.Empty },
                                  new SelectListItem{ Text="0", Value=ExtensionMethods.GetEnumDescription(QuestionStatusType.InProgress)},
                                  new SelectListItem{ Text="1", Value=ExtensionMethods.GetEnumDescription(QuestionStatusType.AvailabletoInstructor)},
                                  new SelectListItem{ Text="2", Value=ExtensionMethods.GetEnumDescription(QuestionStatusType.Deleted)},
                                }, "Text", "Value");
                            
                            %>
                                <%= Html.DropDownList("bow-QuestionStatus", statusList, new { @class = "select2 single bowDDSize" })%>
                                    
                            </td>
    </tr>
    </table>