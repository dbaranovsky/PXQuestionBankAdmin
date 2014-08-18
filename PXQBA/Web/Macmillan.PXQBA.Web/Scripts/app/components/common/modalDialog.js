/**
* @jsx React.DOM
*/

var ModalDialog = React.createClass({displayName: 'ModalDialog',


    componentDidMount: function(){
         if(typeof this.props.closeDialogHandler !== 'undefined'){
            var self = this;
            $(this.getDOMNode()).on('hidden.bs.modal', function (e) {
                self.props.closeDialogHandler();
            });
        }
       

        if (this.props.showOnCreate){
            $(this.getDOMNode()).modal('show');
        }

        if(this.props.setInnerHtml){
            $(this.getDOMNode()).find(".modal-body").html(this.props.innerHtml);
        }
    },

    closeDialog: function(){
         if(this.props.preventDefaultClose){
             this.props.closeDialogHandler();
         } else {
             $(this.getDOMNode()).modal('hide'); 
         }
    },

    render: function() {
        return (
            React.DOM.div({className: "modal fade", id: this.props.dialogId, tabIndex: "-1", role: "dialog", 'data-backdrop': "static", 'aria-labelledby': "addQuestionModalLabel", 'aria-hidden': "true"}, 
                React.DOM.div({className: "modal-dialog"}, 
                    React.DOM.div({className: "modal-content"}, 
                        React.DOM.div({className: "modal-header"}, 
                            React.DOM.button({type: "button", className: "close", onClick: this.closeDialog}, "×"), 
                            React.DOM.h4({className: "modal-title", id: "myModalLabel"}, this.props.renderHeaderText())
                        ), 
                        React.DOM.div({className: "modal-body"}, 
                            this.props.setInnerHtml? "" :this.props.renderBody()
                        ), 
                       
                      (this.props.renderFooterButtons !== undefined) ? this.props.renderFooterButtons() :""
                    
                    )
                )
            )
            );
    }
});
