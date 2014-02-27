<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Menu>" %>

<%= ResourceEngine.IncludesFor("~/Scripts/contentwidget.js") %> 
<div class="addlinkdialog">
<div id="addMenu-wrapper">

<%var saveText = "Create Link"; %>

    <div id="tabVerticalMenu">
        <ol id="olMenulist" class="popup-items-text">
            <li><input type="radio" name="rdoItem" class="rdoItemContent" checked="checked" /> Link to course contents</li>
            <li><input type="radio" name="rdoItem" class="rdoItemCreateMyOwn" /> Add a link</li>
         </ol>
    </div>
    <div id="tabContainer" >
        <ul> 
            <li>
                <div id="ContentEditor" class="muItemEditorRegion muEditionRegionContentEditor">                    
                    
                    <h2 class="divPopupTitle rightnav-title" style="width: 100%;">                    
                       <a href="" class="lnkContentEditor">Content</a> <span class="divider"> | </span> <a href="" class="lnkEbookEditor">e-Book Content</a> <span class="divider"> | </span> <a href="" class="lnkCourseMaterials">Course Materials</a>
                    </h2>

                     <div id="divContentEditor">     
                        <% Html.RenderAction("MceContentEditor", "PageAction"); %>
                     </div>  
                    
                    <div id="divEbookEditor">
                    </div>  

                    <div id="divCourseMaterialsEditor" class="muItemEditorRegion muEditionRegionBookMark">
                    </div>
                    
                </div>
             </li>
              <li>
              <div id="ManualEditor" class="muItemEditorRegion muEditionRegionManual">                                           
                           
                          <ol class="formlist">
                              <li class="menuitemtextli">Title: <%=Html.TextBox("Title", "", new { id = "Title", @class = "menuItemTextBox" })%>  
                                <br />
                                <label class="error important-alert" for="Title"  id="titleError" style="display:none;color:red">Please enter a title</label>
                              </li>
                              <li class="menuitembtns">URL: <%=Html.TextBox("Url", "", new { id = "Url", @class = "menuItemTextBox", @style = "margin-left: 3px;" })%>
                                <br />                                
                                <label class="error important-alert" for="Url" id="urlError" style="display:none;color:red">Please enter a valid URL</label>  
                                <br />  
                                <br />                     
                              </li>                              
                              <li>    
                              <input type="button" id="btnAddCustomLink" style="font-size: 12px" name="behavior" value="<%=saveText%>" />                                                                
                                    | <a href="" class="mceAddLinkClose" style="font-size: 12px">Cancel</a>
                            </li>
                        </ol>             
                         <input type="hidden" id="tocURL" /> 
                         <input id="hdnSelectedHTML" type="hidden" value="" />
                         <input id="hdnSelectedId" type="hidden" value="" />
                         <input id="hdnSelectedText" type="hidden" value="" />
                         <input id="hdnIsSelectedText" type="hidden" value="false" />
                         <input id="hdnTitle" type="hidden" value="" />
                   </div>
              </li>
             </ul>
        </div>

</div>
</div>



<script type="text/javascript">
    (function ($) {

        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/tinymceaddlink.js") %>'],
            function () {
                PXMceAddLink.BindControls();
             });
        });

    } (jQuery));    
</script>
