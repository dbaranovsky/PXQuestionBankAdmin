<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl"  %>
<%@ Import Namespace="StackExchange.Profiling" %>

<%= MiniProfiler.RenderIncludes(RenderPosition.Right, showTimeWithChildren: true, showControls: true) %>
<script type="text/javascript">
    var deps = ['<%= Url.ContentCache("~/Scripts/tiny_mce/tiny_mce_gzip.js", false) %>'];
    PxPage.Require(deps, function() {
        tinyMCE_GZ.init({
            theme: "advanced,zen",
            plugins: "customButton,indentDropdown,textOpsDropdown,subSupDropdown,justifyDropdown,tableDropdown,autolink,lists,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,wordcount,advlist,autosave,spellchecker,example,-testbox",
            languages: 'en',
            disk_cache: false,
            debug: false
        });
    });
</script>
<script type="text/javascript" >
    (function ($) {
        var deps = <%= ResourceEngine.JsonFor("~/Scripts/common.js") %>;
        var depsIncluded = <%= ResourceEngine.JsonForAllFiles("~/Scripts/common.js") %>;
        PxPage.Require(deps, function() {
            $(document).ready(function() {

                PxPage.OnReady(function() {
                    PxPage.SetActiveHeaderMenu('home_menu');
                    $('div.homepageheader').toggleClass("hideHeader", false);
                    PxSearch.Init();
                });
                
                PxPage.Init({
                    //link to configuration resource: http://www.tinymce.com/wiki.php/Configuration
                    launchpad_editor_options: {
                        // Location of TinyMCE script
                        script_url: '<%= Url.ContentCache("~/Scripts/tiny_mce/tiny_mce.js") %>',
                        plugins : "launchpad,autolink,lists,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,justifyDropdown,advimage,hts",
                        
                        mode : "textareas",
                        theme : "advanced",
                        theme_advanced_buttons1 : "bold,mymorestylesbutton,sizebutton,forecolor,|,justifyDropdown,bullist,|,table,link,image,media,filebutton,htsFormulaEditor,|,code",
                        theme_advanced_toolbar_location : "top",
                        theme_advanced_toolbar_align : "left",
                        theme_advanced_statusbar_location : "bottom",
                        theme_advanced_resizing : true,
                        theme_advanced_path : false,
                        convert_urls: false,                       
                        gecko_spellcheck : true,
                        //spellchecker_rpc_url: '{backend}',                    
                        //forced_root_block: '', // removed because it caused inproper url recognition in case of a new line
                        //encoding: "xml",

                        image_upload_path: '<%= Url.ActionCache("Index", "ImageUpload") %>',
                        equation_image_path: '<%= Bfw.PX.PXPub.Components.BusinessContextBase.HTSEquationImageUrl %>',

                        extended_valid_elements: "img[class|src|border=0|alt|title|hspace|vspace|width|height|align|onmouseover|onmouseout|name|iprof|hts-data-id|hts-data-type|hts-data-equation|hts-data-variable-type|hts-data-variable-index|function|datafile]",
                        
                        setup: function(ed) {
                            // word count in bottom of editor
                            ed.onKeyUp.add(function(ed, e) {    
                                var strip = (tinyMCE.activeEditor.getContent( {format : 'raw'} )).replace(/(<([^>]+)>)/ig,"");
                                var text = "";
                                if (strip != "") {
                                    text = "<span style='font-size:10px;'>Word count: " + strip.split(' ').length + "</span>";
                                }
                                tinymce.DOM.setHTML(tinymce.DOM.get(tinyMCE.activeEditor.id + '_path_row'), text);    
                            });
                        },
                 
                        onchange_callback : "setEditorValueChanged",
                        init_instance_callback : "editorInitialized",

                        width: "500",
                        height: "100"
                    },

                    xbook_editor_options: {
                        // Location of TinyMCE script
                        script_url: '<%= Url.ContentCache("~/Scripts/tiny_mce/tiny_mce.js") %>',
                        plugins : "launchpad,autolink,lists,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,justifyDropdown,advimage,hts",
                        
                        mode : "textareas",
                        theme : "advanced",
                        theme_advanced_buttons1 : "bold,mymorestylesbutton,sizebutton,forecolor,|,justifyDropdown,bullist,|,table,link,image,media,filebutton,htsFormulaEditor,|,code",
                        theme_advanced_toolbar_location : "top",
                        theme_advanced_toolbar_align : "left",
                        theme_advanced_statusbar_location : "bottom",
                        theme_advanced_resizing : true,
                        theme_advanced_path : false,
                        convert_urls: false,                       
                        gecko_spellcheck : true,

                        image_upload_path: '<%= Url.ActionCache("Index", "ImageUpload") %>',
                        equation_image_path: '<%= Bfw.PX.PXPub.Components.BusinessContextBase.HTSEquationImageUrl %>',

                        extended_valid_elements: "img[class|src|border=0|alt|title|hspace|vspace|width|height|align|onmouseover|onmouseout|name|iprof|hts-data-id|hts-data-type|hts-data-equation|hts-data-variable-type|hts-data-variable-index|function|datafile]",
                        
                        setup: function(ed) {
                            // word count in bottom of editor
                            ed.onKeyUp.add(function(ed, e) {    
                                var strip = (tinyMCE.activeEditor.getContent( {format : 'raw'} )).replace(/(<([^>]+)>)/ig,"");
                                var text = "";
                                if (strip != "") {
                                    text = "<span style='font-size:10px;'>Word count: " + strip.split(' ').length + "</span>";
                                }
                                tinymce.DOM.setHTML(tinymce.DOM.get(tinyMCE.activeEditor.id + '_path_row'), text);    
                            });
                        },
                 
                        onchange_callback : "setEditorValueChanged",
                        init_instance_callback : "editorInitialized",

                        width: "500",
                        height: "100"
                    },
                    
                    html_editor_options: {
                        // Location of TinyMCE script
                        script_url: '<%= Url.ContentCache("~/Scripts/tiny_mce/tiny_mce.js") %>',
                        mode: "specific_textareas",
                        editor_selector: "html-editor",
                        // General options
                        theme: "advanced",
                        plugins: "customButton,indentDropdown,textOpsDropdown,subSupDropdown,justifyDropdown,tableDropdown,example,autolink,lists,pagebreak,style,layer,table,save,advhr,advimage,netImageBrowser,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,pxWordCount,advlist,autosave,spellchecker,-testbox,hts",
						
                        // Theme options
                        theme_advanced_buttons1: "bold,italic,underline,strikethrough,subSupDropdown,forecolor,backcolor,|,fontselect,fontsizeselect,|,styleselect,justifyDropdown,bullist,indentDropdown,|,formatselect", //outdent,indent,
                        theme_advanced_buttons2: "textOpsDropdown,|,search,replace,|,undo,redo,|,spellchecker,ispell,tableDropdown,|,customButton,netImageBrowser,media,charmap,hr,pagebreak,|,quickLinkList,|,code,image",   //link
                        theme_advanced_buttons3: "",
						
                        extended_valid_elements: "img[class|src|border=0|alt|title|hspace|vspace|width|height|align|onmouseover|onmouseout|name|iprof|hts-data-id|hts-data-type|hts-data-equation|hts-data-variable-type|hts-data-variable-index|function|datafile]",
                        //force_br_newlines : false,
                        //force_p_newlines : true,
                        convert_urls: false,                        
                        //relative_urls: false,
                        //remove_script_host: false,
                        theme_advanced_toolbar_location: "top",
                        theme_advanced_toolbar_align: "left",
                        theme_advanced_statusbar_location: "top",
                        theme_advanced_resizing: false,
                        theme_advanced_path: false,   //removes the word PATH from status bar						
                        //fix_list_elements : true,
                        forced_root_block: '',                        
                        //apply_source_formatting : true,
                        //constrain_menus : true,
                        encoding: "xml",

                        image_upload_path: '<%= Url.ActionCache("Index", "ImageUpload") %>',
                        
                        //start debugging in FF
                        //handle_node_change_callback : "nodeChangeHandler",
                        onchange_callback : "setEditorValueChanged",
                        init_instance_callback : "editorInitialized",					

                        // Style formats
                        style_formats: [
                            { title: 'Bold text', inline: 'b' },
                            { title: 'Red text', inline: 'span', styles: { color: '#ff0000' } },
                            { title: 'Red header', block: 'h1', styles: { color: '#ff0000' } },
                            { title: 'Example 1', inline: 'span', classes: 'example1' },
                            { title: 'Example 2', inline: 'span', classes: 'example2' },
                            { title: 'Table styles' },
                            { title: 'Table row 1', selector: 'tr', classes: 'tablerow1' }			
			
                        ],

                        setup: function(ed) { 

                        }	//end SETUP

                    },	//end init


                    editableNote_editor_options: {
                        // Location of TinyMCE script
                        script_url: '<%= Url.ContentCache("~/Scripts/tiny_mce/tiny_mce.js") %>',
                        editor_selector: "zen-editor",
                        mode: "specific_textareas",
                        theme: "advanced",
                        skin : "o2k7a",
                        skin_variant : "silver",
                        plugins: "tabfocus, emotions",
                        theme_advanced_toolbar_location : "top",
                        theme_advanced_toolbar_align : "left",
                        theme_advanced_statusbar : false,
                        theme_advanced_resizing : false,
                        theme_advanced_path : false,   //removes the word PATH from status bar
                        // Theme options
                        theme_advanced_buttons1 : "bold,italic,underline,strikethrough",
                        theme_advanced_buttons2 : "",
                        theme_advanced_buttons3 : "",
                        setupcontent_callback: "setContentOk"
                    },

                    rubricText_editor_options: {
                        // Location of TinyMCE script
                        script_url: '<%= Url.ContentCache("~/Scripts/tiny_mce/tiny_mce.js") %>',
                        editor_selector: "rubricText-editor",
                        mode: "specific_textareas",
                        theme: "advanced",
                        skin : "o2k7a",                        
                        skin_variant : "silver",
                        plugins: "tabfocus, emotions",
                        theme_advanced_toolbar_location : "top",
                        theme_advanced_toolbar_align : "left",
                        theme_advanced_statusbar : false,
                        theme_advanced_resizing : false,
                        theme_advanced_path : false,   //removes the word PATH from status bar
                        // Theme options
                        theme_advanced_buttons1 : "bold,italic,underline,strikethrough",
                        theme_advanced_buttons2 : "",
                        theme_advanced_buttons3 : "",
                        setupcontent_callback: "setContentOk"
                    },

                    css_routes: {
                        highlights_css: '<%= Url.ContentCache("~/Content/Widgets/Highlights.css") %>',
                        treeThemeUrl: '<%= Url.ContentCache("~/Scripts/jstree/themes/default/style.css") %>',
                        gradeReports_css: '<%= Url.Content("~/Content/Print/Rubric_LO_Report_Print.css") %>',
                        rubric_css: '<%= Url.Content("~/Content/widgets/Rubric.css") %>'
                    },

                    routes: {
                        common: '<%= Url.ContentCache("~/Scripts/Common/") %>',
                        show_course_activation: '<%=Url.ActionCache("ShowCreateNameCourse", "Course") %>',
                        assessment_view: '<%=Url.ActionCache("Index", "AssessmentSettings") %>',
                        change_assessment_type: '<%=Url.ActionCache("ChangeType", "AssessmentSettings") %>',
                        show_tocitem_tooltip: '<%=Url.ActionCache("GetTocToolTip", "Assignment")%>',
                        show_submission_status: '<%=Url.ActionCache("GetSubmissionStatus", "Assignment")%>',
                        assignment_delete_docs: '<%=Url.ActionCache("DeleteDocs", "Assignment")%>',

                        render_trail: '<%= Url.ActionCache("BreadcrumbTrail", "Breadcrumb") %>',

                        template_picker: '<%= Url.ActionCache("TemplatePicker", "Template") %>',
                        template_management: '<%= Url.ActionCache("TemplateManagement", "Template") %>',
                        template_management_details: '<%= Url.ActionCache("TemplateManagementDetails", "Template") %>',
                        create_template: '<%= Url.ActionCache("CreateTemplate", "Template") %>',
                        save_templateas: '<%= Url.ActionCache("SaveTemplateAs", "Template") %>',
                        save_template: '<%= Url.ActionCache("SaveTemplate", "Template") %>',
                        save_template_update: '<%= Url.ActionCache("SaveTemplateUpdate", "Template") %>',
                        delete_template: '<%= Url.ActionCache("DeleteTemplate", "Template") %>',
                        get_template_items: '<%= Url.ActionCache("GetTemplateItems", "Template") %>',
                        get_Template_info: '<%= Url.ActionCache("GetTemplateInfo", "Template") %>',
						
                        get_mce_contenteditor: '<%= Url.ActionCache("MceContentEditor", "PageAction") %>',
                        get_mce_ebookeditor: '<%= Url.ActionCache("MceEbookEditor", "PageAction") %>',
                        get_mce_coursematerials: '<%= Url.ActionCache("MceCourseMaterial", "PageAction") %>',
                        studentView: '<%=  Url.Action("SwitchView","AccountWidget",new { To = AccountWidget.StudentViewStates.StudentView }) %>',
                        instructorView: '<%=  Url.Action("SwitchView","AccountWidget",new { To = AccountWidget.StudentViewStates.InstructorView }) %>',
                        create_content: '<%= Url.ActionCache("CreateContent", "ContentWidget") %>',
                        reorder_content: '<%= Url.ActionCache("ReorderContent", "ContentWidget") %>',
                        delete_confirmation_dialog: '<%= Url.ActionCache("DeleteConfirmDialog", "ContentWidget") %>',
                        display_content: '<%= Url.Action("DisplayItem", "ContentWidget", new { id = ""} ) %>',
                        addGradebookCategory_link :'<%= Url.Action("AddGradebookCategory", "ContentWidget", new { id=""} ) %>',
                        addGradebookCategoryToUnit :'<%= Url.Action("AddGradebookCategoryToUnit", "ContentWidget") %>',
                        edit_content: '<%= Url.ActionCache("DisplayItem", "ContentWidget") %>',
                        get_content_info: '<%= Url.ActionCache("GetContentInfo", "ContentWidget") %>',
                        save_ac_category_date: '<%= Url.ActionCache("AssignAssignmentCenterCategory", "ContentWidget") %>',
                        save_assignmentcenter_date: '<%= Url.ActionCache("AssignAssignmentCenterItemDate", "ContentWidget") %>',
                        save_assignment_date: '<%= Url.ActionCache("AssignItemDate", "ContentWidget") %>',
                        unassign_item_date: '<%= Url.ActionCache("UnAssignItemDate", "ContentWidget") %>',
                        assigned_items_date: '<%= Url.ActionCache("ListAssignmentsByDate", "ContentWidget") %>',
                        show_createandassign: '<%= Url.ActionCache("CreateAndAssign", "ContentWidget") %>',
                        delete_item: '<%= Url.ActionCache("DeleteItem", "ContentWidget") %>',
                        content_read: '<%= Url.ActionCache("MarkContentAsRead", "ContentWidget") %>',
                        set_content_duration: '<%= Url.ActionCache("StoreContentDuration", "ContentWidget") %>',
                        expand_to_item: '<%= Url.ActionCache("ExpandToItem", "ContentWidget") %>',
                        save_resource_to_toc: '<%= Url.ActionCache("SaveResourceToTOC", "ContentWidget") %>',
                        assign_item: '<%= Url.ActionCache("AssignItem", "ContentWidget") %>',
                        get_contentviewer: '<%= Url.ActionCache("GetDocumentViewer", "ContentAreaWidget") %>',
                        get_contenttype: '<%= Url.ActionCache("GetContentType", "ContentAreaWidget") %>',
                        get_contentarea_viewoptions: '<%= Url.ActionCache("ContentAreaViewOptions", "ContentAreaWidget") %>',
                        get_relatedcontent: '<%= Url.ActionCache("GetRelatedContents", "ContentAreaWidget") %>',
                        get_xbook_viewoptions: '<%= Url.ActionCache("XbookContentOptions", "ContentAreaWidget") %>',
                        widget_create: '<%= Url.ActionCache("ShowWidgetConfig", "CustomWidget") %>',
                        refresh_default_item_description: '<%= Url.ActionCache("RefreshDefaultItemDescription", "ContentWidget") %>',
                        item_has_grade: '<%= Url.ActionCache("GradeExistForItem", "Grade") %>',
                        assignment_item_count: '<%= Url.ActionCache("AssignmentItemCount", "Assignment") %>',
                        group_tab_list: '<%= Url.ActionCache("GetSettingsTabList", "Groups") %>',

                        group_list: '<%= Url.ActionCache("GroupList", "Groups") %>',
                        group_management: '<%= Url.ActionCache("ManageGroupsFne", "Groups") %>',
                        greadbook_scorecard: '<%= Url.ActionCache("GradebookScorecardFne", "Groups") %>',
                        create_group: '<%= Url.ActionCache("CreateGroup", "Groups") %>',
                        create_highlight: '<%= Url.ActionCache("Create", "Highlight") %>',
                        delete_highlight: '<%= Url.ActionCache("Delete", "Highlight") %>',
                        get_mediaVaultUrl: '<%= Url.ActionCache("GetMediaVaultUrlAction", "Highlight") %>',
                        share_highlight: '<%= Url.ActionCache("ToggleShare", "Highlight") %>',
                        save_highlight_comment: '<%= Url.ActionCache("SaveComment", "Highlight") %>',
                        load_comment_library: '<%= Url.ActionCache("CommentLibraryDropList", "Highlight") %>',
                        save_highlight_color: '<%= Url.ActionCache("SaveHighlightColor", "Highlight") %>',
                        get_poolquestionlist: '<%= Url.ActionCache("QuestionList", "Quiz") %>',
                        get_mainpoollist: '<%= Url.ActionCache("DisplayQuestionList", "Quiz") %>',
                        add_comment: '<%= Url.ActionCache("AddComment", "Highlight") %>',
                        Highlight_AddCommentToTopNote: '<%= Url.ActionCache("AddCommentToTopNote", "Highlight") %>',
                        update_note: '<%= Url.ActionCache("UpdateNote", "Highlight") %>',
                        clear_highlights: '<%= Url.ActionCache("DeleteMyHighlights", "Highlight") %>',
                        togglelock_highlight_notes: '<%= Url.ActionCache("ToggleLock", "Highlight") %>',
                        Hightlight_ToggleLockTopNote: '<%= Url.ActionCache("ToggleLockTopNote", "Highlight") %>',
                        clear_notes: '<%= Url.ActionCache("DeleteMyNotes", "Highlight") %>',
                        load_note_settings: '<%= Url.ActionCache("NoteSettings", "Highlight") %>',
                        load_notes: '<%= Url.ActionCache("HighlightCollection", "Highlight") %>',
                        get_hightlight_menu:'<%= Url.ActionCache("GetMenuData", "Highlight") %>',
                        load_highlights: '<%= Url.ActionCache("HighlightDetails", "Highlight") %>',
                        update_note_settings: '<%= Url.ActionCache("UpdateNoteSettings", "Highlight") %>',
                        delete_note: '<%= Url.ActionCache("DeleteNote", "Highlight") %>',
                        delete_notes: '<%= Url.ActionCache("DeleteNotes", "Highlight") %>',
                        document_viewer: '<%= Url.ActionCache("DocumentViewer", "Highlight") %>',
                        share_single_note: '<%= Url.ActionCache("ToggleShareNote", "Highlight") %>',
                        save_highlight: '<%= Url.ActionCache("SaveHighlight", "Highlight") %> ',

                        navigation_structure: '<%= Url.ActionCache("GetTocStructure", "Navigation") %>',

                        save_note: '<%= Url.ActionCache("SaveNote", "NoteLibrary") %>',
                        create_note: '<%= Url.ActionCache("CreateNote", "NoteLibrary") %>',                        
                        open_note: '<%= Url.ActionCache("Index", "NoteLibrary") %>',
                        delete_note_notelibrary: '<%= Url.ActionCache("DeleteNote", "NoteLibrary") %>',
                        reorder_notes: '<%=Url.ActionCache("ReorderNotes", "NoteLibrary") %>',

                        get_searchInfo: '<%= Url.ActionCache("GetStudents", "NoteSetting") %>',
                        student_exist: '<%= Url.ActionCache("StudentsExist", "NoteSetting") %>',
                        share_notes: '<%= Url.ActionCache("ShareNotes", "NoteSetting") %>',
                        stop_sharing: '<%= Url.ActionCache("StopSharing", "NoteSetting") %>',
                        note_settings: '<%= Url.ActionCache("Index", "NoteSetting") %>',

                        open_link: '<%= Url.ActionCache("Index", "LinkLibrary") %>',
                        search_links: '<%= Url.ActionCache("SearchLink", "LinkLibrary") %>',

                        sc_display_commenting: '<%= Url.ActionCache("DisplayCommenting", "SocialCommentingWidget") %>',
                        sc_update_item: '<%= Url.ActionCache("ChangeCommentingFlagOnItem", "SocialCommentingWidget") %>',

                        import_dialog: '<%= Url.ActionCache("ImporterDialog", "QuestionImporter") %>',
                        validate_respondus_questions: '<%= Url.ActionCache("ValidateRespondusQuestions", "QuestionImporter") %>',
                        import_respondus_questions: '<%= Url.ActionCache("ImportRespondusQuestions", "QuestionImporter") %>',

                        edit_quiz: '<%= Url.ActionCache("EditQuiz", "Quiz") %>',
                        show_quiz: '<%= Url.ActionCache("ShowQuiz", "Quiz") %>',
                        store_live_quiz: '<%= Url.ActionCache("StoreQuizData", "Quiz") %>',
                        make_hts_question: '<%= Url.ActionCache("MakeHtsQuestion", "Quiz") %>',
                        question_order: '<%= Url.ActionCache("SaveQuestionList", "Quiz") %>',
                        append_to_question_list: '<%= Url.ActionCache("AddQuestionsToQuiz", "Quiz") %>',
                        edit_question: '<%= Url.ActionCache("EditQuestion", "Quiz") %>',
                        create_question: '<%= Url.ActionCache("CreateQuestion", "Quiz") %>',
                        question_list: '<%= Url.ActionCache("Questions", "Quiz") %>',
                        single_question: '<%= Url.ActionCache("GetSingleQuestion", "Quiz") %>',
                        validate_question_list: '<%= Url.ActionCache("ValidateQuestions", "Quiz") %>',
                        update_question_bank_totals: '<%= Url.ActionCache("UpdateQuestionBankTotals", "Quiz") %>',
                        search_question: '<%= Url.ActionCache("SearchQuestions", "QuizSearch") %>',						
                        hts_player: '<%= Url.RouteUrl("PxHTSPlayer") %>',
                        custom_inline_preview: '<%= Url.ActionCache("GetCustomInlinePreview", "Quiz") %>',
                        save_points: '<%= Url.ActionCache("SavePoints", "Quiz") %>',
                        add_new_pool: '<%= Url.ActionCache("AddNewPool", "Quiz") %>',
                        validate_pool_title: '<%= Url.ActionCache("DoesQuestionPoolExist", "Quiz") %>',
                        edit_pool: '<%= Url.ActionCache("EditPool", "Quiz") %>',
                        update_questions_meta: '<%= Url.ActionCache("UpdateQuestionsMetaData", "Quiz") %>',
                        update_previous_question: '<%= Url.ActionCache("UpdatePreviousQuestion", "Quiz") %>',
                        remove_question_from_cache: '<%= Url.ActionCache("RemoveQuestionFromCache", "Quiz") %>',
                        delete_questions: '<%= Url.ActionCache("DeleteQuestions", "Quiz") %>',
                        get_related_content: '<%= Url.ActionCache("GetRelatedContent", "Quiz") %>',
                        remove_related_content: '<%= Url.ActionCache("RemoveRelatedContent", "Quiz") %>',
                        add_related_content: '<%= Url.ActionCache("AddRelatedContent", "Quiz") %>',
                        save_related_content:'<%= Url.ActionCache("SaveRelatedContent", "Quiz") %>',
                        get_lc_question_preview_url:'<%= Url.ActionCache("GetLearningCurveIndividualQuestionPreviewUrl", "Quiz") %>',
                        make_quiz_gradable:'<%= Url.ActionCache("MakeQuizGradable", "Quiz") %>',
                        make_quiz_gradable_if_submitted:'<%= Url.ActionCache("MakeQuizGradableIfSubmitted", "Quiz") %>',
                        hts_svc: '<%= ConfigurationManager.AppSettings["PxHTSEditorUrl"] %>',

                        render_children: '<%= Url.ActionCache("ExpandSection", "QuizEdit") %>',
                        render_base_page: '<%= Url.ActionCache("BrowseMainPage", "QuizEdit") %>',

                        search_advanced: '<%= Url.ActionCache("AdvancedSearch", "Search") %>',
                        search_results: '<%= Url.ActionCache("Results", "Search") %>',
                        search_results_advanced: '<%= Url.ActionCache("AdvancedResults", "Search") %>',
                        search_category_toggle: '<%= Url.ActionCache("ToggleSearchCategory", "Search") %>',
                        search_faceted_results: '<%= Url.ActionCache("FacetedSearchResults","Search") %>',

                        get_templates: '<%= Url.ActionCache("ItemTemplates", "Template") %>',
                        template_edits: '<%= Url.ActionCache("ApplyTemplateChanges", "Template") %>',
                        get_related_templates: '<%= Url.ActionCache("GetRelatedTemplates", "Template") %>',
                        item_from_template: '<%= Url.ActionCache("ItemFromTemplate", "Template") %>',
                        copy_item_settings: '<%= Url.ActionCache("CopyItemSettings", "Template") %>',
                        course_materials_compose_template: '<%= Url.ActionCache("ComposeMaterials", "CourseMaterials") %>',
                        course_materials_edit_template: '<%= Url.ActionCache("EditMaterials", "CourseMaterials") %>',
                        course_materials_update_resourcelist: '<%= Url.ActionCache("UpdateCourseMaterialResourceList", "CourseMaterials") %>',
                        course_materials_download_resource: '<%= Url.ActionCache("DownloadCourseMaterialResource", "CourseMaterials") %>',
                        course_materials_delete_resource: '<%= Url.ActionCache("DeleteCourseMaterialsResource", "CourseMaterials") %>',
                        settings_view_save_settings: '<%= Url.ActionCache("SaveSettings", "SettingsView") %>',

                        bookmarkmenuitems: '<%= Url.ActionCache("BookmarkMenuItems", "Bookmark") %>',
                        list_children_ids: '<%= Url.ActionCache("ListChildrenIds", "ContentWidget") %>',
                        expand_section: '<%= Url.ActionCache("ExpandSection", "ContentWidget") %>',
                        show_contentpicker: '<%= Url.ActionCache("ContentPicker", "PageAction") %>',
                        show_contentpicker_expand: '<%= Url.ActionCache("ExpandContentPicker", "PageAction") %>',
                        save_attempts: '<%= Url.ActionCache("SaveAttempts", "Quiz") %>',
                        
                        save_timelimits: '<%= Url.ActionCache("SaveTimeLimits", "Quiz") %>',
                        save_score: '<%= Url.ActionCache("SaveScore", "Quiz") %>',
                        save_scrambled: '<%= Url.ActionCache("SaveScrambled", "Quiz") %>',
                        save_hints: '<%= Url.ActionCache("SaveHints", "Quiz") %>',
                        save_review: '<%= Url.ActionCache("SaveReview", "Quiz") %>',
                        group_dropdown: '<%= ConfigurationManager.AppSettings["GroupDropDown"] %>',
                        show_assignment_widget: '<%= Url.ActionCache("Summary", "AssignmentWidget") %>',
                        view_assignment_widget: '<%= Url.ActionCache("View", "AssignmentWidget") %>',
                        search_assignments: '<%= Url.ActionCache("SearchAssignments", "AssignmentWidget") %>',
                        save_important_assignments: '<%= Url.ActionCache("SaveImportantAssignments", "AssignmentWidget") %>',
                        create_assignment: '<%= Url.ActionCache("CreateAssignment", "AssignmentWidget")%>',
                        set_assignment_folder: '<%= Url.ActionCache("SetAssignmentFolder", "AssignmentWidget")%>',
                        save_rubric_data: '<%= Url.ActionCache("SaveRubricData", "Rubric") %>',                        select_rubric: '<%= Url.ActionCache("SelectRubric", "Rubric") %>',
                        edit_rubric: '<%= Url.ActionCache("EditRubric", "Rubric") %>',
                        delete_rubric: '<%= Url.ActionCache("DeleteRubric", "Rubric") %>',
                        edit_rubric_alignment: '<%= Url.ActionCache("EditRubricAlignment", "Rubric") %>',
                        
                        rubric_alignments: '<%= Url.ActionCache("RubricAlignments", "Rubric") %>',
                        rubric_alignments_by_assignment: '<%= Url.ActionCache("AlignmentsByAssignment", "Rubric") %>',
                        rubric_alignments_by_rubric: '<%= Url.ActionCache("AlignmentsByRubric", "Rubric") %>',

                        copy_rubric: '<%= Url.ActionCache("CopyRubric", "Rubric") %>',
                        align_rubric: '<%= Url.ActionCache("AlignRubric", "Rubric") %>',
                        preview_rubric: '<%= Url.ActionCache("PreviewRubric", "Rubric") %>',
                        rubric_objectives: '<%= Url.ActionCache("LearningObjectives", "Rubric") %>',
                        add_course_rubrics: '<%= Url.ActionCache("AddCourseRubrics", "Assessment") %>',
                        remove_course_rubrics: '<%= Url.ActionCache("RemoveCourseRubrics", "Assessment") %>',
                        assessment_display_rubrics: '<%= Url.ActionCache("ManageRubrics", "Assessment") %>',
                        assessment_display_LOB: '<%= Url.ActionCache("ManageObjectives", "Assessment") %>',
                        assessment_display_reports: '<%= Url.ActionCache("ManageReports", "Assessment") %>',

                        rpt_main_view: '<%=Url.ActionCache("Report", "Report") %>',
                        rpt_detail_view:'<%=Url.ActionCache("GetDetails", "Report") %>',
                        rpt_performance_view:'<%=Url.ActionCache("GetRubricHighlights", "Report") %>',
                        rpt_performance_view2:'<%=Url.ActionCache("GetLearningObjHighlights", "Report") %>',
                        add_new_objective: '<%= Url.ActionCache("Add", "LearningObjective") %>',
                        update_objectives: '<%= Url.ActionCache("Update", "LearningObjective") %>',
                        delete_objectives: '<%= Url.ActionCache("Delete", "LearningObjective") %>',
                        rpt_main_view: '<%=Url.ActionCache("Report", "Report") %>',
                        rpt_detail_view:'<%=Url.ActionCache("GetDetails", "Report") %>',
                        rpt_performance_view:'<%=Url.ActionCache("GetRubricHighlights", "Report") %>',
                        rpt_performance_view2:'<%=Url.ActionCache("GetLearningObjHighlights", "Report") %>',
                        index_objectives: '<%= Url.ActionCache("Index", "LearningObjective") %>',
                        list_objectives: '<%= Url.ActionCache("List", "LearningObjective") %>',
                        addWidget: '<%= Url.ActionCache("AddWidget", "PageAction") %>',
                        editWidget: '<%= Url.ActionCache("EditWidget", "PageAction") %>',
                        getWidgetTemplate: '<%= Url.ActionCache("GetWidgetTemplate", "PageAction") %>',
                        moveWidget: '<%= Url.ActionCache("MoveWidget", "PageAction") %>',
                        removeWidget: '<%= Url.ActionCache("RemoveWidget", "PageAction") %>',
                        setWidgetDisplay: '<%= Url.ActionCache("SetWidgetDisplay", "PageAction") %>',
                        addAnnouncement: '<%= Url.ActionCache("AddAnnoucement","AnnouncementWidget") %>',
                        reloadAnnouncement: '<%= Url.ActionCache("ViewSummary", "AnnouncementWidget") %>',
                        AnnouncementsWidget: '<%= Url.ActionCache("Summary", "AnnouncementWidget") %>',
                        viewAllAnnoucements: '<%= Url.ActionCache("ViewAll", "AnnouncementWidget") %>',
                        removeAnnouncement: '<%= Url.ActionCache("RemoveAnnouncement", "AnnouncementWidget") %>',
                        editAnnouncement: '<%= Url.ActionCache("EditAnnouncement", "AnnouncementWidget") %>',
                        moveAnnouncement: '<%= Url.ActionCache("MoveAnnouncement", "AnnouncementWidget") %>',
                        pinAnnouncement: '<%= Url.ActionCache("PinAnnouncement", "AnnouncementWidget") %>',
                        unpinAnnouncement: '<%= Url.ActionCache("UnPinAnnouncement", "AnnouncementWidget") %>',
                        archiveAnnouncement: '<%= Url.ActionCache("ArchiveAnnouncement", "AnnouncementWidget") %>',
                        addMenuItem: '<%= Url.ActionCache("AddMenuItem", "PageAction") %>',
                        moveMenuItem: '<%= Url.ActionCache("MoveMenuItem", "PageAction") %>',
                        removeMenuItem: '<%= Url.ActionCache("RemoveMenuItem", "PageAction") %>',
                        setMenuItemtDisplay: '<%= Url.ActionCache("SetMenuDisplay", "PageAction") %>',
                        archiveRSSArticle: '<%= Url.ActionCache("ArchiveRssFeed", "RSSLink") %>',
                        unarchiveRSSArticle: '<%= Url.ActionCache("UnArchiveRssFeed", "RSSLink") %>',
                        saveRSSArticle: '<%= Url.ActionCache("SaveRssFeed", "RSSLink") %>',
                        addLink: '<%= Url.ActionCache("AddLink", "Link") %>',
                        assignRSSArticle: '<%= Url.ActionCache("AssignRssFeed", "RSSLink") %>',
                        unassignRSSArticle: '<%= Url.ActionCache("UnAssignRssFeed", "RSSLink") %>',
                        viewAllRSSArticle: '<%= Url.ActionCache("ViewAllArchived", "RSSFeedWidget") %>',
                        saveCustomWidget: '<%= Url.ActionCache("DYNAMIC_ACTION", "DYNAMIC_CONTROLLER") %>',
                        reloadCustomWidget: '<%= Url.ActionCache("DYNAMIC_RELOAD_ACTION", "DYNAMIC_RELOAD_CONTROLLER") %>',
                        partialListViewCustomRss: '<%= Url.ActionCache("PartialListView", "RSSFeedWidget") %>',
                        compactSummaryFacePlateRss: '<%= Url.ActionCache("CompactViewFacePlateLoad", "RSSFeedWidget") %>',
                        menuItemManage: '<%= Url.ActionCache("MenuItemManage", "PageAction") %>',
                        mceAddLink: '<%= Url.ActionCache("MceAddLink", "PageAction") %>',
                        eBookBrowser: '<%= Url.ActionCache("Index", "EbookBrowser") %>',
						
                        saveMenuItem: '<%= Url.ActionCache("SaveMenuItem", "PageAction") %>',
                        save_quiz_settings: '<%= Url.ActionCache("QuizController", "Save") %>',
                        question_settings: '<%= Url.ActionCache("QuestionSettings", "Quiz") %>',
                        lc_question_settings: '<%= Url.ActionCache("LearningCurveQuestionSettings", "Quiz") %>',
                        quiz_question_settings: '<%= Url.ActionCache("QuizQuestionSettings", "Quiz") %>',

                        renameCourse: '<%= Url.ActionCache("RenameCourse", "PageAction") %>',

                        load_page_definiton : '<%= Url.ActionCache("LoadPageDefinition", "Home") %>',
                        showpreviewasvisitor: '<%= Url.ActionCache("ShowPreviewAsVisitor", "Home") %>',
                        resumeEditing: '<%= Url.ActionCache("ResumeEditing", "Home") %>',
                        sort_activities_widget: '<%= Url.ActionCache("ActivitiesSorted", "ActivitiesWidget") %>',
                        ActivitiesWidget_Assign: '<%= Url.ActionCache("Assign", "ActivitiesWidget") %>',
                        ActivitiesWidget_UpdateDueDateColumn: '<%= Url.ActionCache("UpdateDueDateColumn", "ActivitiesWidget") %>',                        
                        unsorted_activities_widget: '<%= Url.ActionCache("Summary", "ActivitiesWidget") %>',
                        activate_eportfolio_course: '<%= Url.ActionCache("ActivateCourse", "Course") %>',
                        delete_eportfolio_course: '<%= Url.ActionCache("DeleteEportfolioCourse", "EportfolioCourse") %>',
                        show_eportfolio_profilewidget: '<%= Url.ActionCache("Summary", "ProfileSummaryWidget")%>',
                        edit_eportfolio_profilewidget: '<%= Url.ActionCache("Edit", "ProfileSummaryWidget")%>',
                        show_eportfolio_coursecustomization: '<%= Url.ActionCache("Summary", "EportfolioCourseSettingsEditorWidget")%>',                        show_create_course_dialog:'<%= Url.ActionCache("CreateCourse", "DashboardCoursesWidget") %>',
                        create_course_from_dashboard:'<%= Url.ActionCache("CreateCourseFromDashboard", "DashboardCoursesWidget") %>',
                        GetAcademicTerms: '<%= Url.ActionCache("GetAcademicTermsByDomain", "DashboardCoursesWidget") %>',
                        create_branch_confirmation: '<%= Url.ActionCache("CreateBranchConfirmation", "DashboardCoursesWidget") %>',
                        delete_course: '<%= Url.ActionCache("DeleteCourse", "DashboardCoursesWidget") %>',
                        show_roster_info: '<%= Url.ActionCache("ViewRoster", "DashboardCoursesWidget") %>',
                        edit_dashboard_course: '<%= Url.ActionCache("EditDashboardCourse", "DashboardCoursesWidget") %>',
                        delete_dashboard_course: '<%= Url.ActionCache("DeleteDashboardCourse", "DashboardCoursesWidget") %>',
                        deactivate_dashboard_course: '<%= Url.ActionCache("DeactivateDashboardCourse", "DashboardCoursesWidget") %>',
                        activate_dashboard_course: '<%= Url.ActionCache("ActivateDashboardCourse", "DashboardCoursesWidget") %>',
                        dashboard_course_option: '<%= Url.ActionCache("CreateCourseOption", "DashboardCoursesWidget") %>',
                        get_current_date_time: '<%= Url.ActionCache("GetCurrentDateTime", "DashboardCoursesWidget") %>',
                        show_upload_and_submit: '<%= Url.ActionCache("UploadAndSubmitForm", "Upload")%>',
                        load_faceplate_loadchildren: '<%= Url.ActionCache("LoadChildrenForParentItem", "LaunchpadTreeWidget") %>',
                        show_faceplate_managementcard: '<%= Url.ActionCache("ManagementCard", "LaunchpadTreeWidget")%>',
                        get_submission_status_management_card: '<%= Url.ActionCache("GetSubmissionStatusForManagementCard", "Assignment")%>',
                        load_faceplate_item: '<%= Url.ActionCache("LoadItem", "LaunchpadTreeWidget")%>',
                        update_faceplate_item_status: '<%= Url.ActionCache("UpdateItemStatus", "LaunchpadTreeWidget")%>',
                        
                        save_launchpad_navigation: '<%= Url.ActionCache("SaveNavigationState", "LaunchpadTreeWidget")%>',
                        launchpad_item_operation: '<%= Url.ActionCache("ItemOperation", "LaunchpadTreeWidget")%>',
                        save_launchpad_navigation_view: '<%= Url.ActionCache("SaveNavigationStateView", "LaunchpadTreeWidget")%>',
                        edit_project_contents: '<%= Url.ActionCache("EditProjectContents", "Project")%>',

                        show_faceplate_resources: '<%= Url.ActionCache("FacePlateBrowseResources", "BrowseMoreResourcesWidget")%>',
                        show_faceplate_resources_console: '<%= Url.ActionCache("FacePlateBrowseResources", "BrowseMoreResourcesWidget")%>',
						
						
						
                        show_faceplate_resources_rss: '<%= Url.ActionCache("FacePlateBrowseRSS", "BrowseMoreResourcesWidget")%>',
                        
                        
                        resource_removed: '<%= Url.ActionCache("FacePlateBrowseRemoved", "BrowseMoreResourcesWidget")%>',
                        resource_mine: '<%= Url.ActionCache("FacePlateBrowseMyResources", "BrowseMoreResourcesWidget")%>',
                        resource_facets: '<%= Url.ActionCache("FacePlateBrowseResourcesFacets", "BrowseMoreResourcesWidget")%>',
                        resource_facets_chapter: '<%= Url.Action("FacePlateBrowseResourcesFacets", "BrowseMoreResourcesWidget", new RouteValueDictionary(new { fieldNames =  "meta-topics/meta-topic_dlap_e", title = "Content by chapter"  }))%>',
                        resource_facets_type: '<%= Url.Action("FacePlateBrowseResourcesFacets", "BrowseMoreResourcesWidget", new RouteValueDictionary(new { fieldNames =  "meta-content-type_dlap_e", title = "Content by type"  }))%>',
                        resource_type_list: '<%= Url.ActionCache("ResourceTypeList", "BrowseMoreResourcesWidget")%>',
                        resource_results: '<%= Url.ActionCache("FacePlateBrowseResourcesResults", "BrowseMoreResourcesWidget")%>',
                        
                        about_widget_refresh: '<%= Url.ActionCache("RefreshWidget", "AboutCourseWidget")%>',
                        update_lms_id: '<%= Url.ActionCache("UpdateLMSID", "AboutCourseWidget")%>',
                        course_activation_widget_refresh: '<%= Url.ActionCache("Refresh", "CourseActivationWidget")%>',

                        start_page: '<%= Url.ActionCache("IndexStart", "Home")%>',
                        update_instructor_console: '<%= Url.ActionCache("Summary", "InstructorConsoleWidget")%>',
                        show_faceplate_resources_string_search: '<%= Url.ActionCache("FacePlateBrowseResourcesStringSearch", "BrowseMoreResourcesWidget")%>',
                        
                        InstructorConsole_FullView: '<%= Url.ActionCache("FullView", "InstructorConsoleWidget")%>',
                        InstructorConsole_EditView: '<%= Url.ActionCache("EditView", "InstructorConsoleWidget")%>',
                        InstructorConsole_BaseUrl: '<%= Url.RouteUrl("CourseSectionHome", new { courseid = "~~courseid~~" }, Request.Url.Scheme) %>',
                        InstructorConsole_FullViewRaw: '<%= Url.Action("FullView", "InstructorConsoleWidget", new {courseId = "~~courseid~~"})%>',
                        InstructorConsole_UpdateCourse: '<%= Url.ActionCache("Update", "InstructorConsoleWidget")%>',
                        InstructorConsole_BatchDueDateUpdater: '<%= Url.ActionCache("BatchDueDateUpdater", "InstructorConsoleWidget")%>',
                        InstructorConsole_BatchDueDateUpdate: '<%= Url.ActionCache("BatchDueDateUpdate", "InstructorConsoleWidget")%>',
                        InstructorConsole_GetBatchDueDateItemCount: '<%= Url.ActionCache("GetBatchDueDateItemCount", "InstructorConsoleWidget")%>',
                        InstructorConsole_GeneralSettings: '<%= Url.ActionCache("Settings", "InstructorConsoleWidget")%>',
                        InstructorConsole_UpdateLaunchPadSettings: '<%= Url.ActionCache("UpdateLaunchPadSettings", "InstructorConsoleWidget")%>',
                        InstructorConsole_SetLaunchpadUnits: '<%= Url.ActionCache("SetLaunchpadUnits", "InstructorConsoleWidget")%>',
                        InstructorConsole_UpdatePassingScore: '<%= Url.ActionCache("UpdatePassingScore", "InstructorConsoleWidget")%>',
                        InstructorConsole_UpdateUseWeightedCategories: '<%= Url.ActionCache("UpdateUseWeightedCategories", "InstructorConsoleWidget")%>',
                        InstructorConsole_GradebookPref: '<%= Url.ActionCache("GradebookPreferences", "InstructorConsoleWidget")%>',
                        InstructorConsole_RenameGradebookCategory: '<%= Url.ActionCache("RenameGradebookCategory", "InstructorConsoleWidget")%>',
                        InstructorConsole_UpdateGradebookCategoryWeight: '<%= Url.ActionCache("UpdateGradebookCategoryWeight", "InstructorConsoleWidget")%>',
                        InstructorConsole_UpdateGradebookCategoryDropLowest: '<%= Url.ActionCache("UpdateGradebookCategoryDropLowest", "InstructorConsoleWidget")%>',
                        InstructorConsole_MoveGradebookCategory: '<%= Url.ActionCache("MoveGradebookCategory", "InstructorConsoleWidget")%>',
                        InstructorConsole_RemoveGradebookCategory: '<%= Url.ActionCache("RemoveGradebookCategory", "InstructorConsoleWidget")%>',
                        InstructorConsole_MoveGradebookItem: '<%= Url.ActionCache("MoveGradebookItem", "InstructorConsoleWidget")%>',
                        InstructorConsole_GradebookExport: '<%= Url.ActionCache("ExportGradebook", "InstructorConsoleWidget")%>',

                        Calendar_View: '<%= Url.ActionCache("View", "CalendarWidget")%>',
                        Calendar_GetAssignments: '<%= Url.ActionCache("GetAssignments", "CalendarWidget")%>',
                        Calendar_ListView: '<%= Url.ActionCache("AgendaFullView", "CalendarWidget")%>',
                        Calendar_MonthView: '<%= Url.ActionCache("MonthFullView", "CalendarWidget")%>',

                        CrunchitBridgePage: '<%= Url.ActionCache("CrunchItBridgePage", "ContentWidget")%>',
						
                        content_item_hide: '<%= Url.ActionCache("HideItem", "ContentWidget") %>',
                        content_item_show: '<%= Url.ActionCache("ShowItem", "ContentWidget") %>',
                        programcourse_term_courses: '<%= Url.ActionCache("TermCourses", "ProgramCourseWidget") %>',
                        show_faceplate_editcontent: '<%= Url.ActionCache("EditContentView", "LaunchpadTreeWidget") %>',
                        contentwidget_isassigndatevalid: '<%= Url.ActionCache("IsAssignDateValid", "ContentWidget") %>',
                        contentwidget_show_gradebookcategory: '<%= Url.ActionCache("ShowAddGradeBookCateagory", "ContentWidget") %>',
                        ProgramDashboard_ReAssignInstructor_Index: '<%= Url.ActionCache("ReAssignInstructor", "ProgramCourseWidget") %>',
                        ProgramDashboard_ReAssignInstructor_Update: '<%= Url.ActionCache("UpdateInstructor", "ProgramCourseWidget") %>',                       
                        show_faceplate_moveorcopy: '<%= Url.ActionCache("ShowMoveOrCopy", "FacePlate") %>',
                        faceplate_moveitem: '<%= Url.ActionCache("MoveContentItem", "FacePlate") %>',
                        ItemHeirachyHasSubmission: '<%= Url.ActionCache("ItemHeirachyHasSubmission", "FacePlate") %>',
                        my_material_remove_item: '<%= Url.ActionCache("RemoveItemMyMaterial","FacePlate")%>',
                        
                        assignmentsettings_search_class_roster: '<%= Url.ActionCache("SearchClassRoster", "AssessmentSettings") %>',

                        show_current_due_item_widget: '<%= Url.ActionCache("LaunchPad", "AssignmentWidget") %>',
                        ecommerce_instructor_hascourse: '<%= Url.ActionCache("HasDerivedCourses", "ECommerce") %>',

                        LearningCurve_GetData: '<%= Url.ActionCache("GenerateLearningCurvePlayerContent", "LearningCurveActivity") %>',  
                        LearningCurve_UpdateTargetScoreInItem: '<%= Url.ActionCache("UpdateTargetScoreInItem", "LearningCurveActivity") %>',
                        LearningCurve_GetEbookInfo: '<%= Url.ActionCache("GetEbookInfo", "LearningCurveActivity") %>',
                        LearningCurve_GetUserEmailAddressJsonResult: '<%= Url.ActionCache("GetUserEmailAddressJsonResult", "LearningCurveActivity") %>',
                                           
                        upload_image: '<%= Url.ActionCache("UploadImage", "Upload") %>',
                        Upload_TinyMceUploadDocumentForm: '<%= Url.ActionCache("TinyMceUploadDocumentForm", "Upload") %>',
                        upload_imageupload: '<%= Url.ActionCache("ImageUpload", "Upload") %>',  
                        document_download: '<%= Url.ActionCache("Document", "Downloads") %>',  

                        addLinkToCollection: '<%= Url.ActionCache("AddLinkToCollection", "LinkCollection") %>',

                        GetEnrollmentTerms: '<%= Url.ActionCache("GetTerms", "ECommerce") %>',
                        GetEnrollmentInstructor: '<%= Url.ActionCache("GetInstructors", "ECommerce") %>',
                        GetEnrollmentCourses: '<%= Url.ActionCache("GetInstructorCourseList", "ECommerce") %>',
                        GetCoursesDetails: '<%= Url.ActionCache("Join", "ECommerce") %>',
                        ECommerceEnroll: '<%= Url.ActionCache("Enroll", "ECommerce") %>',
                        ShowEnrollmentConfirmation: '<%= Url.ActionCache("EnrollmentConfirmation", "ECommerce") %>',
                        search_onyx_school :'<%= Url.ActionCache("FindOnyxSchool", "Course") %>',

                        // calling the widget view that shows adding a question to existing quizzes
                        show_add_question_to_existing_assessment: '<%= Url.ActionCache("AddQuestionToExistingAssessment", "FacePlate") %>',
                        add_question_to_quiz: '<%= Url.ActionCache("AddQuestionToQuiz", "Quiz") %>',
                        show_add_to_new_assessment:'<%= Url.ActionCache("ShowAddToNewAssessment", "Quiz") %>',

                        view_gradebook_assignedscores: '<%= Url.ActionCache("AssignedScores", "GradebookWidget") %>',
                        view_gradebook_otherscores: '<%= Url.ActionCache("GetItemGrades", "GradebookWidget") %>',
                        view_gradebook_itemscores: '<%= Url.ActionCache("ItemScores", "GradebookWidget") %>',
                        view_gradebook_studentdetail: '<%= Url.ActionCache("StudentDetail", "GradebookWidget") %>',
                        view_gradebook_student: '<%= Url.ActionCache("StudentGradebook", "GradebookWidget") %>',
                        view_gradebook_assignmentsummary: '<%= Url.ActionCache("AssignmentSummary", "GradebookWidget") %>',
                        download_assigneditem_report: '<%= Url.ActionCache("ExportAssignedReport", "GradebookWidget") %>',

                        beingmeta_mock: '<%= Url.ActionCache("BeingMetaSearch", "BeingMetaMock") %>',
                        beingmeta_search: '<%= ConfigurationManager.AppSettings["BeingMetaSearchUrl"] %>',
                        close_persistent_qtip: '<%= Url.ActionCache("ClosePersistentQtip", "PersistingQtips") %>',
                        get_related_items: '<%= Url.ActionCache("GetRelatedContents", "ContentAreaWidget") %>',
                        get_product_course: '<%= Url.ActionCache("GetContextCourse", "Course") %>',
                        log_javascript_errors: '<%= Url.ActionCache("LogJSError", "Log") %>'
                    },

                    context: {
                        BaseUrl: '<%= Url.RouteUrl("CourseSectionHome") %>',
                        IsAuthenticated: '<% Html.RenderAction("UserAuthenticated", "Account"); %>',
                        product_type: '<% Html.RenderAction("GetProductType", "Course"); %>',
                        course_type: '<% Html.RenderAction("GetCourseType", "Course"); %>',
                        course_tinyMCE_options: '<% Html.RenderAction("GetCourseTinyMCEOptions", "Course"); %>',
                        CourseActivationEnabled: '<% Html.RenderAction("GetCourseActivation", "Course"); %>',
                        CurrentUserId: '<% Html.RenderAction("CurrentUserId", "Account"); %>',
                        IsInstructor: '<% Html.RenderAction("IsInstructor", "Account"); %>',
                        IsProductCourse: '<% Html.RenderAction("IsProductCourse", "Course"); %>',
                        IsSandboxCourse: '<% Html.RenderAction("IsSandboxCourse", "Course"); %>',
                       
                        DataTablesToolsSwf: '<%= Url.ContentCache("~/Scripts/DataTablesExtras/copy_csv_xls.swf") %>'
                    },
                    externalurlprefix: '<%= ConfigurationManager.AppSettings["xUrlPrefix"] %>',
                    questionFilter: JSON.parse('<% Html.RenderAction("QuestionFilter", "Quiz"); %>')
                });
            });
        }, depsIncluded);

    }(jQuery));

    function sessionKeepAlive() {
        var isAuthenticated = (PxPage.Context.IsAuthenticated == 'true');
        if (isAuthenticated) {
            setTimeout(sessionKeepAlive, <%= System.Configuration.ConfigurationManager.AppSettings["KeepAliveInterval"] %>);

            var url = '<%= Url.Action("KeepAlive", "Account") %>';

            if (jQuery('.keep-alive-frame').length > 0) {
                jQuery('.keep-alive-frame').remove();
            }

            jQuery('body').append('<iframe class="keep-alive-frame" style="display:none;" src="' + url + '" />');
        }
    }

    setTimeout(sessionKeepAlive, 60000);
</script>