/**
* @jsx React.DOM
*/

var MetadataMetaEditorTab = React.createClass({displayName: 'MetadataMetaEditorTab',

	getInitialState: function() {
        return { 
          showInternalFieldDialog: false,
          showAvailibleValuesDialog: false,
          indexRowForDialog: -1,
          valueForDialog: "",
        };
    },


	renderFields: function() {
		var fields = [];
		for(var i=0; i<this.props.data.fields.length; i++) {
			fields.push(TitleSpecificMetadataField( 
						 {data:this.props.data.fields[i], 
						 index:i,
						 availableFieldTypes:this.props.availableFieldTypes,
						 deleteHandler:this.props.metadataFieldsHandlers.deleteHandler,
						 updateHandler:this.props.metadataFieldsHandlers.updateHandler,
             showInternalFieldDialogHandler:this.showInternalFieldDialogHandler,
             showAvailibleValuesDialog:this.showAvailibleValuesDialog}
						 ));
		}
		return fields;
	},
 	
  closeInternalFieldDialogHandler: function() {
     this.setState({showInternalFieldDialog: false});
  },

  closeAvailibleValuesDialogHandler: function() {
     this.setState({showAvailibleValuesDialog: false});
  },


	showInternalFieldDialogHandler: function(index, value) {
		this.setState({
		   	showInternalFieldDialog: true,
        indexRowForDialog: index,
        valueForDialog: value
			});
	},

  showAvailibleValuesDialog: function(index, value) {
        this.setState({
        showAvailibleValuesDialog: true,
        indexRowForDialog: index,
        valueForDialog: value
      });
  },

	renderInternalFieldDialog: function() {
		if(this.state.showInternalFieldDialog) {
			return (InternalFieldDialog( {closeDialogHandler:this.closeInternalFieldDialogHandler, 
                                  value:this.state.valueForDialog, 
                                  itemIndex:this.state.indexRowForDialog,
                                  updateHandler:this.props.metadataFieldsHandlers.updateHandler}
                                   ));
		}

		return null;
  },

  renderAvailibleValuesDialog: function() {
    if(this.state.showAvailibleValuesDialog) {
      return (AvailibleValuesDialog( {closeDialogHandler:this.closeAvailibleValuesDialogHandler, 
                                  value:this.state.valueForDialog, 
                                  itemIndex:this.state.indexRowForDialog,
                                  updateHandler:this.props.metadataFieldsHandlers.updateHandler}
                                   ));
    }

    return null;
  },

  render: function() {
       return (
       		React.DOM.div(null, 
              React.DOM.div(null, 
               this.renderInternalFieldDialog(),
               this.renderAvailibleValuesDialog()
             ),
               React.DOM.div(null,  
               		React.DOM.table( {className:"table table metadata-table"}, 
               			React.DOM.thead(null, 
               			 	React.DOM.tr(null, 
               			 		React.DOM.th( {className:"field-column"},  " ", React.DOM.span(null, "Field name"), " " ),
               			 		React.DOM.th( {className:"internal-column"},  " ", React.DOM.span(null, "Internal name " )),
               			 		React.DOM.th( {className:"type-column"},  " ", React.DOM.span(null,  " Type " ), " ", ToltipElement( {tooltipText:"Type"}), " " ),
               			 		React.DOM.th( {className:"values-column"},  " ", React.DOM.span(null, "Values options"), 
                             ToltipElement(   {tooltipText:"Enter the values that will be available to question authors here. Text fields don't have pre-defined values"})
                        ),
               			 		React.DOM.th( {className:"display-column"},  " ", React.DOM.span(null, "Display options")),
               			 		React.DOM.th( {className:"delete-column"},  " " )
               			 	)
               			),
               		React.DOM.tbody(null, 
               			this.renderFields()
               		)
               		)
               ),
               React.DOM.div(null, 
               	   React.DOM.button( {type:"button", className:"btn btn-primary metadata-button",  onClick:this.props.metadataFieldsHandlers.addHandler} , "Add field")
               )
            )
            );
    }
});




