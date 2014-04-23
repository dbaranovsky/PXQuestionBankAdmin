/**
* @jsx React.DOM
*/

var QuestionFilterItemsContainer = React.createClass({displayName: 'QuestionFilterItemsContainer',

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

        $.each(availableChoice, function(propery,  option){
            options.push({ value: propery,
                         text: option
                       });
        });

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