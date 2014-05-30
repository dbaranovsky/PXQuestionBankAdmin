/**
* @jsx React.DOM
*/

var MetadataTabs = React.createClass({displayName: 'MetadataTabs',

    render: function() {
       return (
            React.DOM.div(null, 
              React.DOM.ul( {className:"nav nav-tabs"}, 
                React.DOM.li( {className:"active"},  
                    React.DOM.a( {href:"#chaptersTab", 'data-toggle':"tab"}, "Define Chapters and Banks")
                ),
                React.DOM.li(null, 
                    React.DOM.a( {href:"#metadataTab", 'data-toggle':"tab"}, "Add Title-Specific Metadata Fields")
                )
              ),

               React.DOM.div( {className:"tab-content"}, 
                    React.DOM.div( {className:"tab-pane active", id:"chaptersTab"}, 
                        MetadataChapterEditorTab(null )
                    ),
                    React.DOM.div( {className:"tab-pane", id:"metadataTab"}, 
                        MetadataMetaEditorTab(null )
                    )
                )
            )
            );
    }
});




