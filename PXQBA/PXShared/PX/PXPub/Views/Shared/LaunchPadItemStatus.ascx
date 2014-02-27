<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.TreeWidgetViewItem>" %>
<%
    var item = Model.Item;
    bool grayoutpastduelater = Model.Settings.GreyoutPastDue;

    bool showDueTime = ViewData["showDueTime"] == null ? false : Convert.ToBoolean(ViewData["showDueTime"]) && item.IsAssigned;
    var timeZone = ViewData["timeZone"] == null ? string.Empty : ViewData["timeZone"].ToString();
    
    string calendarPastDueVisibility = string.Empty;
    if (grayoutpastduelater && item.DueDate <= DateTime.Now.GetCourseDateTime() && item.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString())
    {
        calendarPastDueVisibility = " grayout";
    }
    bool isBarren;
    if (item.Type.ToLowerInvariant() == "pxunit")
    {
        isBarren = false;
    }
    else
    {
        isBarren = true;
    }
    
 %>
   <%var ddlStartDateMonth = item.StartDate.Year < 1900 ? "" : item.StartDate.ToString("MMM"); %>
    <%var ddlStartDateDay = item.StartDate.Year < 1900 ? "" : item.StartDate.ToString("dd"); %>
    <%var ddDueDateMonth = item.DueDate.Year < 1900 ? "" : item.DueDate.ToString("MMM"); %>
    <%var ddDueDateDay = item.DueDate.Year < 1900 ? "" : item.DueDate.ToString("dd"); %>

        <% var showDateDiv = (ddlStartDateDay != "" || ddDueDateDay != "");
           var dueSoon =
               ((item.StartDate.Date.Year > DateTime.MinValue.Year) && (item.StartDate.Subtract(TimeSpan.FromDays(7)) < DateTime.Now)) ||
               ((item.DueDate.Date.Year > DateTime.MinValue.Year) && (item.DueDate.Subtract(TimeSpan.FromDays(7)) < DateTime.Now)); %>
        <div class='due_date pxunit-display-duedate <%= dueSoon ? "due_soon" : ""  %> <%=calendarPastDueVisibility %>' style='display: <%= showDateDiv?"block":"none" %>' >
            <div class="dd_cal">
                <div class="dd_cal_month">
                    <span class="dd_cal_month_inner">
                        <% if (isBarren || ddlStartDateMonth == ddDueDateMonth || ddlStartDateMonth == "")
                           {%>
                        <span class="due_date_month">
                            <%= ddDueDateMonth %>
                        </span>
                        <% }
                           else
                           { %>
                        <span class="start_date_month">
                            <%= ddlStartDateMonth %>
                        </span>- <span class="due_date_month">
                            <%= ddDueDateMonth %>
                        </span>
                        <% } %>
                    </span>
                </div>
                <div class="dd_cal_date">
                    <% if (isBarren || ddlStartDateDay == ddDueDateDay || ddlStartDateDay == "")
                       {%>
                    <span class="due_date_day">
                        <%=ddDueDateDay%>
                    </span>
                    <%}
                       else
                       { %>
                    <span class="start_date_day">
                        <%=ddlStartDateDay%>
                    </span>- <span class="due_date_day">
                        <%=ddDueDateDay%>
                    </span>
                    <%} %></div>
            </div>
        </div>
        <div class="pxunit-display-points col">
            <% if (isBarren)
               { 
                   if (item.MaxPoints > 0)
                    {
                   // to do: need to get student's points that were graded by the instructor, the instructor can not currently grade students
                        string scoreTempStr = "";
                        if(item.IsUserSubmitted && item.Score > -1)
                        {
                            var score = Math.Floor(item.Score * 100);
                            scoreTempStr = (score / 100).ToString();    
                        }
                    %>
                    <%= ((item.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student && item.IsUserSubmitted) ? 
                    (item.IsUserSubmitted && item.Score > -1 ? scoreTempStr : "<span class='achievedPoints'>&mdash;") +  " / </span>" : "") + item.MaxPoints.ToString() + (item.MaxPoints > 1 ? " pts" : " pt") %>
                    <%}

                   if (showDueTime)
                   {                      
                    %>                    
                        &nbsp;
                        <%= String.Format("{0} {1}", item.DueDate.ToString("hh:mm tt"), timeZone)%>  
                    <%
                   }
                }
            %></div>
        <div class="col faceplate-item-assign">
            <span></span>
            <% if (item.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student && item.StudentItemsAssigned > 0)
               { %>
            <div class="status-percentage col">
                <div class="comp-per-full" >
                    <div class="complete-percentage" style="width:<%= item.StudentCompletedPercentage + "%" %>">
                    </div>
                </div>
                <div class="ave-per-full">
                    <%
                        int averagePercentage = item.StudentScorePercentage > 100 ? 100 : item.StudentScorePercentage;
                     %>
                    <div class="average-percentage" style="width:<%= averagePercentage + "%" %>">
                    </div>
                </div>

                <div class="chapter-student-status" >
                    <div class="complete_tooltip_text"><b><span class="completed-number"><%=(item.StudentCompletedItems + "/" + item.StudentItemsAssigned )%></span></b> Graded activities complete</div>
                    <div class="grade_tooltip_text">Score for graded activities: <b><span class="completed-grade"><%=item.StudentScorePercentage %>%</span></b></div>
                </div>

            </div>
            <%
               //var dateDifference = (Convert.ToDateTime(item.DueDate) - DateTime.Today).Days;
               //if ((dateDifference >= 0 && dateDifference < 8))
               //{ 
               //     <div class="due-clock col show-due-clock">
               //     </div>
               //}
               //else
               //{
               // <div class="due-clock col">
               // </div>
               //}
           } %>
        </div>
            <% if (item.UserAccess == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student && item.IsUserSubmitted && item.IsAssigned)
               { %>
                    <span class="item-submitted"></span>
            <% } %>
          