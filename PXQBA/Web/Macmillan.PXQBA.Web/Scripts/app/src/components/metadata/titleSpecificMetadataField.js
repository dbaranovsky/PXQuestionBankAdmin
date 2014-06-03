/**
* @jsx React.DOM
*/

var TitleSpecificMetadataField = React.createClass({

 	deleteHandler: function() {
 		alert('deleted');
 	},

    render: function() {
       return (
               <tr>  
               		<td> {this.props.data.fieldName}</td>
               		<td> {this.props.data.internalName}</td>
               		<td> {this.props.data.fieldType}</td>
               		<td> ...</td>
               		<td> ...</td>
               		<td>   
               			 <button type="button" className="btn btn-default btn-sm" onClick={this.deleteHandler} 
               		 				 data-toggle="tooltip"
               		 				  title="Delete field">
               		 				  <span className="glyphicon glyphicon-trash">
               		 				  </span>
               		 				</button>
               		</td>
                </tr>
            );
    }
});




