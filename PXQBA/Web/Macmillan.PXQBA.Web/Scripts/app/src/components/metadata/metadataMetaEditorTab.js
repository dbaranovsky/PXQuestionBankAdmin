﻿/**
* @jsx React.DOM
*/

var MetadataMetaEditorTab = React.createClass({

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
			fields.push(<TitleSpecificMetadataField 
						 data={this.props.data.fields[i]} 
						 index={i}
						 availableFieldTypes={this.props.availableFieldTypes}
						 deleteHandler={this.props.metadataFieldsHandlers.deleteHandler}
						 updateHandler={this.props.metadataFieldsHandlers.updateHandler}
             showInternalFieldDialogHandler={this.showInternalFieldDialogHandler}
             showAvailibleValuesDialog={this.showAvailibleValuesDialog}
						 />);
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

  showAvailibleValuesDialog: function(index, value, fieldNameCaption) {
        this.setState({
        showAvailibleValuesDialog: true,
        indexRowForDialog: index,
        valueForDialog: value,
        fieldNameCaption: fieldNameCaption
      });
  },

	renderInternalFieldDialog: function() {
		if(this.state.showInternalFieldDialog) {
			return (<InternalFieldDialog closeDialogHandler={this.closeInternalFieldDialogHandler} 
                                  value={this.state.valueForDialog} 
                                  itemIndex={this.state.indexRowForDialog}
                                  updateHandler={this.props.metadataFieldsHandlers.updateHandler}
                                   />);
		}

		return null;
  },

  renderAvailibleValuesDialog: function() {
    if(this.state.showAvailibleValuesDialog) {
      return (<AvailibleValuesDialog closeDialogHandler={this.closeAvailibleValuesDialogHandler} 
                                  value={this.state.valueForDialog} 
                                  itemIndex={this.state.indexRowForDialog}
                                  updateHandler={this.props.metadataFieldsHandlers.updateHandler}
                                  fieldNameCaption={this.state.fieldNameCaption}
                                   />);
   }

    return null;
  },

  render: function() {
       return (
       		<div>
              <div>
               {this.renderInternalFieldDialog()}
               {this.renderAvailibleValuesDialog()}
             </div>
               <div> 
               		<table className="table table metadata-table">
               			<thead>
               			 	<tr>
               			 		<th className="field-column"> <span>Field name</span> </th>
               			 		<th className="internal-column"> <span>Internal name </span></th>
               			 		<th className="type-column"> <span> Type </span> <ToltipElement tooltipText="Type"/> </th>
               			 		<th className="values-column"> <span>Values options</span> 
                             <ToltipElement   tooltipText="Enter the values that will be available to question authors here. Text fields don't have pre-defined values"/>
                        </th>
               			 		<th className="display-column"> <span>Display options</span></th>
               			 		<th className="delete-column"> </th>
               			 	</tr>
               			</thead>
               		<tbody>
               			{this.renderFields()}
               		</tbody>
               		</table>
               </div>
               <div>
               	   <button type="button" className="btn btn-primary metadata-button"  onClick={this.props.metadataFieldsHandlers.addHandler} >Add field</button>
               </div>
            </div>
            );
    }
});




