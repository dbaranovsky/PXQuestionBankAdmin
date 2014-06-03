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

 	onBlurHandler: function() {
 		if((this.props.data.internalName==null)||
 		  (this.props.data.internalName=='')) {
 		  this.props.updateHandler(this.props.index, 
 		  						   "internalName",
 		  						    this.getInternalName(this.props.data.fieldName));
 		}
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
               			 			 dataChangeHandler={this.changeFieldNameHandler}
               			 			 onBlurHandler={this.onBlurHandler}
               				/> 
               		</td>
               		<td> {this.props.data.internalName}</td>
               		<td> 
               		   <SingleSelectSelector 
                        allOptions={this.props.availableFieldTypes}
                        dataPlaceholder="No Type"
                       onChangeHandler={this.changeTypeHandler}
                       currentValues = {this.getCurrentTypeValues()} />
               		</td>
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




