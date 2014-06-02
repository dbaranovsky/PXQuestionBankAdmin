/**
* @jsx React.DOM
*/

var MetadataChapterEditorTab = React.createClass({displayName: 'MetadataChapterEditorTab',

	changeHandler: function(fieldName, text) {
      var data = this.props.data;
      data[fieldName] = text;
      this.props.dataChangeHandler(data);
	},

  render: function() {
       return (
       		React.DOM.div(null, 
               React.DOM.table(null, 
                  React.DOM.tr(null, 
                    React.DOM.td(null, 
                      React.DOM.div( {className:"metadata-multi-line-container"}, 
                        React.DOM.div( {className:"metadata-multi-line-lable"},   
                            "Chapters/Modules" 
                        ),
 
                         React.DOM.div(null,   
                            TextAreaEditor( 
                             {classNameProps:"metadata-multi-line-editor",
                              dataChangeHandler:this.changeHandler.bind(this, "chapters"), 
                              value:this.props.data.chapters} )
                         )
                      )
                    ),
                    React.DOM.td(null, 
                      React.DOM.div( {className:"metadata-multi-line-container"}, 
                        React.DOM.div( {className:"metadata-multi-line-lable"},  
                             "Banks"
                        ),

                        React.DOM.div(null,   
                           TextAreaEditor( 
                            {classNameProps:"metadata-multi-line-editor",
                            dataChangeHandler:this.changeHandler.bind(this, "banks"), 
                            value:this.props.data.banks} )
                        )
                      )
                    )
                    )
                )
 			   )
            );
  }
});




