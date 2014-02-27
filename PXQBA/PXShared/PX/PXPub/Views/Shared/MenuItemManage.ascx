<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Menu>" %>

<%= ResourceEngine.IncludesFor("~/Scripts/contentwidget.js") %> 

<%var OnSuccess  = "function (response) { PxMenuConfig.DisplayNewMenuItem(response); PxPage.Loaded();}"; %>



<div id="addMenu-wrapper">

<input type="hidden" id="SelectedItem_Id" value = "<%=Model.SelectedMenuItem.Id %>" />
<input type="hidden" id="SelectedItem_BfwMenuCreatedby" value = "<%=Model.SelectedMenuItem.BfwMenuCreatedby %>" />

<%var saveText = "Create Tab";
  if (!string.IsNullOrEmpty(Model.SelectedMenuItem.Id))
  {
      saveText = "Save";
  }%>

<%=Html.Hidden("SelectedItem_Param_Id", Model.SelectedMenuItem.ContentItemId)%>

    <div id="tabVerticalMenu">
        <h2 id="leftnav-title" class="divPopupTitle">
            Create custom tab <span id="link_action_menu" class="action_menu"></span>
        </h2>
        <ol id="olMenulist" class="popup-items-text">
            <li><input type="radio" name="rdoItem" class="rdoItemContent" /> Choose tab from course contents</li>
            <li><input type="radio" name="rdoItem" class="rdoItemCreateMyOwn" /> Create my own tab</li>
         </ol>
    </div>
    <div id="tabContainer" >
        <ul> 
            <li>
                <div id="ContentEditor" class="muItemEditorRegion muEditionRegionContentEditor">
                    <h2 class="divPopupTitle rightnav-title" style="width: 100%;">
                        <a href="" class="miContentLink">Content</a> <span class="divider"> | </span> <a href="" class="miToolsLink">Tools</a>
                    </h2>
                    <div id="divEbookEditor">
                    <% using (Ajax.BeginForm("SaveMenuItem", "PageAction", new AjaxOptions() { }, new { id = "saveFromContent" }))
                            {
                                OnSuccess = "function (response) { PxMenuConfig.DisplayNewMenuItem(response, '" + Model.SelectedMenuItem.Id + "'); PxPage.Loaded();}";
                           %>
                        <ul>
                            <li>
                                <div id="divEbook">
                                    <div id="MLPTOC" class="MLPTOC">
                                        <div id="leftToc">
                                            <div id="toc">
                                                <ul>
                                                    <%  var vd = new ViewDataDictionary<IEnumerable<TocItem>>();
                                                        vd["includeDiscussion"] = true;
                                                        vd["category"] = ViewData["category"];
                                                        vd["HasUserMaterials"] = ViewData["HasUserMaterials"];
                                                        Html.RenderPartial("ExpandSection", ViewData [ "toc"], vd); 
                                                    %>
                                                </ul>
                                               </div>
                                        </div>
                                    </div>
                                    <div id="ignoreShowContent"></div>
                                    <input type="hidden" runat="server" id="minSequence" name="minSequence" class="minSequence" />
                                    <input type="hidden" runat="server" id="maxSequence" name="maxSequence" class="maxSequence" />
                                </div>
                             </li>
                             <li class="menuitemtextli">Title: <%=Html.TextBox("Title", Model.SelectedMenuItem.Title, new { id = "Title", @class = "menuItemTextBox", keyup = "" })%></li>
                             <li class="menuitemvisible"><%=Html.CheckBox ( "VisibleByStudent", Model.SelectedMenuItem.VisibleByStudent, new {id="VisibleByStudent"} )%> <label for="VisibleByStudent">Allow students to see this tab</label> </li>
                             <li class="menuitembtns">
                                <input type="button" name="behavior" value="<%=saveText%>" onclick="if (PxMenuConfig.ValidateTitle ('saveFromContent #Title')) {  PxPage.OnFormSubmit('Processing...', true, { form: '#saveFromContent', data: { behavior: 'template' }, updateSelector: '#content-item', success: <%= OnSuccess %>});}" />
                                | <a href="" class="menuItemClose">Cancel</a>
                            </li>
                        </ul>
                             <%=Html.Hidden("ContentItemId", Model.SelectedMenuItem.ContentItemId, new { id = "ContentItemId" })%>
                             <%=Html.Hidden("BfwMenuCreatedby", "PX_MENU_ITEM_CONTENT_TEMPLATE", new { id = "BfwMenuCreatedby" })%>
                             <%=Html.Hidden("Id", Model.SelectedMenuItem.Id, new { id = "Id" })%>    
                             <%=Html.Hidden( "minSequence", "", new { @class = "minSequence" })%>
                             <%=Html.Hidden( "maxSequence", "", new { @class = "maxSequence" })%>   
                             <%=Html.Hidden("VisibleByInstructor", Model.SelectedMenuItem.VisibleByInstructor, new { id = "VisibleByInstructor" })%>    
                             <%=Html.Hidden("OriginalText", Model.SelectedMenuItem.Title, new { id = "OriginalText" })%>                  
                         <%} %>
                    </div>                     

                    <div id="divToolsEditor" class="muItemEditorRegion muEditionRegionTools">    

                        <% using (Ajax.BeginForm("SaveMenuItem", "PageAction", new AjaxOptions() { }, new { id = "saveFromTemplate" }))
                            { %>
                            <h1 class="divPopupTitle" style="width: 100%;">Select an existing Menu item</h1>
                                                                                                                                                                                                    <ol class="formlist">
                            <li></li>
                            <li class="menuitemtextli" style="padding-bottom:5px;">
                                <select name="drop1" id="selExistingLink" size="10" style="width: 400px;" onchange="PxMenuConfig.SelectExistingTemplateMenuItem(this);">

                                <% OnSuccess = "function (response) { PxMenuConfig.DisplayNewMenuItem(response, '', '#saveFromTemplate #BfwMenuCreatedby'); PxPage.Loaded();}";%>

                                    <%bool isEditing = Model.MenuItemTemplates.Exists(i => i.Id == Model.SelectedMenuItem.BfwMenuCreatedby); %>
                                    <%foreach (var templateItem in Model.MenuItemTemplates)
                                    {
                                        if (templateItem.Id == "PX_MENU_ITEM_CUSTOM_TEMPLATE")
                                            continue; %>
                                        
                                      
                                        <option value="<%=templateItem.Id%>" <%=(isEditing || Model.MenuItems.Exists(i => i.BfwMenuCreatedby == templateItem.Id)) ? "disabled='disabled'" : "" %>  <%= (templateItem.Id.Contains (Model.SelectedMenuItem.BfwMenuCreatedby) && !string.IsNullOrEmpty(Model.SelectedMenuItem.BfwMenuCreatedby)) ? "selected='selected'" : "" %>><%=templateItem.Title%></option>
                                    <%} %>
                                </select>
                            </li>
                            <li class="menuitembtns" style="padding-bottom:5px;">Title:                                    
                                <%if (string.IsNullOrEmpty(Model.SelectedMenuItem.Id))
                                    { %>                                    
                                    <%=Html.TextBox("Title", Model.SelectedMenuItem.Title, new { id = "Title", disabled = "true", @class = "menuItemTextBox" })%>  
                                <%}
                                    else
                                    { %>
                                    <%=Html.TextBox("Title", Model.SelectedMenuItem.Title, new { id = "Title", @class = "menuItemTextBox" })%>  
                                <%} %>
                            </li>
                            <li class="menuitemvisible"><%=Html.CheckBox ( "VisibleByStudent", Model.SelectedMenuItem.VisibleByStudent, new {id="VisibleByStudent"} )%> <label for="VisibleByStudent">Allow students to see this tab</label> </li>
                            <li style="padding:5px 0;">
                                <%=Html.Hidden("BfwMenuCreatedby", Model.SelectedMenuItem.BfwMenuCreatedby, new { id = "BfwMenuCreatedby" })%>
                                <%=Html.Hidden("Id", Model.SelectedMenuItem.Id, new { id = "Id" })%>
                                <input type="button" <%=string.IsNullOrEmpty(Model.SelectedMenuItem.Id) ? "disabled='disabled'" : "" %> name="behavior" id="btnSave" value="<%=saveText%>" onclick="if ( PxMenuConfig.ValidateTitle('saveFromTemplate #Title') ) { PxPage.OnFormSubmit('Processing...', true, { form: '#saveFromTemplate', data: { behavior: 'template' }, updateSelector: '#content-item', success: <%= OnSuccess %>});}" />
                                |<a href="" class="menuItemClose">Cancel</a>
                            </li>
                            </ol>
                                <%=Html.Hidden( "minSequence", "", new { @class = "minSequence" })%>
                                <%=Html.Hidden( "maxSequence", "", new { @class = "maxSequence" })%>  
                                <%=Html.Hidden("VisibleByInstructor", Model.SelectedMenuItem.VisibleByInstructor, new { id = "VisibleByInstructor" })%>  
                                <%=Html.Hidden("OriginalText", Model.SelectedMenuItem.Title, new { id = "OriginalText" })%>                                       
                          <%} %>
                    </div>
                </div>
             </li>
              <li>
              <div id="ManualEditor" class="muItemEditorRegion muEditionRegionManual">
                  <% using (Ajax.BeginForm("SaveMenuItem", "PageAction", new AjaxOptions() {}, new { id = "saveItemManual" }))
                     { %>
                      
                          <% var manualMenuItem = Model.SelectedMenuItem == null ? new Bfw.PX.PXPub.Models.MenuItem() : Model.SelectedMenuItem;%>
                          <% var url = manualMenuItem.Callbacks.IsNullOrEmpty() ? "" : manualMenuItem.Callbacks.First().Value.Url; %>

                          <% OnSuccess = "function (response) { PxMenuConfig.DisplayNewMenuItem(response, '" + manualMenuItem.Id + "'); PxPage.Loaded();}"; %>
                          <h2 class="divPopupTitle rightnav-title" style="width: 100%;">Create a custom tab</h2>

                           <%=Html.Hidden ("BfwMenuCreatedby", "PX_MENU_ITEM_CUSTOM_TEMPLATE") %>
                           
                          <ol class="formlist">
                              <li class="menuitemtextli">Title: <%=Html.TextBox("Title", manualMenuItem.Title, new { id = "Title", @class = "menuItemTextBox" })%>  
                              <label class="error important-alert" for="Title"  id="titleError" style="display:none;color:red">Please enter a title</label>
                              </li>
                              <li class="menuitembtns">URL: <%=Html.TextBox("Url", url, new { id = "Url", @class = "menuItemTextBox"})%>
                                <label class="error important-alert" for="Url" id="urlError" style="display:none;color:red">Please enter a valid URL</label>                         
                              </li>
                              <li class="menuitemvisible"><%=Html.CheckBox("VisibleByStudent", Model.SelectedMenuItem.VisibleByStudent, new { id = "VisibleByStudent"})%> <label for="VisibleByStudent">Allow students to see this tab</label> </li>
                              <li>                              
                                  <%=Html.Hidden("minSequence", "", new { @class = "minSequence" })%>
                                  <%=Html.Hidden("maxSequence", "", new { @class = "maxSequence" })%>
                                  <%=Html.Hidden("Id", manualMenuItem.Id, new { id = "Id" })%>
                                  <%=Html.Hidden("Sequence", manualMenuItem.Sequence, new { id = "Sequence" })%>
                                  <%=Html.Hidden("VisibleByInstructor", manualMenuItem.VisibleByInstructor, new { id = "VisibleByInstructor" })%>

                                  <input type="button" name="behavior" value="<%=saveText%>" onclick="if (PxMenuConfig.IsValidData('saveItemManual #Title','saveItemManual #Url', false)) { PxPage.OnFormSubmit('Processing...', true, { form: '#saveItemManual', data: { behavior: 'Save' }, updateSelector: '#content-item', success: <%= OnSuccess %>});}" />
                                    | <a href="" class="menuItemClose">Cancel</a>
                            </li>
                        </ol>
                       
                  <%} %>
                   </div>
              </li>
             </ul>
        </div>

</div>
