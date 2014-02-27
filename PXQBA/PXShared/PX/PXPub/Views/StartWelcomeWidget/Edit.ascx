<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.StartWelcomeWidget>" %>

<style>
    #Contents_tbl, #Contents_ifr
    { 
        width: 650px !important; 
        height: 250px !important; 
    }
    .ui-dialog
    {
        top: 30px !important; 
    }
</style>

<%         
using (Html.BeginForm("Save", "StartWelcomeWidget", FormMethod.Post, new { id = "saveItem" }))
{ %>
<div class="customEditWidget"  dialogWidth="700" dialogHeight="225" dialogMinWidth="400" dialogMinHeight="225" dialogTitle="Welcome Message">
    <div id="form" class="faceplatecopy"> 
        <ol class="formlist">
	        <li>
		        <span>Welcome Page Title:</span><br />
		        <%= Html.TextBoxFor(m => m.Title, new { @class = "InputForControllerAction", style = "width: 99%" })%>
		        <%= Html.ValidationMessageFor(m => m.Title)%>                    
	        </li>				
	        <li>
		        <span>Contents:</span><br />
		        <%: Html.TextAreaFor(m => m.Contents, new { id = "Contents", name = "Contents", @class = "InputForControllerAction html-editor", style = "visibility: hidden; width: 100%;" }) %>
                <%= Html.ValidationMessageFor(m => m.Contents)%>
	        </li>            
        </ol>
    </div>
</div>
<% } %>

<script type="text/javascript">
    (function ($) {
        PxPage.OnReady(function () {
            $(":button").bind('click', function () {
                if (tinyMCE) {
                    tinyMCE.triggerSave();
                }

                if (tinyMCE.activeEditor) {
                    tinyMCE.activeEditor.remove();
                }
            });

            PxPage.Require(['<%= Url.ContentCache("~/Scripts/StartWelcomeWidget/StartWelcome.js") %>'], function () {
                StartWelcomeWidget.Init();
            });
        });

    } (jQuery));    
</script>
