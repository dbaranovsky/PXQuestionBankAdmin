/**
* @jsx React.DOM
*/

var TitleSpecificMetadataField = React.createClass({displayName: 'TitleSpecificMetadataField',

	deleteHandler: function() {
		this.props.deleteHandler(this.props.index)
	},

 	changeTypeHandler: function(items) {
      var value = items[0];
      this.props.updateHandler(this.props.index, "fieldType", value);
 	},

 	changeFieldNameHandler: function(text) {
 		this.props.updateHandler(this.props.index, "fieldName", text);
 	},

 	onBlurFieldNameHandler: function() {
 		if((this.props.data.internalName==null)||
 		  (this.props.data.internalName=='')) {
 		  this.props.updateHandler(this.props.index, 
 		  						   "internalName",
 		  						    this.getInternalName(this.props.data.fieldName));
 		}
 	},

 	openInternalNameDialogHandler: function() {
 		this.props.showInternalFieldDialogHandler(this.props.index, this.props.data.internalName);
 	},

  openAvailibleValuesDialog: function() {
    this.props.showAvailibleValuesDialog(this.props.index, this.props.data.valuesOptions, this.props.data.fieldName, this.props.data.fieldType);
  },

  openDisplayOptionsDialog: function() {
    this.props.showDisplayOptionsDialog(this.props.index, this.props.data.displayOptions);
  },

  renderAvailibleValuesButton: function() {
    var fieldType = this.props.data.fieldType;
     if((fieldType==window.enums.metadataFieldType.text)|| 
        (fieldType==window.enums.metadataFieldType.itemLink)|| 
        (fieldType==window.enums.metadataFieldType.multilineText)) {
      return null;
     }
     return (React.DOM.button( {type:"button", className:"btn btn-default",  onClick:this.openAvailibleValuesDialog} , "Values..."));
  },

  renderDisplayOptionsButton: function() {
     return (React.DOM.button( {type:"button", className:"btn btn-default",  onClick:this.openDisplayOptionsDialog} , "Edit..."));
  },

 	getInternalName: function(name) {
    var internalName = name.toLowerCase();
    internalName = internalName.replace(/\s+/g, '');
 		return internalName;
 	},

 	getCurrentTypeValues: function() {
 		if(this.props.data.fieldType!=null) {
 			return [this.props.data.fieldType];
 		}
 		return [];
 	},

    render: function() {
       return (
               React.DOM.tr(null,   
               		React.DOM.td(null, 
               			 TextEditor( {value:this.props.data.fieldName,
               			 			 dataChangeHandler:this.changeFieldNameHandler,
               			 			 onBlurHandler:this.onBlurFieldNameHandler}
               				) 
               		),
               		React.DOM.td(null,  
               		     React.DOM.span(null,  " ", this.props.data.internalName, " " ),
               		     React.DOM.span(null,  " ", React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.openInternalNameDialogHandler}, React.DOM.span( {className:"glyphicon glyphicon-pencil"})), " " )
               		),
               		React.DOM.td(null,  
               		   SingleSelectSelector( 
                        {allOptions:this.props.availableFieldTypes,
                        dataPlaceholder:"No Type",
                       onChangeHandler:this.changeTypeHandler,
                       currentValues:  this.getCurrentTypeValues()} )
               		),
               		React.DOM.td(null, this.renderAvailibleValuesButton()),
               		React.DOM.td(null,  " ", this.renderDisplayOptionsButton()),
               		React.DOM.td(null,    
               			 React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.deleteHandler, 
               		 				 'data-toggle':"tooltip",
               		 				  title:"Delete field"}, 
               		 				  React.DOM.span( {className:"glyphicon glyphicon-trash"}
               		 				  )
               		 				)
               		)
                )
            );
    }
});




