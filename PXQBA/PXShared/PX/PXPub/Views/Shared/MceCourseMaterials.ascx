<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.CourseMaterials>" %>

<ul>
    <li>
        <div id="divCourseMaterials">
            <div id="MLPTOC" class="MLPTOC">
                <div id="leftToc">
                    <div id="toc">
                    <% if (Model.ResourceList.Any())
                       { %>

                        <ul class="mcecoursematerials">

                        <% foreach (var item in Model.ResourceList)
                           {%>
                            <li>
                                <a id="<%= item.Id %>" class="itemTitle"><%= item.Title.Trim()%></a>
                                <input id="tocItemUrl" type="hidden" value="<%= Url.Action("Index", "EbookBrowser", new { ItemId = item.Id, category = item.Categories.Last().Id }) %>" />
                            </li>

                        <% } %>

                        </ul>
                      <% }
                       else
                       { %>
                        <div class="nocontent"> No Course Materials!</div>
                      <% } %>
                    </div>
                      
                    </div>
                </div>
            </div>                                        
        </div>
        </li>
        <li class="menuitemtextli">Link text: <input  type="text" id="Title" class="menuItemTextBox"/> </li>
                             
        <li class="menuitembtns">
        <input type="button" id="btnAddLink" name="behavior" value="Create Link" />
        | <a href="" class="mceAddLinkClose">Cancel</a>
    </li>                            
</ul>



