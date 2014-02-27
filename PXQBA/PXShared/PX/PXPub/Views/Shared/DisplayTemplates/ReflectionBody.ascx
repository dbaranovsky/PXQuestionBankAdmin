<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ResourceDocument>" %>

<% 
    if (!String.IsNullOrEmpty(Model.body))
   { %>
        <div class="submission-body px-default-text">
           
          <%=HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(Model.body))%>
            <% string id = Model.url.Split('/').Last();
               id = id.Substring(0, id.Length - 6);               
            %>
            <input type="hidden" name="saved-reflection-id" value=<%=id%>/>
        </div>
 <% } %>