/**
* @jsx React.DOM
*/

var QuestionFilterItemsContainer = React.createClass({

   buildFiltersDescriptors: function() {
        var descriptors = [];
        for(var i=0; i<this.props.filter.length; i++) {
            for(var j=0; j<this.props.allAvailableColumns.length; j++) {
               if(this.props.filter[i].field==this.props.allAvailableColumns[j].metadataName) {
                     descriptors.push({
                             field: this.props.filter[i].field,
                             caption: this.props.allAvailableColumns[j].friendlyName,
                             allOptions: this.getAllOptions(this.props.allAvailableColumns[j].editorDescriptor.availableChoice, this.props.filter[i].values),
                             currentValues: this.props.filter[i].values
                     });
               }
           }
        }
      return descriptors;
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

            var index = -1;

            for(var j=0; j<options.length; j++) {
                 if(options[j].value==currentValuesArray[i]) {
                    index = j;
                    break;
                 }
            }
             
            if(index==-1) {
             options.push({ value: currentValuesArray[i],
                            text: currentValuesArray[i]
                          });
            }
 
        }
        
        return options;
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
        this.buildFiltersDescriptors();
        return (
            <div className="questionFilterContainer">
                 <div> 
                     {this.renderFilterItems()}
                </div>
            </div>
            );
        }
});