(function() {

	tinymce.create('tinymce.plugins.TextOpsDropdown', {

		getInfo: function ()
		{
			return {
				longname: 'TextOpsDropdown',
				author: 'MacMillan Higher Education',
				authorurl: 'mishka',
				//infourl: '',
				version: 0.1 //tinymce.majorVersion + "." + tinymce.minorVersion
			};
		},

		int : function(n, cm) {
			return null;
		},

	    createControl: function(n, cm) {
	        switch( n ) {
	            case 'textOpsDropdown':
	                var c = cm.createSplitButton('textOpsDropdown', {
	                    title : 'Text Operations',
						icon : 'cut',
	                    'class': 'mce_cut',
	                    onclick: function() {
	                        //tinymce.activeEditor.execCommand('mceInsertTable');
							//tinymce.activeEditor.SplitButton('mceSplitButtonHover');
	                    }
	                });
					
	                c.onRenderMenu.add(function(c, m) {

	                	m.add({ title: 'Text Operations', 'class': 'mceMenuItemTitle' }).setDisabled(1);

						m.add({
							title: 'Cut', 
							icon: 'cut',
							'class': 'mce_cut',
							cmd: 'cut'
						});
						//m.addSeparator();
						m.add({
							title: 'Copy', 
							icon: 'copy',
							'class': 'mce_copy',
							cmd: 'copy'
						});
						m.add({
							title: 'Paste', 
							icon: 'paste',
							'class': 'mce_paste',
							cmd: 'paste'
						});
						m.add({
							title: 'Paste as Plain Text', 
							icon: 'pastetext',
							'class': 'mce_pastetext',
							cmd: 'pastetext'
						});
						m.add({
							title: 'Paste from Word', 
							icon: 'pasteword',
							'class': 'mce_pasteword',
							cmd: 'pasteword'
						});
						
						// This really needs to be done with a callback
						setTimeout( function() {
							c.editor.nodeChanged();
						}, 50);
						
					});
	                
	                return c;
	        }
	
	        return null;
	        
	    }
	});
	
	// Register plugin
	tinymce.PluginManager.add('textOpsDropdown', tinymce.plugins.TextOpsDropdown);
	
})();