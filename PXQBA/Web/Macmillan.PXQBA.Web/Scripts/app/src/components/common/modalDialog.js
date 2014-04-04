/**
* @jsx React.DOM
*/

var ModalDialog = React.createClass({

    onClose: function(){
        if(typeof this.props.closeDialogHandler !== 'undefined'){
            this.props.closeDialogHandler();
        }

    },

    render: function() {
        return (
            <div className="modal fade" id={this.props.dialogId} tabIndex="-1" role="dialog" aria-labelledby="addQuestionModalLabel" aria-hidden="true" >
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <button type="button" className="close" data-dismiss="modal" aria-hidden="true" onClick={this.onClose}>&times;</button>
                            <h4 className="modal-title" id="myModalLabel">{this.props.renderHeaderText()}</h4>
                        </div>
                        <div className="modal-body" >
                            {this.props.renderBody()}
                        </div>
                       
                      {this.props.renderFooterButtons()}
                    
                    </div>
                </div>
            </div>
            );
    }
});
