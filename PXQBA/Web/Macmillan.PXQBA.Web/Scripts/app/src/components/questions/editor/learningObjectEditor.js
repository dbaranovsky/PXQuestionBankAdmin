/**
* @jsx React.DOM
*/

var LearningObjectEditor = React.createClass({ 
    getInitialState: function(){
         var currentValues = this.props.question[this.props.field];
     
        var  availableChoices =  this.props.metadataField.editorDescriptor.availableChoice;
	      var options = [];


        for (var propertyName in availableChoices) {
            availableChoice = availableChoices[propertyName];
            options.push(<option value={propertyName}>{availableChoice}</option>);
        }

         return ({options: options});

    },

   
     editHandler: function(selectedOptions){
      
       
       var items = [];
       
       $.each(selectedOptions, function(i, option){
       	  var learningObject = { guid: option.value, description: option.text};
          items.push(learningObject);
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


    componentDidUpdate: function(){
      var guids = [];
      $.each(this.props.question[this.props.field], function(i, objective){
           guids.push(objective.guid);
       });
      
      var selector = this.getDOMNode();
      $(selector).val(guids);
      $(selector).trigger("chosen:updated");
    },

    componentDidMount: function(){
        var selector = this.getDOMNode();
        var chosenOptions = {width: "100%", hide_dropdown: false, allow_add_new_values: this.props.metadataField.canAddValues};

  		 var currentValues = [];

     
	    var guids = [];

	    $.each(this.props.question[this.props.field], function(i, objective){
           guids.push(objective.guid);
       });
        var handler =  this.editHandler;
        $(selector).val(guids)
                   .chosen(chosenOptions)
                   .change(function(e, params){
                             handler(e.currentTarget.selectedOptions);
                    });

                         
    },

   
    render: function() {
        return (
             <select data-placeholder="No Value" multiple disabled={this.props.isDisabled}>
                    {this.renderMenuItems()}  
             </select> 
         );
    }
});