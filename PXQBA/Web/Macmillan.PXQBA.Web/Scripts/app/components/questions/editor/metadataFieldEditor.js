/**
* @jsx React.DOM
*/
var MetadataFieldEditor = React.createClass({displayName: 'MetadataFieldEditor',

     editHandler: function(selectedOptions){
      
       
       var text = "";
       if (selectedOptions[0] !== undefined){
            text = selectedOptions[0].text;
            var value = selectedOptions[0].value;
            //Checking if value is text or int. Ugly code! Move the sign to the state. 
            if (text.toLowerCase()!= value.toLowerCase())
            {
              text = value;
            }
       } 
       else {
            
            text = this.refs.editor.getDOMNode().value;
       }

      var question = this.props.question;
      if (question[this.props.field] !== text)
      {
        question[this.props.field] = text;
        this.props.editHandler(question);
      }

     },

    renderMenuItems: function(availableChoices) {
        var items = [];
        if (this.props.allowDeselect){
            items.push(React.DOM.option( {value:""}));
        }
        for (var propertyName in availableChoices) {
            availableChoice = availableChoices[propertyName];
            items.push(this.renderMenuItem(availableChoice, propertyName));
        }
        return items;
    },

    renderMenuItem: function(label, value) {
        if(label.toLowerCase()== value.toLowerCase()){
          return (React.DOM.option( {value:label}, label));
        }
        return (React.DOM.option( {value:value}, label));
    },

     renderBody: function(){


       var field = this.props.field;
       var metadataField = $.grep(this.props.metadata, function(e){ return $.inArray(e.metadataName, [field, "dlap_q_"+field, "dlap_"+field, field.toLowerCase()])!=-1;  });
       var editorType = metadataField.length>0 ? metadataField[0].editorDescriptor.editorType : 0;
       var currentValue = this.props.question[this.props.field];
       switch (editorType) {
          case window.enums.editorType.singleSelect:
             return (React.DOM.select( {ref:"editor", className:"single-selector", value:currentValue},  " ", this.renderMenuItems(metadataField[0].editorDescriptor.availableChoice), " " ) );

          case window.enums.editorType.multiSelect:
              if(field=="learningObjectives"){
                return (LearningObjectEditor( {values:currentValue, metadataField:metadataField[0], question:this.props.question, field:this.props.field, editHandler:this.props.editHandler} ));
              }
             return (MultiSelectEditor( {values:currentValue, metadataField:metadataField[0], question:this.props.question, field:this.props.field, editHandler:this.props.editHandler} ));

          default: 
            if(metadataField[0]!== undefined && metadataField[0].isMultiline){
                 return ( React.DOM.textarea( {onChange:this.editHandler,  ref:"editor", className:"question-body-editor",  rows:"10", type:"text", placeholder:"Enter text...", value:currentValue} ));  
             }
           
             return (React.DOM.input( {type:"text", onChange:this.editHandler, ref:"editor", value:currentValue}));
        }
    },

    componentDidUpdate: function(){
    var self = this;
    var chosenOptions = {width: "100%"};
    if (self.props.allowDeselect){
        chosenOptions.allow_single_deselect = true;
        chosenOptions.placeholder_text_single = "No Value";
    }

      $(self.getDOMNode()).find('.single-selector')
                           .chosen(chosenOptions)
                           .change(function(e, params){
                              self.editHandler(e.currentTarget.selectedOptions);
                           });
    },

    componentDidMount: function(){
       var self = this;
      if (!this.props.setDefault){
        return;
      }
      var field = this.props.field;
      var metadataField = $.grep(this.props.metadata, function(e){ return $.inArray(e.metadataName, [field, "dlap_q_"+field, "dlap_"+field, field.toLowerCase()])!=-1; });
      var question = this.props.question;
      var availableChoices = metadataField[0].editorDescriptor.availableChoice;
        for (var propertyName in availableChoices) {
            availableChoice = availableChoices[propertyName];
            question[this.props.field] = (availableChoice.toLowerCase() == propertyName.toLowerCase())? availableChoice: propertyName;
            break;
        }
       this.props.editHandler(question);
      
    },


    render: function() {
        return (

            React.DOM.div( {className:"metadata-field-editor"}, 
                   React.DOM.label(null, this.props.title === undefined ? this.props.field : this.props.title),
                   React.DOM.br(null ),
                    this.renderBody(),
                   React.DOM.br(null )
                          
            ) 
         );
    }

});