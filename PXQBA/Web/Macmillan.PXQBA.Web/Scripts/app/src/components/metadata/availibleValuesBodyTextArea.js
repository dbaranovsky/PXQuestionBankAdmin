﻿/**
* @jsx React.DOM
*/


var AvailibleValuesBodyTextArea = React.createClass({

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


    renderTextDescription: function() {
        if(this.props.fieldType==window.enums.metadataFieldType.keywords) {
            return (<div>
                List pre-defined values for <b>{this.props.fieldNameCaption}</b> below, one per line. Editors 
                will be able to define additional values while editing questions. Values will
                appear to editors and instructors in alphabetical order.
               </div>);
        }

        return (<div>
                List all possible values for <b>{this.props.fieldNameCaption}</b> below, one per line. Values
                will appear to editors and instructors in the order listed.
               </div>);
    },

    render: function() {
        return(
            <div>
                <div>
                    <div className="metadata-values-label"> 
                        {this.renderTextDescription()}
                     </div> 
                     <div> 
                            <TextAreaEditor 
                              disabled={!this.props.canEdit}
                              classNameProps="metadata-availible-values-editor"
                              dataChangeHandler={this.onChangeHandler} 
                              value={this.state.value} />
                     </div>
                </div>
                <div className="modal-footer clearfix">
                                 <button type="button" className="btn btn-default" data-dismiss="modal">Cancel</button>
                                 <button type="button" className="btn btn-primary" data-dismiss="modal" onClick={this.editAvailibleValuesHandler}>Save</button>
                </div>
            </div>
            );
    },

});