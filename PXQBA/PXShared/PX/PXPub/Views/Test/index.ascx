<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DocumentToView>" %>
<link type="text/css" rel="stylesheet" href="<%= Url.ContentCache("~/Content/widgets/Highlights.css") %>" />
<style>
    #content-item
    {
        display: inline;
        width: 600px;
        height: 600px;
    }
    .document-viewer-frame-host,
    #document-body-iframe
    {
        display: inline;
        height: 500px;
        width: 700px;
    }
    #highlight-container
    {
        display: inline;
        height: 500px;
        width: 300px;
        background-color: Gray;
        float: right;
    }
    
    .highlight-block .shared {
	background-image: url("style/images/hockenbury5e/ui-icons_course.png");
    }

    #highlight-widget-menu #highlight-widget-delete{
    background-image: url("../images/ui-icons_888888_256x240.png");
    }
    
    .highlight-block .close {background-image: url("style/images/ui-icons_888888_256x240.png");background-position:-32px -192px;height:16px;width:16px;float:right;}
    .highlight-block .share {background-image: url("style/images/ui-icons_888888_256x240.png");background-position:-128px -96px;height:16px;width:16px;float:right;}
    .highlight-block .shared {background-image: url("style/images/ui-icons_assigned_256x240.png");background-position:-128px -96px;height:16px;width:16px;float:right;}
    .highlight-block .delete {background-image: url("style/images/ui-icons_888888_256x240.png");background-position: -176px -96px;width:16px; height:16px;float:right;}
    .highlight-block .lock {background-image: url("style/images/ui-icons_888888_256x240.png");background-position:-192px -96px;height:16px;width:16px;float:right;}
    .highlight-block .unlock {background-image: url("style/images/ui-icons_888888_256x240.png");background-position:-208px -96px;height:16px;width:16px;float:right;}

    .highlight-block .commentLink {background-image: url("style/images/ui-icons_888888_256x240.png");background-position:-240px -112px;width:16px;height:16px;position:relative;display:inline;margin-left:4px;}
    .highlight-block .commentLibrary {background:url("style/images/icon_briefcase.png") no-repeat scroll 0 transparent;left:220px;top:20px;width:17px;height:16px;position:inherit;margin-top: 1px;}
    .highlight-block .comment-submit {background-image: url("style/images/ui-icons_888888_256x240.png");background-position:-96px -112px;height:16px;width:16px;margin-left:4px;float:right;}
</style>

<script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/jquery/jquery-1.10.2.js") %>"></script>
<script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/jquery/jquery.blockUI.js") %>"></script>

