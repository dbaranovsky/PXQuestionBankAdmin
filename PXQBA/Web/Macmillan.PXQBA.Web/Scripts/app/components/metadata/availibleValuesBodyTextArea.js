/**
* @jsx React.DOM
*/


var AvailibleValuesBodyTextArea = React.createClass({displayName: 'AvailibleValuesBodyTextArea',

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
            return (React.DOM.div(null, 
                "List pre-defined values for ", React.DOM.b(null, this.props.fieldNameCaption), " below, one per line. Editors"+' '+ 
                "will be able to define additional values while editing questions. Values will"+' '+
                "appear to editors and instructors in alphabetical order."
               ));
        }

        return (React.DOM.div(null, 
                "List all possible values for ", React.DOM.b(null, this.props.fieldNameCaption), " below, one per line. Values"+' '+
                "will appear to editors and instructors in the order listed."
               ));
    },

    render: function() {
        return(
            React.DOM.div(null, 
                React.DOM.div(null, 
                    React.DOM.div( {className:"metadata-values-label"},  
                        this.renderTextDescription()
                     ), 
                     React.DOM.div(null,  
                            TextAreaEditor( 
                              {disabled:!this.props.canEdit,
                              classNameProps:"metadata-availible-values-editor",
                              dataChangeHandler:this.onChangeHandler, 
                              value:this.state.value} )
                     )
                ),
                React.DOM.div( {className:"modal-footer clearfix"}, 
                                 React.DOM.button( {type:"button", className:"btn btn-default", 'data-dismiss':"modal"}, "Cancel"),
                                 React.DOM.button( {type:"button", className:"btn btn-primary", 'data-dismiss':"modal", onClick:this.editAvailibleValuesHandler}, "Save")
                )
            )
            );
    },

});