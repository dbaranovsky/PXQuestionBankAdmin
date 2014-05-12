/**
* @jsx React.DOM
*/
var MetadataFieldEditor = React.createClass({displayName: 'MetadataFieldEditor',

     getInitialState: function() {

      var metadataField = this.getMetaField();
       var allowDeselect = metadataField != null ? metadataField.allowDeselect : false;
      return { editMode: this.props.editMode === undefined? true : this.props.editMode,
               editMenu: false,
               allowDeselect: allowDeselect,
               metadataField:  metadataField};
    },
     editHandler: function(selectedOptions){
        var text = "";
         if (selectedOptions[0] !== undefined){
              text = selectedOptions[0].text;
              var value = selectedOptions[0].value;
              //Checking if value is text or int. Ugly code! Move the sign to the state. 
              if (text.toLowerCase()!= value.toLowerCase()){
                text = value;
              }
         } 
         else {
              
              text = this.refs.editor.getDOMNode().value;
         }

         this.updateQuestion(text);
       
     },

     getMetaField: function(){
       var field = this.props.field;
       var metadataField = $.grep(this.props.metadata, function(e){ return $.inArray(e.metadataName, [field, "dlap_q_"+field, "dlap_"+field, field.toLowerCase()])!=-1;  });
       
       return metadataField.length>0 ? metadataField[0]: null;
     },

    updateQuestion: function(value){
         var question = this.props.question;
        if (question[this.props.field] !== value)
        {
          question[this.props.field] = value;
          this.props.editHandler(question);
        }
       
    },


    renderMenuItems: function(availableChoices) {
        var items = [];
        if (this.state.allowDeselect){
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
       var metadataField = this.props.reload? this.getMetaField() : this.state.metadataField;
       var editorType = metadataField != null ? metadataField.editorDescriptor.editorType : 0;
       var currentValue = this.props.question[this.props.field];

       if(this.state.editMode){
       switch (editorType) {
          case window.enums.editorType.singleSelect:
             return (React.DOM.select( {ref:"editor", className:"single-selector", disabled:this.props.isDisabled, value:currentValue},  " ", this.renderMenuItems(metadataField.editorDescriptor.availableChoice), " " ) );

          case window.enums.editorType.multiSelect:
              if(field=="learningObjectives"){
                return (LearningObjectEditor( {values:currentValue, isDisabled:this.props.isDisabled, metadataField:metadataField, question:this.props.question, field:this.props.field, editHandler:this.props.editHandler} ));
              }
             return (MultiSelectEditor( {values:currentValue, isDisabled:this.props.isDisabled, metadataField:metadataField, question:this.props.question, field:this.props.field, editHandler:this.props.editHandler} ));

          default: 
          currentValue = currentValue || "";
            if(metadataField!= null && metadataField.isMultiline){
                 return ( React.DOM.textarea( {onChange:this.editHandler, disabled:this.props.isDisabled,  ref:"editor", className:this.props.isDisabled? "disabled" : "",  rows:"10", type:"text", placeholder:"Enter text...", value:currentValue} ));  
             }
           
             return (React.DOM.input( {type:"text", onChange:this.editHandler, disabled:this.props.isDisabled, className:this.props.isDisabled? "disabled" : "", ref:"editor", value:currentValue}));
        }
      }

      var values = [];

       switch (editorType) {
          case window.enums.editorType.singleSelect:
              var singleSelectValue = metadataField.editorDescriptor.availableChoice[currentValue];
              if(currentValue!= ''){  
              values.push(React.DOM.div( {className:"current-values-view"},  " ", singleSelectValue === undefined? currentValue : singleSelectValue, 
                                React.DOM.span( {className:"glyphicon glyphicon-pencil btn custom-btn",  'data-toggle':"tooltip", title:"Edit", onClick:this.switchEditMode})
                           ));
            }
              break;
          case window.enums.editorType.multiSelect:
                
                  if(field=="learningObjectives"){
                     $.each(currentValue, function(i, value){
                       values.push(React.DOM.div( {className:"current-values-view learning-objectives label label-warning"},  " ", value.description, " " ));
                     });
                  }else{
                     $.each(currentValue, function(i, value){
                        values.push(React.DOM.div( {className:"current-values-view label label-warning"}, value));
                     });
                  }

                   if (values.length == 0){
                      values.push(React.DOM.div( {className:"current-values-view"},  " No value", 
                                    React.DOM.span( {className:"glyphicon glyphicon-pencil btn custom-btn",  'data-toggle':"tooltip", title:"Edit", onClick:this.switchEditMode})
                                  ));
                   } else{
                         values.push(React.DOM.div( {className:"current-values-view"},  " ", React.DOM.span( {className:"glyphicon glyphicon-pencil btn custom-btn",  'data-toggle':"tooltip", title:"Edit", onClick:this.switchEditMode}), " " ));
                   }

                 break;               
          default: 
            if (currentValue != null && currentValue !=''){           
              values.push(React.DOM.div( {className:"current-values-view"},  " ", currentValue, 
                               React.DOM.span( {className:"glyphicon glyphicon-pencil btn custom-btn",  'data-toggle':"tooltip", title:"Edit", onClick:this.switchEditMode})
                          ));
            }
        }

      if (values.length == 0){
         values.push(React.DOM.div( {className:"current-values-view"},  " No value",  
                          React.DOM.span( {className:"glyphicon glyphicon-pencil btn custom-btn",  'data-toggle':"tooltip", title:"Edit", onClick:this.switchEditMode})
                     ));
      }   

      return values;

    },

    switchEditMode: function(){

      $(this.getDOMNode()).find('div:not([data-reactid])').remove();
      this.setState({editMode: !this.state.editMode, editMenu: !this.state.editMenu, currentValue: this.props.question[this.props.field]});
    },

    componentDidUpdate: function(){
    var self = this;
    var chosenOptions = {width: "100%"};
    if (self.state.allowDeselect){
        chosenOptions.allow_single_deselect = true;
        chosenOptions.placeholder_text_single = "No Value";
    }

  

      $(self.getDOMNode()).find('.single-selector')
                           .chosen(chosenOptions)
                           .change(function(e, params){
                              self.editHandler(e.currentTarget.selectedOptions);
                           });
       $(self.getDOMNode()).find('.single-selector').trigger("chosen:updated");
    },

    componentDidMount: function(){
       var self = this;
       this.componentDidUpdate();
      if (!this.props.setDefault){
        return;
      }
     this.resetDefaults();
      
    },

    resetDefaults: function(){
      var field = this.props.field;
      var metadataField = this.state.metadataField;
      var question = this.props.question;
      var availableChoices = metadataField.editorDescriptor.availableChoice;
        for (var propertyName in availableChoices) {
            availableChoice = availableChoices[propertyName];
            question[this.props.field] = (availableChoice.toLowerCase() == propertyName.toLowerCase())? availableChoice: propertyName;
            break;
        }
       this.props.editHandler(question);
    },

    applyChanges: function(){
       this.saveMetafieldValue();
      this.props.applyHandler();
      this.switchEditMode();
    },

    declineChanges: function(){
      this.updateQuestion(this.state.currentValue);
      this.switchEditMode();
    },

    saveMetafieldValue: function(){
      questionDataManager.saveQuestionData(this.props.question.id,
                                           this.state.metadataField== null? this.props.field : this.state.metadataField.metadataName, 
                                           this.props.question[this.props.field]);
    },

    renderMenu: function(){
        if (this.state.editMenu){
          return( React.DOM.span( {className:"input-group-btn"}, 
                                React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.applyChanges, 'data-toggle':"tooltip", title:"Apply"}, React.DOM.span( {className:"glyphicon glyphicon-ok"})), 
                                React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.declineChanges, 'data-toggle':"tooltip", title:"Cancel"}, React.DOM.span( {className:"glyphicon glyphicon-remove"})) 
                   )   );
        }

        return null;
    },


    render: function() {
        return (

            React.DOM.div( {className:"metadata-field-editor"}, 
                   React.DOM.label(null, this.props.title === undefined? this.props.field : this.props.title),
                   React.DOM.br(null ),
                    this.renderBody(),
                   React.DOM.br(null ),
                   this.renderMenu()
                    
            ) 
         );
    }

});