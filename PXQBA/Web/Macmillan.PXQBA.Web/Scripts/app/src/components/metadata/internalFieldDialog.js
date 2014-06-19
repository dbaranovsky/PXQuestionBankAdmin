/**
* @jsx React.DOM
*/

var InternalFieldDialog = React.createClass({

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
             return (<div>
                        <div>
                            <div> Internal name: </div> <TextEditor value={self.state.value} dataChangeHandler={self.onChangeHandler}/>
                        </div>
                         <div className="modal-footer clearfix">
                                 <button type="button" className="btn btn-default" data-dismiss="modal">Cancel</button>
                                 <button type="button" className="btn btn-primary" data-dismiss="modal" onClick={self.editInternalFieldHandler}>Edit</button>
                            </div>
                    </div>
            );
        };

        return (<ModalDialog showOnCreate={true}
                             renderHeaderText={renderHeaderText} 
                             renderBody={renderBody} 
                             closeDialogHandler = {this.props.closeDialogHandler}
                             dialogId="internalFieldDialog"/>);
    }
});




