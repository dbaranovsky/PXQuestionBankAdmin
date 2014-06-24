/**
* @jsx React.DOM
*/

var TitleSpecificMetadataField = React.createClass({

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

  buildDisabledClass: function(classText, isDisabled) {
    if(isDisabled) {
      classText+=" disabled"
    }
    return classText;
  },

  renderInternalNameButton: function() {
    return (<button type="button"
                    className={this.buildDisabledClass("btn btn-default btn-xs", !this.props.canEditTitleMetadataFull)} 
                    onClick={this.openInternalNameDialogHandler}>
            <span className="glyphicon glyphicon-pencil"></span>
            </button>);
  },

  renderAvailibleValuesButton: function() {
    var fieldType = this.props.data.fieldType;
     if((fieldType==window.enums.metadataFieldType.text)|| 
        (fieldType==window.enums.metadataFieldType.itemLink)|| 
        (fieldType==window.enums.metadataFieldType.multilineText)) {
      return null;
     }

     return (<button type="button" className="btn btn-default"  
                                   onClick={this.openAvailibleValuesDialog} >Values...</button>);
  },

  renderDisplayOptionsButton: function() {
     return (<button type="button" className={this.buildDisabledClass("btn btn-default", !this.props.canEditTitleMetadataReduced)} 
                                   onClick={this.openDisplayOptionsDialog} >Edit...</button>);
  },

  renderDeleteButton: function() {
     return (<button type="button" className={this.buildDisabledClass("btn btn-default btn-sm", !this.props.canEditTitleMetadataReduced)}
                     onClick={this.deleteHandler} 
                     data-toggle="tooltip"
                     title="Delete field">
                   <span className="glyphicon glyphicon-trash">
                   </span>
             </button>);
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
               <tr>  
               		<td>
               			 <TextEditor value={this.props.data.fieldName}
                           disabled={!this.props.canEditTitleMetadataReduced}
               			 			 dataChangeHandler={this.changeFieldNameHandler}
               			 			 onBlurHandler={this.onBlurFieldNameHandler}
               				/> 
               		</td>
               		<td> 
               		     <span> {this.props.data.internalName} </span>
               		     <span> {this.renderInternalNameButton()}</span>
               		</td>
               		<td> 
               		   <SingleSelectSelector 
                       disabled={!this.props.canEditTitleMetadataReduced}
                       allOptions={this.props.availableFieldTypes}
                       dataPlaceholder="No Type"
                       onChangeHandler={this.changeTypeHandler}
                       currentValues = {this.getCurrentTypeValues()} />
               		</td>
               		<td>{this.renderAvailibleValuesButton()}</td>
               		<td> {this.renderDisplayOptionsButton()}</td>
               		<td>   
                       {this.renderDeleteButton()}
               		</td>
                </tr>
            );
    }
});




