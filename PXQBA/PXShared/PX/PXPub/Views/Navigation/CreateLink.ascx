<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Link>" %>
  
  <% var path = Url.RouteUrl("CourseSectionHome");%>
  
<div style="text-align:right">
<% using (Ajax.BeginForm("SaveLink", "Navigation", new AjaxOptions() { OnSuccess="window.location = '" + path + "';" }))
   { %>       
    
       <ol>
           <li>
               <%= Html.LabelFor(model => model.ParentId) %>
               <%= Html.TextBoxFor(model => model.ParentId) %>
               <%= Html.ValidationMessageFor(model => model.ParentId) %>
           </li>
           <li>
               <%= Html.LabelFor(model => model.Sequence) %>
               <%= Html.TextBoxFor(model => model.Sequence) %>
               <%= Html.ValidationMessageFor(model => model.Sequence) %>
           </li>
           <li>
               <%= Html.LabelFor(model => model.Title) %>
               <%= Html.TextBoxFor(model => model.Title) %>
               <%= Html.ValidationMessageFor(model => model.Title) %>
           </li>
           <li>
               <%= Html.LabelFor(model => model.Url) %>
               <%= Html.TextBoxFor(model => model.Url) %>
               <%= Html.ValidationMessageFor(model => model.Url) %>
           </li>
           <li>
               <p>
                   <input type="submit" value="Save" />
               </p>
           </li>
       </ol>

    <% } %>

    <div>
        <%= Html.ActionLink("Back to List", "Index") %>
    </div>
</div>