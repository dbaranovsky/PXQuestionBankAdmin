/**
* $Id: editor_plugin_src.js 520 2008-01-07 16:30:32Z spocke $
*
* @author Moxiecode
* @copyright Copyright © 2004-2008, Moxiecode Systems AB, All rights reserved.
*/

(function () {
    tinymce.create('tinymce.plugins.netImageBrowser', {
        init: function (ed, url) {
            // Register commands
            ed.addCommand('mcenetImageBrowser', function () {
                // determine redirect url 
                var urlRef = window.top.location.href;
                var urlSplit = urlRef.split("/");
                var urlRedirect = "";
                for (var i = 0, len = urlSplit.length; i < len; ++i) {
                    urlRedirect += urlSplit[i] + "/";
                    if (urlSplit[i] == "lms") {
                        urlRedirect += urlSplit[i + 1] + "/WritingTab/ImageUpload";
                        break;
                    }
                }  // end determine redirect url 
                ed.windowManager.open({
                    file: urlRedirect,
                    width: 660 + parseInt(ed.getLang('netBrowser.delta_width', 0)),
                    height: 313 + parseInt(ed.getLang('netBrowser.delta_height', 0)),
                    inline: 1
                }, {
                    plugin_url: url
                });
            });
            // Register buttons
            ed.addButton('netImageBrowser', { title: 'Image Browser', cmd: 'mcenetImageBrowser' });
        },

        getInfo: function () {
            return {
                longname: 'netImageBrowser',
                author: 'iLyas Osmanogullari',
                authorurl: 'http://ilyax.com',
                infourl: 'http://ilyax.com/imagebrowser/',
                version: tinymce.majorVersion + "." + tinymce.minorVersion
            };
        }
    });

    // Register plugin
    tinymce.PluginManager.add('netImageBrowser', tinymce.plugins.netImageBrowser);
})();