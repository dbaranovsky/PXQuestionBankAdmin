<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Question>" %>
<%
    var response = ViewData["response"] as string;
    var title = ViewData["metatitle"] as string;
     %>
<div class="customquestion-properties-section">
        <a href="#" class="customquestion-properties">Properties</a>
</div>
<input id="txtMetaTitle_customComponent" value="<%=title %>" type="text" style="display:none" />
<div id="customquestion-meta-title-dialog">
    <div class="custom-question-wrap">
        <div class="x-form-item " tabindex="-1">
            <label for="qmeta_title_customeditorcomponent" style="width:49px;" class="x-form-item-label">title:</label>
            
            <input type="text" value="<%=title %>" size="20" autocomplete="off" id="qmeta_title_customeditorcomponent" name="qmeta-title" style="width: 409px;">
            
        </div>
    </div>
</div>
<div class="custom-question-component">
<%=response %>
</div>