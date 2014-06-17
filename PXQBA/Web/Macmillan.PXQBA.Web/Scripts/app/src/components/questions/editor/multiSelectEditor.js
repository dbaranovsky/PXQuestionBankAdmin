/**
* @jsx React.DOM
*/
var MultiSelectEditor = React.createClass({

 
    getInitialState: function(){
         var metadataValues = [];
         var currentValues = this.props.question[this.props.field];
     
         var  availableChoices =  this.props.metadataField.editorDescriptor.availableChoice;
         var options = [];

        for(var i=0; i<availableChoices.length; i++) {
           metadataValues.push({value: availableChoices[i].value, text: availableChoices[i].text});
        }
        
        if(currentValues !== undefined && currentValues != null && currentValues.length>0){
          for(var i=0; i<currentValues.length; i++) {
            metadataValues = this.appendIfNotExist(metadataValues, currentValues[i]);
          }
        }
      
         
        for(var i=0; i<metadataValues.length; i++) {
            options.push(<option value={metadataValues[i].value}>{metadataValues[i].text}</option>)
        }
                    
        return ({options: options});

    },

    appendIfNotExist: function(metadataValues, newValue) {
      var isExist = false;
      for(var i=0; i<metadataValues.length; i++) {
         if(metadataValues[i].value==newValue) {
            isExist=true;
         }
      }
      if(!isExist) {
         metadataValues.push({value: newValue, text: value})
      }

      return metadataValues;
    },


     editHandler: function(selectedOptions){
      
       var items = [];

       $.each(selectedOptions, function(i, option){
          items.push(option.value);
       });
      
      var question = this.props.question;
      if (question[this.props.field]== null || question[this.props.field].length !== items.length)
      {
        question[this.props.field] = items;
        this.props.editHandler(question);
      }
     

     },

    renderMenuItems: function() {

        return (this.state.options);
    },


   componentDidMount: function(){
        var selector = this.getDOMNode();
        var chosenOptions = {width: "100%", hide_dropdown: false, allow_add_new_values: this.props.canAddValues};
        var handler =  this.editHandler;
        $(selector).val(this.props.question[this.props.field])
                   .chosen(chosenOptions)
                   .change(function(e, params){
                             handler(e.currentTarget.selectedOptions);
                    });

                         
    },

    componentDidUpdate: function(){
       var selector = this.getDOMNode();
       $(selector).val(this.props.question[this.props.field]);
       $(selector).trigger("chosen:updated");
    },

   
    render: function() {
        return (
             <select data-placeholder="No Value" multiple disabled={this.props.isDisabled}>
                    {this.renderMenuItems()}  
             </select> 
         );
    }
});