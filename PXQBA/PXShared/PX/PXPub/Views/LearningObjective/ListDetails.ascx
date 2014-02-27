<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.LearningObjective>>" %>
 
 <%
 
     bool isItemLocked = ViewData["isItemLocked"] != null ? Convert.ToBoolean(ViewData["isItemLocked"]) : false;
 %>



<% if (Model.IsNullOrEmpty()) { %>
		This assignment does not yet have a learning objective.
<% } else { %>
		<ul>
            <%foreach (var lo in Model)
              {
                  var title = lo.Title;
                  var description = lo.Description;
                  try
                  {
                      title = HttpUtility.HtmlDecode(lo.Title);
                      description = HttpUtility.HtmlDecode(lo.Description);                      
                  }
                  catch
                  {

                  }
                  %>

              <li class="objective-title"><div><%=title%></div></li>
              <li class="objective-description"><div><%=description%></div></li>
        <%
              }
               %>
        </ul>
        
<% } %>

