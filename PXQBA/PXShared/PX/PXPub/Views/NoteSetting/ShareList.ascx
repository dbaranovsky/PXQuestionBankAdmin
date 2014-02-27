<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.Student>>" %>

 <div class="shareList">
         <% if (Model != null) 
           {
                foreach (var item in Model)
                {           
                %>
                    <div class="shared-student">
                        <span class="shared-student-name"><%= string.Format("{0} {1}",item.FirstName,item.LastName) %></span>
                        <% if(!item.IsInstructor) { %>
                            <span class="remove"></span>
                            <%= Html.Hidden("SharedStudentId", item.Id )%>
                            <%= Html.Hidden("SharedStudentName", (item.FirstName + " "+ item.LastName))%>
                            <%= Html.Hidden("SharedStudentFirstName", item.FirstName )%>
                        <%} %>
                    </div>                        
                <%} %>
        <%} %>
</div>
<div class="added-info-message">
            
</div>
