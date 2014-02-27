<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<Bfw.PX.PXPub.Models.Template>>" %>

    <div id="templates" class="template-picker">
        <h2 id="fne-title" class="fne-title">

    <% var assignmentContext = TemplateContext.TemplateDisplayContext.Assignments; %>
    <% var templateDisplayContext = (TemplateContext.TemplateDisplayContext)ViewData["TemplateDisplayContext"]; %>

    <%  
        if (templateDisplayContext == assignmentContext)
        { 
    %>
        <span class="existing-content-title px-default-text">
            <input type="radio" name="contentMode" class="add-existing-item" onclick="jQuery.fn.acHelper.addExisting();" /> Assign existing Content
            <input type="radio" name="contentMode" checked="checked"/> Create a new assignment
        </span>
    <%
        } 
    %>
        
    <%  
        if (templateDisplayContext !=assignmentContext) 
        { 
    %>
            <span id="title-text">Create a new assignment</span>
    <%  
        }
    %>

        <span class="nonmodal-actions">
            <a href="#" class="nonmodal-unblock-action" id="nonmodal-unblock-action"></a>
    <%  
        if (templateDisplayContext != assignmentContext) 
        { 
    %>
        <button class="add-existing-item" style="display:none;">Add Existing</button>
    <%
        } 
    %>
            </span>
        </h2>

        <div class="template-list">
            <ul>
    <% 
        foreach (var item in Model)
        {            
            var addedClass = string.Empty;
            if (templateDisplayContext == assignmentContext || templateDisplayContext == TemplateContext.TemplateDisplayContext.FacePlate) 
            {
                addedClass = "create assignmentCenter";
            }
            else
            {
                addedClass = "createcontent contentBrowser";
            }
                  
    %>
            <li id="<%= item.Id%>" itemid="<%= item.Id%>" class="templateLineItem <%=addedClass%>">
            <span class="item-title" itemid="<%= item.Id %>"><%= item.Title %></span>
            <input type="hidden" class="item-description" value="<%= item.Description %>" />
            <input type="hidden" class="item-policies" value="<%= String.Join("|", item.Policies.Map(p => p.Replace("|", "/"))) %>" />
    <%
            if (templateDisplayContext == assignmentContext)                      
            { 
    %>
            <span class="tocAssign workspace" style="display:none;float:right"><a href="#" onclick="return false;" class="addToIW" >Add to Workspace</a></span>
    <%
            } 
    %>
            </li>
    <% 
        } 
    %>
        </ul>
    </div>

    <div class="details">
        <h2 class="title"></h2>
        <span class="description"></span>
        <h3 class="policies-header">Policies</h3>
        <ul class="policies"></ul>
    </div>
</div>
<div class="buttonHolder">
    <input class="create-done-button savebtn" type="button" value="Create New" />   
</div>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.Require(['<%= Url.ContentCache("~/Scripts/Template/TemplatePicker.js") %>'], function () {
                TemplatePicker.Init();
            });
        });

    } (jQuery));    
</script>
