/**
* @jsx React.DOM
*/

var QuestionFilterItemsContainer = React.createClass({

   buildFiltersDescriptors: function() {
      var descriptors = [];
      for(var i=0; i<this.props.filter.length; i++) {
            descriptors.push(this.buildFilterDescriptor(this.props.filter[i], this.props.allAvailableColumns))
      }
      return descriptors;
   },


   buildFilterDescriptor: function(filter, allAvailableColumns) {
        for(var i=0; i<allAvailableColumns.length; i++) {
            if(filter.field==allAvailableColumns[i].metadataName) {
                return { field: filter.field,
                         filterType: allAvailableColumns[i].filterType,
                         caption: allAvailableColumns[i].friendlyName,
                         allOptions: this.getAllOptions(allAvailableColumns[i].editorDescriptor.availableChoice, filter.values),
                         currentValues: filter.values
                };
            }
        }
        return null;
   },


   getAllOptions: function(availableChoice, currentValuesArray) {
        if(availableChoice==null) {
             availableChoice =[];
        }
        var options = [];

        for(var i=0; i<availableChoice.length; i++) {
           options.push({ value: availableChoice[i].value,
                          text: availableChoice[i].text
           });
        }

        for(var i=0; i<currentValuesArray.length; i++) {
            if (!this.isPresentInOptions(options, currentValuesArray[i])) {
             options.push({ value: currentValuesArray[i],
                            text: currentValuesArray[i]
                          });
            }
        }
        
        return options;
   },

   isPresentInOptions: function(options, newValue) {
       var isNew = true;
       $.each(options, function(index, option) {
            if(option.value==newValue) {
                isNew=false;
                return false
            }
       });

       return !isNew;
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
      return (<QuestionFilterItemBase descriptor={itemDescriptor}/>)
   },


   render: function() {
        return (
            <div className="questionFilterContainer">
                 <div> 
                     {this.renderFilterItems()}
                </div>
            </div>
            );
        }
});