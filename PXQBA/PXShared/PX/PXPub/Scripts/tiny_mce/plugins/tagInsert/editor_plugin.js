tinymce.create('tinymce.plugins.TagInsert',{
	createControl: function(n, cm) {
		switch (n) {
			case 'taginsert':
				var mlb = cm.createListBox('taginsert', {
					 title : 'Insert Template Tag',
					 onselect : function(v) {
						 tinyMCE.activeEditor.selection.setContent(v);
					 }
				});

				// Add values to the list box
				mlb.add("display-name1", "{tag-name1}");
				mlb.add("display-name2", "{tag-name2}");
				mlb.add("display-name3", "{tag-name3}");
				mlb.add("display-name4", "{tag-name4}");
				// etc.

				// Return the new listbox instance
				return mlb;
		}
		return null;
	}
});
tinymce.PluginManager.add('taginsert', tinymce.plugins.TagInsert);