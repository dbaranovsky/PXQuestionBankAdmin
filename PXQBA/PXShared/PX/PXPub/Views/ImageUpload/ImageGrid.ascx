<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<div id="ImageGrids">
    <div id="emptyMessage" style="display:none;"><span>No images found.</span></div>
    <div id="loadingMessage" style="margin: 10px 10px 10px 0;font-weight:bold;" class="px-default-text">
    <span>Please wait: Loading your images...</span></div>
    <div id="documents" class="gridWrapper">            
        <table id="documentsGrid" class="scroll jqGrid"></table>

            <ul id="document-grid-options" class="grid-menu" style="display:none;">                        
                <li><a class="delete" href="#">Delete</a></li>                        
            </ul>
        <br /><br />
    </div>
</div>

<script type="text/javascript">
    jQuery(document).ready(function() {
        PxImageUpload.BindImageGrid();
    });
</script>