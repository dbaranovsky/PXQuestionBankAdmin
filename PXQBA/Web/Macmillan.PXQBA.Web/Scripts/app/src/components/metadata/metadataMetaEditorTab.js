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
               		<table className="table table">
               			<thead>
               			 	<tr>
               			 		<th> Field name </th>
               			 		<th> Internal name </th>
               			 		<th> Type </th>
               			 		<th> Values options</th>
               			 		<th> Display options</th>
               			 		<th> </th>
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




