/**
* @jsx React.DOM
*/

var QuestionFilterItemsContainer = React.createClass({displayName: 'QuestionFilterItemsContainer',

   buildFiltersDescriptors: function() {
   	var descriptors = [];

   	for(var i=0; i<this.props.filter.length; i++) {
   		for(var j=0; j<this.props.allAvailableColumns.length; j++) {
   			if(this.props.filter[i].field==this.props.allAvailableColumns[j].metadataName) {
   				descriptors.push({
   							 field: this.props.filter[i].field,
   							 caption: this.props.allAvailableColumns[j].friendlyName,
   							 allValues: this.getAllValues(this.props.allAvailableColumns[j].editorDescriptor.availableChoice, this.props.filter[i].values),
   							 currentValues: this.props.filter[i].values
   						 });
   			}
   		}
   	}
 	return descriptors;
   },

   getAllValues: function(availableChoice, currentValuesArray) {
   	var values = [];

    $.each(availableChoice, function(propery, option){
          values.push(option);
    });

   	for(var i=0; i<currentValuesArray.length; i++) {
   		var index = $.inArray(currentValuesArray[i], values);
   		if(index==-1) {
   			values.push(currentValuesArray[i]);
   		}
   	}
   	return values;
   },

   renderFilterItems: function() {
   		var descriptors = this.buildFiltersDescriptors();

   		var items = [];
   		for(var i=0; i<descriptors.length; i++) {
   			items.push(this.renderFilterItem(descriptors[i]))
   		}

   		return items;
   },

   renderFilterItem: function(itemDescriptor) {
   	  return (QuestionFilterItemBase( {descriptor:itemDescriptor}))
   },


   render: function() {
    	this.buildFiltersDescriptors();
        return (
            React.DOM.div( {className:"questionFilterContainer"}, 
                 React.DOM.div(null,  
                     this.renderFilterItems()
                )
            )
            );
        }
});