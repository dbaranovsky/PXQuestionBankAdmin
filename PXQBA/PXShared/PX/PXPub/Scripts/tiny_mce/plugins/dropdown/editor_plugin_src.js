/**
* editor_plugin_src.js
*
* Copyright 2009, Moxiecode Systems AB
* Released under LGPL License.
*
* License: http://tinymce.moxiecode.com/license
* Contributing: http://tinymce.moxiecode.com/contributing
*/
(function() {
    tinymce.create('tinymce.plugins.dropdown', {
        createControl: function(n, cm) {
            switch (n) {
                case 'mylistbox':
                    var mlb = cm.createListBox('mylistbox', {
                },
                    tinymce.ui.NativeListBox);

                mlb.onPostRender.add(function(t) {
                    tinymce.dom.Event.add(t.id, 'change', function(e) {
                        var v = e.target.options[e.target.selectedIndex].value;
                        $('.clsDiv').hide();
                        $('#' + v).show();
                        $('#attributeTitle').show();
                        $('#attributeList').val(v);
                        $('.mceNativeListBox').val('');
                        e.target.selectedIndex = 0;
                    });
                });

                mlb.add('Writing', 'Writing');
                mlb.add('Details', 'Details');
                mlb.add('Rubrics', 'Rubrics');
                mlb.add('References', 'References');

                return mlb;

            case 'splitbutton':
                var c = cm.createSplitButton('splitbutton', {
                    title: 'split button',
                    image: 'img/example.gif',
                    onclick: function() {
                    }
                });

                c.onRenderMenu.add(function(c, m) {
                    m.add({ title: 'Some title', 'class': 'mceMenuItemTitle' }).setDisabled(1);
                    m.add({ title: 'Some item 1', onclick: function() {
                        tinyMCE.activeEditor.windowManager.alert('Some  item 1 was clicked.');
                    }
                    });
                    m.add({ title: 'Some item 2', onclick: function() {
                        tinyMCE.activeEditor.windowManager.alert('Some  item 2 was clicked.');
                    }
                    });
                });
                return c;
        }
        return null;
    },
    getInfo: function() {
        return { longname: "dropdown plugin",
            author: "Moxiecode Systems AB",
            authorurl: "http://tinymce.moxiecode.com",
            infourl: "http://wiki.moxiecode.com/index.php/TinyMCE:Plugins/dropdown",
            version: tinymce.majorVersion + "." + tinymce.minorVersion
        }
    }
});
// Register plugin with a short name
tinymce.PluginManager.add('dropdown', tinymce.plugins.dropdown);
})();
