<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<script language="javascript" type="text/javascript">
//    tinyMCE.init({
//        theme: "advanced",
//        mode: "none",
//        plugins: "table,paste,nonbreaking",
//        theme_advanced_toolbar_location: "external",
//        theme_advanced_toolbar_align: "left",
//        theme_advanced_buttons1: "bold,italic,underline,|,justifyleft,justifycenter,justifyright,|,link,unlink,|,forecolor,backcolor,|,bullist,numlist,|,hr,blockquote,charmap,code,|,styleselect,formatselect",
//        theme_advanced_buttons2: "",
//        theme_advanced_buttons3: "",
//        add_unload_trigger: false,
//        remove_linebreaks: false,
//        debug: false,
//        width: "100%",
//        handle_event_callback: "set_current_editor"
//    });
//    var editor_onmenu = false;
//    function set_current_editor(e) {
//        //  this only shows the tinymce menu for the current tinymce editor
//        var editorid = e.target.editorId; // grab the id of the current editor
//        if (editorid) {
//            $('.mceToolbarExternal').hide(); // use jquery to hide all the tinymce toolbars
//            if (editor_onmenu || e.type != "blur") {
//                // if the user is still in the editor - we show its toolbar - again using jquery
//                $('#' + editorid + "_toolbar").show();
//            }
//        }
//        return true; // Continue handling
//    }
//    function editor_init() {
//        // use jquery to find all <textarea class="editor"> elements inside the iframe
//        // and then add tinymce to them
//        $(".editor", editor_frame.document).each(
//        function () {
//            //alert("Binding to id: "+$(this).attr('id'));
//            tinyMCE.addMCEControl(this, $(this).attr('id'), editor_frame.document);
//        }
//    );
//        $('.mceToolbarExternal').mouseover(function () {
//            editor_onmenu = true;
//        });
//        $('.mceToolbarExternal').mouseout(function () {
//            editor_onmenu = false;
//        });        
//    }

</script>
