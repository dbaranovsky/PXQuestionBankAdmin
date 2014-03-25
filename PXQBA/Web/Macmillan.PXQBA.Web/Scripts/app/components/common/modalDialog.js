/**
* @jsx React.DOM
*/

var ModalDialog = React.createClass({displayName: 'ModalDialog',

    render: function() {
        return (
            React.DOM.div( {className:"modal fade", id:"addQuestionModal", tabIndex:"-1", role:"dialog", 'aria-labelledby':"addQuestionModalLabel", 'aria-hidden':"true"}, 
                React.DOM.div( {className:"modal-dialog"}, 
                    React.DOM.div( {className:"modal-content"}, 
                        React.DOM.div( {className:"modal-header"}, 
                            React.DOM.button( {type:"button", className:"close", 'data-dismiss':"modal", 'aria-hidden':"true"}, "×"),
                            React.DOM.h4( {className:"modal-title", id:"myModalLabel"}, this.props.renderHeaderText())
                        ),
                        React.DOM.div( {className:"modal-body"}, 
                            this.props.renderBody()
                        ),
                        React.DOM.div( {className:"modal-footer"}, 
                           this.props.renderFooterButtons()
                        )
                    )
                )
            )
            );
    }
});
