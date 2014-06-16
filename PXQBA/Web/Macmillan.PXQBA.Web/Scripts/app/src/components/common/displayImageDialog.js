/**
* @jsx React.DOM
*/

var DisplayImageDialog = React.createClass({

    render: function() {
 
        var self = this;
        var renderHeaderText = function() {
             return self.props.title;
        };
      
        var renderBody = function(){
             return (<div>
                        <div>
                            <div><img src={self.props.imageUrl} width={"950px"} /></div>  
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
                             dialogId="displayImageDialog"/>);
    }
});