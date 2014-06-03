/**
* @jsx React.DOM
*/

var MetadataMetaEditorTab = React.createClass({

	renderFields: function() {
		var fields = [];
		for(var i=0; i<this.props.data.fields.length; i++) {
			fields.push(<TitleSpecificMetadataField 
						 data={this.props.data.fields[i]} 
						 index={i}
						 availableFieldTypes={this.props.availableFieldTypes}
						 deleteHandler={this.props.metadataFieldsHandlers.deleteHandler}
						 updateHandler={this.props.metadataFieldsHandlers.updateHandler}
						 />);
		}
		return fields;
	},
 
    render: function() {
       return (
       		<div>
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




