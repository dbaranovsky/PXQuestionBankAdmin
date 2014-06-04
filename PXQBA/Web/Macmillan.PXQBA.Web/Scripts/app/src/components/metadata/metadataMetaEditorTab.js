/**
* @jsx React.DOM
*/

var MetadataMetaEditorTab = React.createClass({

	getInitialState: function() {
        return { showInternalFieldDialog: false,
                 indexInternalFieldDialog: -1,
                 valueInternalFieldDialog: ""
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
						 />);
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
			return (<InternalFieldDialog closeDialogHandler={this.closeInternalFieldDialogHandler} 
                                  value={this.state.valueInternalFieldDialog} 
                                  itemIndex={this.state.indexInternalFieldDialog}
                                  updateHandler={this.props.metadataFieldsHandlers.updateHandler}
                                   />);
		}

		return null;
  	},

    render: function() {
       return (
       		<div>
              <div>
               {this.renderInternalFieldDialog()}
             </div>
               <div> 
               		<table className="table table metadata-table">
               			<thead>
               			 	<tr>
               			 		<th className="field-column"> <span>Field name</span> </th>
               			 		<th className="internal-column"> <span>Internal name </span></th>
               			 		<th className="type-column"> <span> Type </span> <ToltipElement tooltipText="Type"/> </th>
               			 		<th className="values-column"> <span>Values options</span> <ToltipElement tooltipText="Values options"/></th>
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




