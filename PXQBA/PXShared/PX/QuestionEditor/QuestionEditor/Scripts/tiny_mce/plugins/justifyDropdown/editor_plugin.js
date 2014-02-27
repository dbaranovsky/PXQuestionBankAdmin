(function() {
	
	tinymce.create('tinymce.plugins.JustifyDropdown', {

		getInfo: function ()
		{
			return {
				longname: 'Alignment options Dropdown',
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
	            case 'justifyDropdown':
	                var c = cm.createSplitButton('justifyDropdown', {
	                    title : 'Align',
	                    'class': 'mce_justifyleft',
	                    onclick: function() {
	                        //tinymce.activeEditor.execCommand('mceInsertTable');
							//tinymce.activeEditor.SplitButton('mceSplitButtonHover');
	                    }
	                });
					
	                c.onRenderMenu.add(function(c, m) {
	                	m.add({ title: 'Align text', 'class': 'mceMenuItemTitle' }).setDisabled(1);

						m.add({
							title: 'Align left', 
							icon: 'justifyleft',
							'class': 'mce_justifyleft',
							cmd: 'justifyleft'
						});
						
						//m.addSeparator();
						
						m.add({
							title: 'Align center', 
							icon: 'justifycenter',
							'class': 'mce_justifycenter',
							cmd: 'justifycenter'
						});
						
						//m.addSeparator();
						
						m.add({
							title: 'Align right', 
							icon: 'justifyright',
							'class': 'mce_justifyright',
							cmd: 'justifyright'
						});
						
						//m.addSeparator();
						
						m.add({
							title: 'Align full', 
							icon: 'justifyfull',
							'class': 'mce_justifyfull',
							cmd: 'justifyfull'
						});
						
						
						// Enable/disable menu items on node change
						//c.editor.onNodeChange.add( function(ed, cm, n, co) {
						//	var items = items = ed.controlManager.controls[ed.id + '_justifyDropdown'].menu.items,
						//		p = ed.dom.getParent(ed.selection.getStart(), 'p,br,caption');
						//	if( p && p.nodeName === 'STYLE' && p.nodeName != 'UNDEFINED' ) p = 0;
						//	tinymce.each(items, function(t) {
							//alert(t.settings.icon); //leftjustify, justifycenter, justifyright, justifyfull
						//		if( t.settings.icon === 'leftjustify' ) return;
						//		t.setActive(!p);
						//	});
						//});
						
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
	tinymce.PluginManager.add('justifyDropdown', tinymce.plugins.JustifyDropdown);
	
})();