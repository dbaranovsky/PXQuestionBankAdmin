<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.RelatedContent>" %>

<div id="related-content-editor">
<div class="relatedContent">
    <div class="heading">
        <span>Related Content:</span>
        <a href="#" class="add-related-resource">+ Add Related Resource</a>
    </div>
    <div class="descriptions">       
        <div>
            <%
                var ResourceTitle = (Model != null) ? Model.Title : string.Empty;
             %>
            <span class="title">Title:</span>        
            <input type="text" value='<%=ResourceTitle %>' class="related-content-text" />
        </div>

        <div class="related-content-grid">
            
            <%
                if (Model != null)
                {
                    if (!Model.RelatedContents.IsNullOrEmpty())
                    {
                        %>
                        <table>
                        <%
                            foreach (var item in Model.RelatedContents)
                            {
                                var title = item.Title;
                                var id = item.Id;
                        %>                    
                            <tr>
                                <td>
                                    <a href="#" class="related-content-title"><%=title%></a>
                                </td>
                                <td>
                                    <a href="#" class="remove-related-content">Remove</a>
                                    <span class="related-content-id" style="display:none"><%=id%></span>
                                </td>                    
                            </tr>

                            <%} 
                        %>                    
                    </table>
                    <%  
                    }
                }
                if(Model == null || Model.RelatedContents.IsNullOrEmpty())
                {%>
                    <span class="no-content">Click "Add Related Resource" to add resources.</span>                
               <%}%>            
        </div>
        
    </div>
</div>
<div class="related-content" style="display:none;">
    <ul class="related-content-description" style="width:500px">
            <%
                    if (Model != null)
                    {
                        if (!Model.RelatedContents.IsNullOrEmpty())
                        {

                            foreach (var item in Model.RelatedContents)
                            {
                                var title = item.Title;
                                var id = item.Id;
                        %>  
                            <li>
                                <span><%=title%></span>
                            </li>                                          
                        <%
                            }                        
                        }
                   }
            %>
    </ul>
 </div>
 </div>
