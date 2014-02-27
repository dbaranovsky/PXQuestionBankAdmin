<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>


<div class="menu">
    <ul id="writing-options" class="link-list">
        <li><a class="rename" href="#">Rename</a></li>
        <li><a class="delete" href="#">Delete</a></li>
        <li><a class="clone" href="#">Clone</a></li>
        <li><a class="add-to-assignment" href="#">Save to Assignment</a></li>
        <li class="last"><a class="add-to-portfolio" href="#">Add to ePortfolio</a></li>
    </ul>
</div>
<div style="clear: both;">
</div>
<div id="documents" class="gridWrapper">
    <table id="documentsGrid" class="scroll jqGrid" cellpadding="0" cellspacing="0">
    </table>
</div>
<div id="comments" class="gridWrapper">
    <table id="commentsGrid" class="scroll jqGrid" cellpadding="0" cellspacing="0">
    </table>
</div>
