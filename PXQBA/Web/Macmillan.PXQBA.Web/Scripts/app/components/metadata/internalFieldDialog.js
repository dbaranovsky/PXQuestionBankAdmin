/**
* @jsx React.DOM
*/

var InternalFieldDialog = React.createClass({displayName: 'InternalFieldDialog',

    getInitialState: function() {
        return { value: this.props.value };
    },

    editInternalFieldHandler: function() {
       this.props.updateHandler(this.props.itemIndex, "internalName",  this.state.value)
    },

    onChangeHandler: function(text) {
        this.setState( {value: text} );
    },

    render: function() {
 
        var self = this;
        var renderHeaderText = function() {
             return "Edit internal name";
        };
      
        var renderBody = function(){
             return (React.DOM.div(null, 
                        React.DOM.div(null, 
                            React.DOM.div(null,  " Internal name: " ), " ", TextEditor( {value:self.state.value, dataChangeHandler:self.onChangeHandler})
                        ),
                         React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal"}, "Cancel"),
                                 React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal", onClick:self.editInternalFieldHandler}, "Edit")
                            )
                    )
            );
        };

        return (ModalDialog( {showOnCreate:true,
                             renderHeaderText:renderHeaderText, 
                             renderBody:renderBody, 
                             closeDialogHandler:  this.props.closeDialogHandler,
                             dialogId:"internalFieldDialog"}));
    }
});




