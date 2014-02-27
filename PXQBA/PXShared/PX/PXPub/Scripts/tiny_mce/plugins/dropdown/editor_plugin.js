(function () {
    tinymce.create('tinymce.plugins.dropdown', {
        createControl: function (n, cm) {
            switch (n) {
                case 'quickLinkList':
                    var quickLink = cm.createListBox('quickLinkList', { title: 'e-Book Quicklinks' });

                    quickLink.add('Chapter 1', 'Chapter1');
                    quickLink.add('Chapter 2', 'Chapter2');
                    quickLink.add('Chapter 3', 'Chapter3');
                    quickLink.add('Chapter 4', 'Chapter4');

                    // Return the new listbox instance
                    return quickLink;
            }

            return null;
        },

        getInfo: function () {
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
