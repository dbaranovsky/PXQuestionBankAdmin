/**
* @jsx React.DOM
*/

var DisplayImageDialog = React.createClass({displayName: 'DisplayImageDialog',

    render: function() {
 
        var self = this;
        var renderHeaderText = function() {
             return self.props.title;
        };
      
        var renderBody = function(){
             return (React.DOM.div(null, 
                        React.DOM.div(null, 
                            React.DOM.div(null, React.DOM.img( {src:self.props.imageUrl, width:"950px"} ))  
                        ),
                         React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal", onClick:self.props.closeDialogHandler}, "Close")
                            )
                    )
            );
        };

        return (ModalDialog( {showOnCreate:true,
                             renderHeaderText:renderHeaderText, 
                             renderBody:renderBody, 
                             closeDialogHandler:  this.props.closeDialogHandler,
                             dialogId:"displayImageDialog"}));
    }
});