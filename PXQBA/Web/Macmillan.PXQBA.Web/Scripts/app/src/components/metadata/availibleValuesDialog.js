/**
* @jsx React.DOM
*/

var AvailibleValuesDialog = React.createClass({

    getInitialState: function() {

        var text = "";

        if(this.props.value!=null) {
            for(var i=0; i<this.props.value.length; i++) {
                text+=this.props.value[i].value;
                if(i!=this.props.value.length-1) {
                    text+='\n';
                }
            }
        }
        return { value: text };
    },

    editAvailibleValuesHandler: function() {
       var options = [];
       var strings = this.state.value.split("\n");

       for(var i=0; i<strings.length; i++) {
         if(strings[i]!="") {
            options.push({text: strings[i], value: strings[i]})
         }
       }

       this.props.updateHandler(this.props.itemIndex, "valuesOptions",  options)
    },

    onChangeHandler: function(text) {
        this.setState( {value: text} );
    },

    render: function() {
 
        var self = this;
        var renderHeaderText = function() {
             return "Available values";
        };
      
        var renderBody = function(){
             return (<div>
                        <div>
                            <div className="metadata-values-label"> 
                                List all possible values for <b>{self.props.fieldNameCaption}</b>, one per line. values
                                will appear to editors and instructors in the order listed.
                            </div> 
                            <div> 
                               <TextAreaEditor 
                              classNameProps="metadata-availible-values-editor"
                              dataChangeHandler={self.onChangeHandler} 
                              value={self.state.value} />
                             </div>
                        </div>
                         <div className="modal-footer clearfix">
                                 <button type="button" className="btn btn-default" data-dismiss="modal" onClick={self.props.closeDialogHandler}>Cancel</button>
                                 <button type="button" className="btn btn-primary" data-dismiss="modal" onClick={self.editAvailibleValuesHandler}>Save</button>
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




