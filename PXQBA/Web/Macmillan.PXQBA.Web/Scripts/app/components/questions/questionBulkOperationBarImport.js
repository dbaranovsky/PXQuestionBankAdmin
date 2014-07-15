/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationBarImport = React.createClass({displayName: 'QuestionBulkOperationBarImport',

    deselectsAllHandler: function() {
        this.props.deselectsAllHandler();
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
                               React.DOM.div( {className:"bulk-operation-item", 'data-toggle':"tooltip", title:  "  !!!!!!!!!!!!"}, 
                                 "!"
                               )
                            ),
                            React.DOM.td( {className:"bulk-operation-cell"}, 
                               React.DOM.div( {className:"deselect-button", onClick:this.deselectsAllHandler, 'data-toggle':"tooltip", title:"Deselect all"}, 
                                 React.DOM.span(null ,  " X " )
                               )
                            )
                          )
                        )
            );
        }
});