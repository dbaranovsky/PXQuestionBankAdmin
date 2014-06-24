/**
* @jsx React.DOM
*/

var MetadataMetaEditorTab = React.createClass({displayName: 'MetadataMetaEditorTab',

	getInitialState: function() {
        return { 
          showInternalFieldDialog: false,
          showAvailibleValuesDialog: false,
          showDisplayOptionsDialog: false,
          showDisplayImageDialog: false,
          indexRowForDialog: -1,
          valueForDialog: "",
          imgUrl:"",
          fieldType: null
        };
    },


	renderFields: function() {
		var fields = [];
		for(var i=0; i<this.props.data.fields.length; i++) {
			fields.push(TitleSpecificMetadataField( 
						 {data:this.props.data.fields[i], 
             canEditMetadataValues:this.props.data.canEditMetadataValues,
             canEditTitleMetadataReduced:this.props.data.canEditTitleMetadataReduced,
             canEditTitleMetadataFull:this.props.data.canEditTitleMetadataFull,
						 index:i,
						 availableFieldTypes:this.props.availableFieldTypes,
						 deleteHandler:this.props.metadataFieldsHandlers.deleteHandler,
						 updateHandler:this.props.metadataFieldsHandlers.updateHandler,
             showInternalFieldDialogHandler:this.showInternalFieldDialogHandler,
             showAvailibleValuesDialog:this.showAvailibleValuesDialog,
             showDisplayOptionsDialog:this.showDisplayOptionsDialog}
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

  closeDisplayOptionsDialogHandler: function() {
    this.setState({showDisplayOptionsDialog: false});
  },

  closeDisplayImageDialogHandler: function() {
     this.setState({
        showDisplayImageDialog: false,
        showDisplayOptionsDialog: true
        });
  },

	showInternalFieldDialogHandler: function(index, value) {
		this.setState({
		   	showInternalFieldDialog: true,
        indexRowForDialog: index,
        valueForDialog: value
			});
	},

  showAvailibleValuesDialog: function(index, value, fieldNameCaption, fieldType) {
        this.setState({
          showAvailibleValuesDialog: true,
          indexRowForDialog: index,
          valueForDialog: value,
          fieldNameCaption: fieldNameCaption,
          fieldType: fieldType
      });
  },

  showDisplayOptionsDialog: function(index, value) {
        this.setState({
          showDisplayOptionsDialog: true,
          indexRowForDialog: index,
          valueForDialog: value,
      });
  },


  showDisplayImageDialog: function(imgUrl) {
    this.setState({
        showDisplayOptionsDialog: false,
        showDisplayImageDialog: true,
        imgUrl: imgUrl,
      });
  },

	renderInternalFieldDialog: function() {
		if(this.state.showInternalFieldDialog) {
			return (InternalFieldDialog( {closeDialogHandler:this.closeInternalFieldDialogHandler, 
                                  canEdit:this.pop,
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
                                  updateHandler:this.props.metadataFieldsHandlers.updateHandler,
                                  fieldNameCaption:this.state.fieldNameCaption,
                                  fieldType:this.state.fieldType,
                                  canEdit:this.props.data.canEditMetadataValues}
                                   ));
   }

    return null;
  },

  renderDisplayOptionsDialog: function() {
    if(this.state.showDisplayOptionsDialog) {
      return (DisplayOptionsDialog( {closeDialogHandler:this.closeDisplayOptionsDialogHandler, 
                                  value:this.state.valueForDialog, 
                                  itemIndex:this.state.indexRowForDialog,
                                  updateHandler:this.props.metadataFieldsHandlers.updateHandler,
                                  showDisplayImageDialogHandler:this.showDisplayImageDialog}
                                   ));
    }

    return null;
  },

  renderDisplayImageDialog: function() {
    if(this.state.showDisplayImageDialog) {
        return (DisplayImageDialog( {closeDialogHandler:this.closeDisplayImageDialogHandler, 
                                    title:"The fields you select will be displayed here:",
                                    imageUrl:this.state.imgUrl}
          ));
    }

    return null;
  },


  renderAddButton: function() {
    var classNameText="btn btn-primary metadata-button";

      if(!this.props.data.canEditTitleMetadataReduced) {
        classNameText+=" disabled";
     }

    return (React.DOM.button( {type:"button", className:classNameText,  onClick:this.props.metadataFieldsHandlers.addHandler} , "Add field"));
  },
 
  render: function() {
       return (
       		React.DOM.div(null, 
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
               	   this.renderAddButton()
               ),
               React.DOM.div( {className:"dialogs-container"}, 
                this.renderInternalFieldDialog(),
                this.renderAvailibleValuesDialog(),
                this.renderDisplayOptionsDialog(),
                this.renderDisplayImageDialog()
             )
            )
            );
    }
});




