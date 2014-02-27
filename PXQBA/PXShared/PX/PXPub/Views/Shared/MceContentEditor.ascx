<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<ul>
    <li>
        <div id="divContent">
            <div id="MLPTOC" class="MLPTOC">
                <div id="leftToc">
                    <div id="toc">
                    <% if (((IEnumerable<TocItem>)ViewData["toc"]).Any())
                       { %>
                        <ul class="mcecontenteditor faux-tree" id="mcecontenteditor">
                            <%  var vd = new ViewDataDictionary<IEnumerable<TocItem>>();
                                vd.Model = (IEnumerable<TocItem>)ViewData["toc"];
                                Html.RenderPartial("MceTocItem", vd);
                            %>
                        </ul>
                      <% }
                       else
                       {%>
                       <div class="nocontent"> No Content!</div>
                      <% } %>
                      
                    </div>
                </div>
            </div>
            <div id="ignoreShowContent"></div>
            <input type="hidden" runat="server" id="minSequence" name="minSequence" class="minSequence" />
            <input type="hidden" runat="server" id="maxSequence" name="maxSequence" class="maxSequence" />
        </div>
        </li>
        <li class="menuitemtextli">Link text: <input  type="text" id="Title" class="menuItemTextBox"/> </li>
                             
        <li class="menuitembtns">
        <input type="button" id="btnAddLink" name="behavior" value="Create Link" />
        | <a href="" class="mceAddLinkClose">Cancel</a>
    </li>                            
</ul>

