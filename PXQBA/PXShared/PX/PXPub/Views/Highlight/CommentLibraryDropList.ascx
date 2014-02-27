<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.Note>>" %>

    <%
        if (!Model.IsNullOrEmpty())
        {
            %>
        <select class="dlCommentLibrary">
        <option value="" title="Select from comment library" selected="selected">Type in a word to refine comment listing or click the arrow to select</option>
        <%            
            foreach (var comment in Model)
            {
                %>
                <option title="<%=comment.Title%>" value="<%=comment.Text%>"><%=comment.Title%></option>
                <%
            }
            %>
            
        </select>
        <% } else { %>
        <select class="dlCommentLibrary" disabled="disabled">
        <option value="" selected="selected">No comments found</option>
        </select>
        <% } %>
