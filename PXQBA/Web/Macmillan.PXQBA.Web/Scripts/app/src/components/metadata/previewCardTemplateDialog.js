/**
* @jsx React.DOM
*/

var PreviewCardTemplateDialog = React.createClass({


    render: function() {
 
        var self = this;
        var renderHeaderText = function() {
             return "Question card preview";
        };
      
        var renderBody = function(){
             return (<div>
                        <div>
                            <div> 
                                <div className="question-card-template" dangerouslySetInnerHTML={{__html: self.props.cardHtml}} />
                             </div>  
                        </div>
                         <div className="modal-footer clearfix">
                                 <button type="button" className="btn btn-default" data-dismiss="modal" onClick={self.props.closeDialogHandler}>Close</button>
                            </div>
                    </div>
            );
        };

        return (<ModalDialog showOnCreate={true}
                             renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             closeDialogHandler = {this.props.closeDialogHandler}
                             dialogId="questionCardPreview"/>);
    }
});




