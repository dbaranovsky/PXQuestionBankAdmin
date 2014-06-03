/**
* @jsx React.DOM
*/

var MetadataChapterEditorTab = React.createClass({displayName: 'MetadataChapterEditorTab',

  chaptersName: "chapters",
  banksName: "banks",

	changeHandler: function(fieldName, text) {
      var data = this.props.data;
      data[fieldName] = text;
      this.props.dataChangeHandler(data);
	},

  render: function() {
       return (
       		React.DOM.div(null, 
              React.DOM.div(null,  
              React.DOM.p(null, 
                "Each question in this title must be aligned to a specific chapter and bank (as listed here). List all chapters"+' '+ 
                "and banks in the area below (one per line). Chapters and banks will appear to editors and instructors in the order listed."
              )
              ),
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
                              dataChangeHandler:this.changeHandler.bind(this, this.chaptersName), 
                              value:this.props.data[this.chaptersName]} )
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
                            dataChangeHandler:this.changeHandler.bind(this, this.banksName), 
                            value:this.props.data[this.banksName]} )
                        )
                      )
                    )
                    )
                )
 			   )
            );
  }
});




