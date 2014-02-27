(function ()
{

	tinymce.create('tinymce.plugins.IndentDropdown', {

		getInfo: function ()
		{
			return {
				longname: 'Indent / Outdent Dropdown',
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

		createControl: function (n, cm)
		{
			switch (n) {
				case 'indentDropdown':
					var c = cm.createSplitButton('indentDropdown', {
						title: 'IndentDropdown.desc',
						icon: 'indent',
						'class': 'mce_indent',
						onclick: function ()
						{
							tinyMCE.activeEditor.selection.setContent('');
							//tinyMCE.activeEditor.execCommand('mceInsertContent', false, n);
							//tinymce.activeEditor.SplitButton('mceSplitButtonHover');
							//tinyMCE.activeEditor.windowManager.alert('Button was clicked.');
						}
					});

					c.onRenderMenu.add(function (c, m)
					{
						m.add({ title: 'Indent text', 'class': 'mceMenuItemTitle' }).setDisabled(1);

						m.add({
							title: 'Increase Indent',
							icon: 'indent',
							'class': 'mce_indent',
							cmd: 'indent'
						});
						//m.addSeparator();
						m.add({
							title: 'Decrease Indent',
							icon: 'outdent',
							'class': 'mce_outdent',
							cmd: 'outdent'
						});

						// This really needs to be done with a callback
						setTimeout(function ()
						{
							c.editor.nodeChanged();
						}, 50);

					});

					return c;
			}

			return null;

		}
	});

	// Register plugin
	tinymce.PluginManager.add('indentDropdown', tinymce.plugins.IndentDropdown);

})();