(function () {

    var DOM = tinymce.DOM, Element = tinymce.dom.Element, Event = tinymce.dom.Event, each = tinymce.each, is = tinymce.is, t = this;

    // Load plugin specific language pack
    //tinymce.PluginManager.requireLangPack('example');

    tinymce.create('tinymce.plugins.CustomButton', {

        getInfo: function () {
            return {
                longname: 'custom button',
                author: 'mishka',
                //authorurl : 'http://',
                //infourl : 'http://',
                version: "1.0"
            };
        },

        init: function (ed) {

            ///add custom class for plugin
            //			ed.onBeforeRenderUI.add(function() {
            ////				ed.customButton = new tinymce.customButton(ed);
            //				DOM.loadCSS(url + '/css/customButton.css'); 
            //			});

            t.editor = ed;

            // Register commands
            //			ed.addCommand('cbclick', t._popup, t);
            //			//add button
            //			ed.addButton('cb2', {title : 'Custom Button 2', class : 'mce_preview', cmd : 'cbclick'});	
            //			//on change
            //			ed.onNodeChange.add(t._nodeChange, t);

            // Add a custom button
            ed.addButton('customButton', {
                'class': 'mce_link',
                title: 'AddLink.desc',
                cmd: 't._popup',
                onclick: function () {
                    //ed.selection.setContent('<STRONG>Hello world!</STRONG>');
                    var selectedHTML = ed.selection.getContent();
                    PxPage.mceAddLink(selectedHTML);
                    //tinyMCE.activeEditor.windowManager.alert('Button was clicked.');
                }
            });

            // Add a node change handler, selects the button in the UI when a image is selected
            //ed.onNodeChange.add(function(ed, cm, n) { cm.setActive('customButton', n.nodeName == 'IMG'); });

        }, // end init

        createControl: function (n, cm) {
            return null;
        },  // end create control

        //		_nodeChange : function(ed, cm, n) {var ed = this.editor;cm.setDisabled('cb', ed.isDirty());ed.windowManager.alert('enabled ' + ed.isDirty());},

        _popup: function () {
            tinyMCE.activeEditor.selection.setContent('<STRONG>Hello world!</STRONG>');
        }


    });  //end tinymce.create

    // Register plugin
    tinymce.PluginManager.add('customButton', tinymce.plugins.CustomButton);
})();