<script type="text/javascript" language="javascript">
    var PxPage = function ($) {
        return {
            Context: {
                IsAuthenticated: 'true',
                IsProductCourse: 'false',
                IsInstructor: 'true'
            },
            CssRoutes: {
                highlights_css: '<%= Url.ContentCache("~/Content/Widgets/Highlights.css") %>'
            },
            Routes: {
                save_highlight: '<%= Url.ActionCache("SaveHighlight", "Highlight") %>',
                save_highlight_color: '<%= Url.ActionCache("SaveHighlightColor", "Highlight") %>',
                save_highlight_comment: '<%= Url.ActionCache("SaveComment", "Highlight") %>',
                note_settings: '<%= Url.ActionCache("Index", "NoteSetting") %>',
                update_note_settings: '<%= Url.ActionCache("UpdateNoteSettings", "Highlight") %>',
				get_hightlight_menu:'<%= Url.ActionCache("GetMenuData", "Highlight") %>',
				delete_highlight: '<%= Url.ActionCache("Delete", "Highlight") %>',				
                clear_highlights: '<%= Url.ActionCache("DeleteMyHighlights", "Highlight") %>',
				clear_notes: '<%= Url.ActionCache("DeleteMyNotes", "Highlight") %>',
				load_note_settings: '<%= Url.ActionCache("NoteSettings", "Highlight") %>',
				document_viewer: '<%= Url.ActionCache("DocumentViewer", "Highlight") %>',
                delete_note: '<%= Url.ActionCache("DeleteNote", "Highlight") %>',			
                share_single_note: '<%= Url.ActionCache("ToggleShareNote", "Highlight") %>',
                share_highlight: '<%= Url.ActionCache("ToggleShare", "Highlight") %>',
				create_highlight: '<%= Url.ActionCache("Create", "Highlight") %>',
                update_note: '<%= Url.ActionCache("UpdateNote", "Highlight") %>',
                add_comment: '<%= Url.ActionCache("AddComment", "Highlight") %>',
                togglelock_notes: '<%= Url.ActionCache("ToggleLock", "Highlight") %>',
                create_note: '<%= Url.ActionCache("CreateNote", "NoteLibrary") %>',
                load_comment_library: '<%= Url.ActionCache("CommentLibraryDropList", "Highlight") %>',
                open_note: '<%= Url.ActionCache("Index", "NoteLibrary") %>',
                open_link: '<%= Url.ActionCache("Index", "LinkLibrary") %>',
				load_notes: '<%= Url.ActionCache("HighlightCollection", "Highlight") %>'
            },
            FneLoadedHooks: {},
            FneInitHooks: {},
            FneResizeHooks: {},
            FneCloseHooks: {},
            UpdateFneSize: function () { },
            editableNote_editor_options: {
                // Location of TinyMCE script
                script_url: '<%= Url.ContentCache("~/Content/Widgets/Highlights.css") %>',
                editor_selector: "zen-editor",
                mode: "specific_textareas",
                theme: "advanced",
                skin: "o2k7a",
                skin_variant: "silver",
                plugins: "tabfocus, emotions",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar: false,
                theme_advanced_resizing: false,
                theme_advanced_path: false,   //removes the word PATH from status bar
                // Theme options
                theme_advanced_buttons1: "bold,italic,underline,strikethrough",
                theme_advanced_buttons2: "",
                theme_advanced_buttons3: "",
                setupcontent_callback: "setContentOk"            
            },
            
            Loading: function (controlId, isClass) {
                if ($.blockUI == null) {
                    return;
                }
                if (controlId == undefined || controlId == "" || controlId == null) {
                    controlId = 'main';
                }
                if (controlId.indexOf('#') == 0) {
                    controlId = controlId.slice(1, controlId.length);
                }
                var target = "";
                if (isClass != null && isClass == true) {
                    target = $('.' + controlId);
                }
                else {
                    target = $('#' + controlId);
                }

                $(target).block({
                    message: '<div id="loadingBlock">Loading.....</div>',
                    css: {
                        padding: 0,
                        margin: 0,
                        top: '10%',
                        left: '30%'
                    }
                });

            },

            Loaded: function (controlId, isclass) {
                if (controlId == undefined || controlId == "" || controlId == null) {
                    controlId = 'main';
                }
                if (controlId.indexOf('#') == 0) {
                    controlId = controlId.slice(1, controlId.length);
                }
                var target = "";
                if (isclass != null && isclass == true) {
                    target = $('.' + controlId);
                }
                else {
                    target = $('#' + controlId);
                }

                if ($(target).length > 0) {
                    $(target).unblock();
                }
            },

            OnReady: function (callback) {
                callback();
            },

            log: function (message) {
                try {
                    if (window.console && window.console.log) {
                        window.console.log(message);
                    }
                }
                catch (ex) {
                }
            },
            GetTitleFromContent: function () {
                var titleElement = $('#fne-content h2.content-title:first').clone();
                titleElement.children().remove();
                return $.trim(titleElement.text());
            }
        };

    } (jQuery);
</script>

