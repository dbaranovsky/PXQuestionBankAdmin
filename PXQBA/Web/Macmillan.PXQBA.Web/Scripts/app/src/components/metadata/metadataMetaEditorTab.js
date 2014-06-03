/**
* @jsx React.DOM
*/

var MetadataMetaEditorTab = React.createClass({

	renderFields: function() {
		var fields = [];

		for(var i=0; i<this.props.data.fields.length; i++) {
			fields.push(<TitleSpecificMetadataField data={this.props.data.fields[i]} index={i}/>);
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

               			{this.renderFields()}
               		</table>
               </div>
               <div>
               	   <button type="button" className="btn btn-primary metadata-button"  onClick={this.props.metadataFieldsHandlers.addHandler} >Add field</button>
               </div>
            </div>
            );
    }
});




