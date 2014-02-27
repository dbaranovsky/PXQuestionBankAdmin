<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Bfw.PX.PXPub.Models.RelatedTemplate>>" %>



    <fieldset id="ePortfolioFields">
        <div class="divAllowedTemplates">
            <% if (Model != null)
               {
            %>
            <ul>
                <%
                                       foreach (var relatedTemplate in Model)
                                       {%>
                <li>
                    <%
                                           Html.RenderPartial("AllowedContentTemplate", relatedTemplate);
                    %>
                </li>
                <% } %>
            </ul>
            <%
                                   }
            %>
        </div>
        </fieldset>