<script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/Highlight/highlights_2.js") %>"></script>
<script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/Highlight/autoHeight.js") %>"></script>
<script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/tiny_mce/tiny_mce.js") %>"></script>
<script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/tiny_mce/tiny_mce_gzip.js") %>"></script>
<script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/HighlightWidget/highlighter.js") %>"></script>
<script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/HighlightWidget/HighlightWidget.js") %>"></script>
<script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/ActionWidget/ActionWidget.js") %>"></script>
<script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/Other/jquery.tmpl.js") %>"></script>
<script type="text/javascript" src="<%= Url.ContentCache("~/Scripts/jquery/jquery-ui-1.10.3.custom.min.js") %>"></script>
<link type="text/css" rel="stylesheet" href="<%= Url.ContentCache("~/Content/jquery.ui/jquery-ui-1.9.0.custom.css") %>" />

<div id="fne-window" class="fne-window-basic basic" style="height:"965px;">
    <div id="content-item" class="fne-local content documentcollection" style="width: auto;">
        <div id="contentwrapper" class="contentwrapper">
            <div id="content" class="content" style="width: 100%;">
                <div id="document-viewer" class="document-viewer allowComments " style="overflow:visible; height:auto;">
                    <div id="document-body" class="document-body" style="height:auto;display:block;width:99%;">
                        <div id="document-viewer-frame-host-665771493" class="document-viewer-frame-host"> 
                            <iframe id="document-body-iframe" class="autoHeight proxyFrame" onload="OnContentLoaded()" src="<%= Url.ActionCache("iframe", "Test") %>">
                            </iframe>
                        </div>

                        <input type="hidden" id="AllowComments" value="<%=Model.AllowComments%>" />
                        <input type="hidden" id="IsExternalContent" value="<%=Model.IsExernalContent%>" />

                        <div id="highlight-container">
                            <div id="highlight-new-container" class="highlight-new-container" style="display:none">
                                <% Html.RenderAction("NewHighlightForm", "Highlight", Model); %>
                            </div>
                            <div id="highlightList">
                                <%--client call will get notes and highlights and add notes in this div--%>
                            </div>
                            <div style="clear:both;"> </div>
                        </div>
                        <div id="content-loading" class="content-loading" style="display:none;margin-top:10px;height:20px;">
                            <div id="loadingBlock"></div>
                        </div>
                    </div>
                </div>
                <div id="newTest">
                    <div id="testRefElement">
                        line1 <br />
                        line2 <br />
                        line3 <br />
                    </div>
                    <div id="testMyElement">
                        myElement
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<script type="text/javascript" language="javascript">
    function OnContentLoaded() {
        PxComments.Init();
    }
</script>
<script type="text/javascript" language="javascript">
    bfw_itemId = '<%= Model.ItemId  %>';
    bfw_secondaryId = '<%= Model.SecondaryId  %>';
    bfw_reviewId = '<%= Model.PeerReviewId  %>';
    bfw_commenterId = '<%= Model.CommenterId  %>';
    bfw_highlightType = '<%= Model.HighlightType  %>';
    bfw_highlightDesc = "<%= System.Web.HttpUtility.UrlEncode(Model.HighlightDescription)  %>";
    bfw_itemUrl = '<%= System.Web.HttpUtility.HtmlEncode(Model.Url) %>';
    bfw_isAssignmentView = '<%= Model.isAssignmentView  %>';
    bfw_isCurrentUserView = '<%= Model.IsCurrentUserContext %>';
    bfw_enrollmentId = '<%= Model.CommenterId %>';
    bfw_highlightId = '<%= Model.HighlightId %>';

    <%
    if(!Model.IsBinary)
    {
    %>
        (function ($) {
            $.ready(function () {
            //safe to use $, safe to use PxPage

                var iframe  = $("#document-body-iframe");

                // this each loop ensures that  PxComments.Init() is called after the iframe is loaded, else it creates cross domain issue
                iframe.each(function(){
                    if (this.attachEvent){
                        this.attachEvent("onload", function(){
                            PxComments.Init('');
                        });
                    }
                    else {
                        this.onload = function(){
                            PxComments.Init('');                       
                        };
                    }
                });
            });
        } (jQuery))
    <%
    }
    %>

</script>