<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<ul>
    <li>
        <div id="divEbook">
            <div id="MLPTOC" class="MLPTOC">
                <div id="leftToc">
                    <div id="toc">
                    <% if (((IEnumerable<TocItem>)ViewData["toc"]).Any())
                       { %>
                        <ul class="mceebookeditor faux-tree" id="mceebookeditor">
                            <%  var vd = new ViewDataDictionary<IEnumerable<TocItem>>();
                                vd.Model =(IEnumerable<TocItem>) ViewData["toc"];
                                Html.RenderPartial("MceEbookItem", vd);
                            %>
                        </ul>
                      <% }
                       else
                       { %>
                        <div class="nocontent"> No Ebooks!</div>
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
<input type="hidden" id="tocURL" /> 
