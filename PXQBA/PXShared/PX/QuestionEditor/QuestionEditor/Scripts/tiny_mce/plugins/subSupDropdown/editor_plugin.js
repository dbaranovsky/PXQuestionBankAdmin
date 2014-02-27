(function() {
	
	tinymce.create('tinymce.plugins.SubSupDropdown', {

		getInfo: function ()
		{
			return {
				longname: 'Subscript / Superscript Dropdown',
				author: 'MacMillan Higher Education',
				authorurl: 'mishka',
				//infourl: '',
				version: 0.1 //tinymce.majorVersion + "." + tinymce.minorVersion
			};
		},

		int: function (n, cm)
		{
			return null;
		},
		
		
		createControl: function(n, cm) {
	        switch( n ) {
	            case 'subSupDropdown':
	                var c = cm.createSplitButton('subSupDropdown', {
	                    title : 'Subscript \\ Superscript',
	                    'class': 'mce_sub',
	                    onclick: function() {
	                        //tinymce.activeEditor.execCommand('mcesub');
							//tinymce.activeEditor.SplitButton('mceSplitButtonHover');
	                    }
	                });

	                c.onRenderMenu.add(function (c, m) 
					{
						m.add({ title: 'Sub/Sup text', 'class': 'mceMenuItemTitle' }).setDisabled(1);
						    
						m.add({
							title: 'Subscript', 
							icon: 'sub',
							'class': 'mce_sub',
							cmd: 'subscript'
						});
						
						//m.addSeparator();
						
						m.add({
							title: 'Superscript', 
							icon: 'sup',
							'class': 'mce_sup',
							cmd: 'superscript'
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
	tinymce.PluginManager.add('subSupDropdown', tinymce.plugins.SubSupDropdown);
	
})();