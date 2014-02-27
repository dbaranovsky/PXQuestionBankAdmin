<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Menu>" %>
<%
    var activeItemIndex = 0;
    var totalItemCount = Model.FlatCount-1; // adding -1 to ignore Primary Source Tab count
    if (Model.IncludeIndexNavigation)
    {
%>
<div class="menu-pagination">

<a class="print-button" href="#"></a>

    <% 
        if (activeItemIndex == 0)
        {    %>
        <a class="start-button" href="#">Start</a>
        <% }
       %>
      
      <span class="menu-numeric-seq">
        <a class="left-open nav-button" href="#"><</a>
        <span class="active-index" style="color: #bd2b4b;"><%=activeItemIndex%></span> of
        <span class="total"> <%=totalItemCount%> </span> <a class="right-open nav-button" href="#">></a>
    </span>
        
</div>


<% }  %>



