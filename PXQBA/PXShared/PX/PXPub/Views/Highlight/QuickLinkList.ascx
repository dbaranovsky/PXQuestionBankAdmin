<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.QuickLink>>" %>

<%
        if (!Model.IsNullOrEmpty())
        {
        %>
        <select class="listLinkLibrary" multiple="single" size="5">
            <option value="" title="Select from link library" selected="selected">Select an eBook link from quicklinks list:</option>
            <%            
                foreach (var link in Model)
                {
                %>
                    <option title="<%=link.Title%>" value="<%=string.IsNullOrEmpty(link.LinkedItemId)? link.LinkUrl: link.LinkedItemId%>"><%=link.Title%></option>
                <%
                }
            %>            
        </select>
        <% } else { %>
            <select class="listLinkLibrary" disabled="disabled">
            <option value="" selected="selected">No links found</option>
            </select>
        <% } 
%>


