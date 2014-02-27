<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% var isIncludeAddNewOption = ViewData["isIncludeAddNewOption"];%>
<% var contentItemId = ViewData["contentItemId"];%>

<div id="ggbcategorydialog">
    <ul>
        <li>
            <label>Category Name:</label></li>
        <li>
            <input type="text" id="txtaddGBBcategory" value="" /></li>
        <li><span id="spnLinkError" class="error"></span></li>
        <li style="float:right;padding-top:10px;">
            <input type= "button" value="Cancel" class="button small" onclick="ContentWidget.cancelCategoryAdd();"/>
            <input type= "button" value="Add" class="button small" onclick="ContentWidget.addGBBcategory('<%=contentItemId%>', '<%=isIncludeAddNewOption %>');" />
        </li>
    </ul>
</div>
