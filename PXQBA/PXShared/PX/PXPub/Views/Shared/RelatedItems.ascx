<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<Bfw.PX.PXPub.Models.RelatedItems>>" %>
<% var count =  ViewData["Count"].ToString();
   bool bShowRelatedContent = bool.Parse(ViewData["ShowRelatedContent"].ToString());
%>
<% if (bShowRelatedContent)
   { %>
<div id="eLibraryButton">
    <a class="relatedContentButton <%= ViewData["Count"].ToString()=="0"?"disabled":"" %>" href="">RELATED CONTENT (<%= ViewData["Count"].ToString()%>)</a>
</div>

<div id="eLibraryEditor" class="elibrary-cmp" style="display:none;width: 285px; background-color: transparent; -webkit-box-shadow: rgb(136, 136, 136) 0px 0px 0px; box-shadow: rgb(136, 136, 136) 0px 0px 0px;">
    <div id="eLibraryLeft" style="display: none;">
        <a class="close-blackWhite close-seeAll">x</a>
        <div id="eLibraryLeftHeader">
        <span class="eLibraryHeaderTitle">Preview</span>
        </div>
    </div>
    <div id="eLibraryComponent" style="display: none;">
    </div>
    <div id="eLibraryContent">
<%--        <h3 id="H1" style="">Searching...</h3>--%>

    <ul class="related-content">
    <%  foreach (var category in Model)
        { 
    %>            
           <li class="category-header"><div class="image"></div><span class="category"><%= category.Category%></span></li>      

        <% foreach (var relatedItem in category.Items)
           {
            %>
            <%-- this will probably Be done server side but I wanted to get something working in this space hudson --%>
                   <%  
                       var imageType = "pxicon-assessment";
                       if (relatedItem.Item.Type == "ExternalContent")
                       {
                         imageType = "pxicon-figure";
               
                    if (relatedItem.Item.Type == "Quiz")
                    {
                        imageType = "pxicon-assessment"; 
                    }
                   
                       } %>
            <li class="relatedItem" data-item-id="<%= relatedItem.Item.Id %>" type="<%= relatedItem.Item.Type %>" sub-type="<%= relatedItem.Item.SubType %>" meta-content-type="<%= relatedItem.Item.FacetMetadata!=null && relatedItem.Item.FacetMetadata.ContainsKey("meta-content-type")?relatedItem.Item.FacetMetadata["meta-content-type"].ToString():"" %>">
              <div class="image pxicon <%= imageType %>"></div><span class="related-item-title"><%= relatedItem.Item.Title%></span> <br /> 
              <a class="pxlinkbutton" href="<%=relatedItem.PreviewHash %>">Preview</a>
            </li>
            <%          
                 
           
        %>      
        <%          
                 
           }
        }%>
    </ul>
    </div>
</div>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/RelatedContent/RelatedContent.js") %>'], function () {
                PxRelatedContent.InitRelatedItems();
                var button = $("#eLibraryButton");
                if (parseInt("<%= Model.Count().ToString() %>", 10) == 0) {
                    button.qtip({
                        content: "No Related Content found",
                        show: "mouseover",
                        hide: "mouseout"
                    });
                };
            });
        });
    } (jQuery));  
</script>
<% }  %>