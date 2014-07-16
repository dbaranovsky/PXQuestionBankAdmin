/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationBarImport = React.createClass({displayName: 'QuestionBulkOperationBarImport',

    backHandler: function() {
      alert('back');
    },

    importHandler: function() {
      alert('import');
    },


    render: function() {
        return ( 
                 React.DOM.table( {className:"bulk-operation-bar-table"}, 
                          React.DOM.tr(null, 
                            React.DOM.td( {className:"bulk-operation-cell"}, 
                              React.DOM.div( {className:"bulk-operation-item"}, 
                                 React.DOM.span(null,  " ", this.props.message,  "  "  )
                               )
                            ),
                            React.DOM.td( {className:"bulk-operation-cell"}, 
                              React.DOM.div( {className:"bulk-operation-item"}, 
                                React.DOM.button( {type:"button", className:"btn btn-primary",  onClick:this.importHandler}, 
                                    "Import questions to..."
                                )
                              )
                            ),
                            React.DOM.td( {className:"bulk-operation-cell"}, 
                              React.DOM.div( {className:"bulk-operation-item"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-default", onClick:this.backHandler}, 
                                     "Back"
                                  )
                              )
                            )
                          )
                        )
            );
        }
});