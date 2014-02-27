<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.GradeData>" %>

<% var showAllAdditionalExport = ViewData["showAllAdditionalExport"] != null ? (bool)ViewData["showAllAdditionalExport"] : false; %>
<!-- View code for the "Other Scores" section of the gradebook-->
<div id="otherscores">
    <div id="viewByOptions">
        <form>
            <div id="groupby">
                <input type="radio" id="byItem" name="groupView" checked="checked"/><label for="byItem">View by Item</label>
                <input type="radio" id="byStudent" name="groupView"/><label for="byStudent">View by Student</label>
            </div>
        </form> 
    </div>
    <div id="otherScoresContent">
        <table id="itemgroup" class="hide">
            <thead>
                <tr>
                    <th class="nameCol">
                        <div class="headerBox">   
                            Item Name
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <th class="avgCol">
                        <div class="headerBox">  
                            Avg. Score
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <th class="submissionCol">
                        <div class="headerBox">  
                            Last Submission
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                </tr>
            </thead>
            <tbody>
                <% 
                    foreach (var item in Model.ItemGrades) 
                    { %>
                        <tr id="<%=item.ItemId %>" gradeRule="<%=item.GradeRule %>" ref="<%= Url.GetComponentHash("gradebook", "itemScores", new Dictionary<string,object>() { 
                                                                                                { "itemid", item.ItemId }, 
                                                                                                { "itemType", "Item" }, 
                                                                                                { "username", item.ItemTitle }
                                                                                            }) %>">
                            <td class="name"><%=item.ItemTitle %></td>
                            <td class="average"><%=item.GradeDisplay %></td>
                            <td class="submission"><%=item.SubmittedDate %></td>
                        </tr>
                <%  }   
                %>
            </tbody>
        </table>
        <table id="studentgroup" class="hide">
            <thead>
                <tr>
                    <th class="nameCol">
                        <div class="headerBox">  
                            Student
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <th class="avgCol">
                        <div class="headerBox">
                            Avg. Score
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                    <th class="submissionCol">
                        <div class="headerBox">  
                            Last Submission
                            <span class="sortIcon"></span>
                        </div>
                    </th>
                </tr>
            </thead>
            <tbody>
                <% 
                    foreach (var item in Model.StudentGrades) 
                    { %>
                        <tr enrollmentid="<%=item.EnrollmentId %>" ref="<%= Url.GetComponentHash("gradebook", "itemScores", new Dictionary<string,object>() { 
                                                                                                { "itemid", item.EnrollmentId }, 
                                                                                                { "itemType", "Student" }, 
                                                                                                { "username", item.EnrollmentName }
                                                                                            }) %>">
                            <td class="name"><%=item.EnrollmentName%></td>
                            <td class="average"><%=item.GradeDisplay%></td>
                            <td class="submission"><%=item.SubmittedDate%></td>
                        </tr>
                <%  }   
                %>
            </tbody>
        </table>
    </div>
</div>
<%= Html.Hidden("hdnClassAvg", string.Format("{0:0.0%}", Model.ClassAverage), new { @class = "hdnClassAvg", id = "hdnClassAvg" })%>
<%= Html.Hidden("showAllAdditionalExport", showAllAdditionalExport.ToString().ToLower(), new { @class = "showAllAdditionalExportBtn", id = "showAllAdditionalExportBtn" })%>