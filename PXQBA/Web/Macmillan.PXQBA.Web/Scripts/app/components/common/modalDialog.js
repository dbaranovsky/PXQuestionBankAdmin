/**
* @jsx React.DOM
*/

var ModalDialog = React.createClass({displayName: 'ModalDialog',

    onClose: function(){
        if(typeof this.props.closeDialogHandler !== 'undefined'){
            this.props.closeDialogHandler();
        }

    },

    render: function() {
        return (
            React.DOM.div( {className:"modal fade", id:this.props.dialogId, tabIndex:"-1", role:"dialog", 'aria-labelledby':"addQuestionModalLabel", 'aria-hidden':"true"} , 
                React.DOM.div( {className:"modal-dialog"}, 
                    React.DOM.div( {className:"modal-content"}, 
                        React.DOM.div( {className:"modal-header"}, 
                            React.DOM.button( {type:"button", className:"close", 'data-dismiss':"modal", 'aria-hidden':"true", onClick:this.onClose}, "×"),
                            React.DOM.h4( {className:"modal-title", id:"myModalLabel"}, this.props.renderHeaderText())
                        ),
                        React.DOM.div( {className:"modal-body"} , 
                            this.props.renderBody()
                        ),
                       
                      this.props.renderFooterButtons()
                    
                    )
                )
            )
            );
    }
});
