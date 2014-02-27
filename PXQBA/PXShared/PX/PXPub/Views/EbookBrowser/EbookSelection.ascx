<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Bfw.PX.PXPub.Models.Ebook>>" %>

<% 
    var accessLevel = (ViewData["accessLevel"] == null) ? "" : ViewData["accessLevel"].ToString();
%>
<div class="ebook-selection">
<% if (Model != null)
   {
       foreach (var book in Model)
       {
           if ( !string.IsNullOrEmpty( book.Title ) && (accessLevel.ToLowerInvariant() == "instructor" || (accessLevel.ToLowerInvariant() == "student" && !book.HiddenFromStudents)))
           { %>
           <div class="ebooks-wrapper">
               <div><a class="ebook-cover-image <%= book.Title %> " href="<%= Url.Action( "Index", "EbookBrowser", new { itemId = book.RootId, category = book.CatagoryId } )%>" target="_blank">
                        <img src="<%= book.CoverImage %>" alt="<%= book.Title %>" width="80%" height="80%"/></a></div>
               <div>
                   <div class="ebook-link"><a href="<%= Url.Action( "Index", "EbookBrowser", new { itemId = book.RootId, category = book.CatagoryId } )%>" target="_blank">Go to <%= book.Title %></a></div>
                   <div class="ebook-title"><a href="<%= Url.Action( "Index", "EbookBrowser", new { itemId = book.RootId, category = book.CatagoryId } )%>" target="_blank"><%= book.Title %></a></div>
                   <div class="ebook-subtitle"><%= book.Subtitle %></div>
                   <div class="ebook-author">by <%= book.Authors %></div>
                   <div class="ebook-description"><%= book.Description %></div>
               </div>
           </div> 
       <% }
       }
   }
%>
</div>
