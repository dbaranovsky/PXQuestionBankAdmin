/**
* @jsx React.DOM
*/

var MetadataMetaEditorTab = React.createClass({displayName: 'MetadataMetaEditorTab',

	getInitialState: function() {
        return { showInternalFieldDialog: false,
                 indexInternalFieldDialog: -1,
                 valueInternalFieldDialog: ""
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
             showInternalFieldDialogHandler:this.showInternalFieldDialogHandler}
						 ));
		}
		return fields;
	},
 	
  closeInternalFieldDialogHandler: function() {
     this.setState({showInternalFieldDialog: false});
  },

	showInternalFieldDialogHandler: function(index, value) {
		this.setState({
		   	showInternalFieldDialog: true,
        indexInternalFieldDialog: index,
        valueInternalFieldDialog: value
			});
	},

	renderInternalFieldDialog: function() {
		if(this.state.showInternalFieldDialog) {
			return (InternalFieldDialog( {closeDialogHandler:this.closeInternalFieldDialogHandler, 
                                  value:this.state.valueInternalFieldDialog, 
                                  itemIndex:this.state.indexInternalFieldDialog,
                                  updateHandler:this.props.metadataFieldsHandlers.updateHandler}
                                   ));
		}

		return null;
  	},

    render: function() {
       return (
       		React.DOM.div(null, 
              React.DOM.div(null, 
               this.renderInternalFieldDialog()
             ),
               React.DOM.div(null,  
               		React.DOM.table( {className:"table table"}, 
               			React.DOM.thead(null, 
               			 	React.DOM.tr(null, 
               			 		React.DOM.th(null,  " Field name " ),
               			 		React.DOM.th(null,  " Internal name " ),
               			 		React.DOM.th(null,  " Type " ),
               			 		React.DOM.th(null,  " Values options"),
               			 		React.DOM.th(null,  " Display options"),
               			 		React.DOM.th(null,  " " )
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




