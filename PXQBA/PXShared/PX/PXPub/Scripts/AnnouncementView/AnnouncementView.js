//Provides a simple Page helper for the application
var PxAnnouncements = function($) {
    return {
        setupEditor: function(prefix) {
            //  Add click event to open link
            $('#' + prefix + '-open').click(function(event) {
                event.preventDefault();
                var prefix = event.target.id.substring(0, event.target.id.length - '-open'.length);
                $('#' + prefix + '-form, #' + prefix + '-clear, #' + prefix + '-open').toggleClass('disabled');

                // Set the title and body edit fields to their correct initial values
                var editor = tinyMCE.get(prefix + '-editbody');
                if (prefix === 'add') {
                    $('#add-edittitle').val('');
                    editor.setContent('');
                } else {
                    var title = $('#' + prefix + '-title')[0].innerHTML;
                    $('#' + prefix + '-edittitle').val(title);
                    var body = $('#' + prefix + '-body')[0].innerHTML;
                    editor.setContent(body);
                }
            });

            // Add click event to clear (cancel) link
            $('#' + prefix + '-clear').click(function(event) {
                event.preventDefault();
                var prefix = event.target.id.substring(0, event.target.id.length - '-clear'.length);
                $('#' + prefix + '-form, #' + prefix + '-clear, #' + prefix + '-open').toggleClass('disabled');
            });
        },
        init: function() {
            PxAnnouncements.setupEditor('add');
            var editLinks = $('ul li div.announcement-editor a.open-button');
            $.each(editLinks, function(index, value) {
                var prefix = value.id.substring(0, value.id.length - '-open'.length);
                PxAnnouncements.setupEditor(prefix);
            });
        }
    };
} (jQuery);

jQuery(document).ready(function() {
    PxAnnouncements.init();
});