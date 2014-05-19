/**
* @jsx React.DOM
*/
var MetadataFieldEditor = React.createClass({

     getInitialState: function() {

      var metadataField = this.getMetaField();
       var allowDeselect = metadataField != null ? metadataField.allowDeselect : false;
       var field = this.props.field;
      return { editMode: this.props.editMode === undefined? true : this.props.editMode,
               editMenu: false,
               allowDeselect: allowDeselect,
               metadataField:  metadataField};
    },

  getDefaultProps: function() {
    return {
      defaultType: window.enums.editorType.text
    };
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
        if (question[this.props.field][0] !== value)
        {
          var valueArr=[];
          valueArr.push(value);
          question[this.props.field] = valueArr;
          this.props.editHandler(question);
        }
       
    },


    renderMenuItems: function(availableChoices) {
        var items = [];
        if (this.state.allowDeselect){
            items.push(<option value=''></option>);
        }

        for(var i=0; i<availableChoices.length; i++) {
           items.push(this.renderMenuItem(availableChoices[i].text, availableChoices[i].value));
        }

        return items;
    },

    renderMenuItem: function(label, value) {
        return (<option value={value}>{label}</option>);
    },

     renderBody: function(){

       var field = this.props.field;
       var metadataField = this.props.reload? this.getMetaField() : this.state.metadataField;
       var editorType = metadataField != null ? metadataField.editorDescriptor.editorType : this.props.defaultType;  
       var currentValue = this.props.question[this.props.field];

       var availableChoice = [];
       if(metadataField!=null) {
           availableChoice = metadataField.editorDescriptor.availableChoice;
       } 



       if(this.state.editMode){
       switch (editorType) {
          case window.enums.editorType.singleSelect:
             currentValue = currentValue === undefined? "" : currentValue[0];
             return (<select ref="editor" className="single-selector" disabled={this.props.isDisabled} value={currentValue}> {this.renderMenuItems(availableChoice)} </select> );

          case window.enums.editorType.multiSelect:
              if(field=="learningObjectives"){
                return (<LearningObjectEditor values={currentValue} isDisabled={this.props.isDisabled} metadataField={metadataField} question={this.props.question} field={this.props.field} editHandler={this.props.editHandler} />);
              }
             return (<MultiSelectEditor values={currentValue} isDisabled={this.props.isDisabled} metadataField={metadataField} question={this.props.question} field={this.props.field} editHandler={this.props.editHandler} />);

          default: 
          currentValue = currentValue || "";
            if(metadataField!= null && metadataField.isMultiline){
                 return ( <textarea onChange={this.editHandler} disabled={this.props.isDisabled}  ref="editor" className={this.props.isDisabled? "disabled" : ""}  rows="10" type="text" placeholder="Enter text..." value={currentValue} />);  
             }
           
             return (<input type="text" onChange={this.editHandler} disabled={this.props.isDisabled} className={this.props.isDisabled? "disabled" : ""} ref="editor" value={currentValue}/>);
        }
      }

      var values = [];

      if(this.props.isUnique){
        return (<span>This field is unique for the current title and has no corresponding shared analogue</span>);
      }

       switch (editorType) {
          case window.enums.editorType.singleSelect:
              var singleSelectValue = metadataField.editorDescriptor.availableChoice[currentValue];
              if(currentValue!= ''){  
              values.push(<div className="current-values-view"> {singleSelectValue === undefined? currentValue : singleSelectValue} 
                                <span className="glyphicon glyphicon-pencil btn custom-btn"  data-toggle="tooltip" title="Edit" onClick={this.switchEditMode}></span>
                           </div>);
            }
              break;
          case window.enums.editorType.multiSelect:
                  if(field=="learningObjectives"){
                     $.each(currentValue, function(i, value){
                       values.push(<div className="current-values-view learning-objectives label label-warning"> {value.description} </div>);
                     });
                  }else{
                     $.each(currentValue, function(i, value){
                        values.push(<div className="current-values-view label label-warning">{value}</div>);
                     });
                  }

                   if (values.length == 0){
                      values.push(<div className="current-values-view"> No value 
                                    <span className="glyphicon glyphicon-pencil btn custom-btn"  data-toggle="tooltip" title="Edit" onClick={this.switchEditMode}></span>
                                  </div>);
                   } else{
                         values.push(<div className="current-values-view"> <span className="glyphicon glyphicon-pencil btn custom-btn"  data-toggle="tooltip" title="Edit" onClick={this.switchEditMode}></span> </div>);
                   }

                 break;               
          default: 
            if (currentValue != null && currentValue !=''){           
              values.push(<div className="current-values-view"> {currentValue} 
                               <span className="glyphicon glyphicon-pencil btn custom-btn"  data-toggle="tooltip" title="Edit" onClick={this.switchEditMode}></span>
                          </div>);
            }
        }

      if (values.length == 0){
         values.push(<div className="current-values-view"> No value  
                          <span className="glyphicon glyphicon-pencil btn custom-btn"  data-toggle="tooltip" title="Edit" onClick={this.switchEditMode}></span>
                     </div>);
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

       //todo: refactor
        if (this.state.editMenu){
             var metadataField = this.state.metadataField;
             var editorType = metadataField != null ? metadataField.editorDescriptor.editorType : 0;
             if(editorType != window.enums.editorType.singleSelect && editorType != window.enums.editorType.multiSelect){ 

                var node = this.getDOMNode();
                var self = this;
                $(node).find("input, textarea").on("keyup", function(){
                    self.updateValidatorState(node);
                });
                this.updateValidatorState(node);

         }
        }

    },

    updateValidatorState: function(node){
       var text_length = $(node).find("input, textarea").val().length;
       var text_remaining = 50 - text_length;
       $(node).find('.shared-validator').html(text_remaining + ' characters');
       if(text_remaining<0){
          $(node).find('.shared-validator').addClass('red');
          $(node).find("input, textarea").addClass("red-border");
        } else{
          $(node).find('.shared-validator').removeClass('red');
          $(node).find("input, textarea").removeClass("red-border");
         }
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
     
      question[this.props.field] = availableChoices[0].value;
      
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
                                           this.props.question[this.props.field], true);
    },

    renderMenu: function(){
        if (this.state.editMenu){

          return( <div className="shared-menu-container">
                    
                     <div className="shared-field-menu">
                     <span className="input-group-btn">
                                  <button type="button" className="btn btn-default btn-xs" onClick={this.applyChanges} data-toggle="tooltip" title="Apply"><span className="glyphicon glyphicon-ok"></span></button> 
                                  <button type="button" className="btn btn-default btn-xs" onClick={this.declineChanges} data-toggle="tooltip" title="Cancel"><span className="glyphicon glyphicon-remove"></span></button> 
                     </span>
                     </div> 
                       {this.renderValidator()}
                  </div>  );
        }

        return null;
    },

    renderValidator: function(){
         var metadataField = this.state.metadataField;
         var editorType = metadataField != null ? metadataField.editorDescriptor.editorType : 0;
         if(editorType != window.enums.editorType.singleSelect && editorType != window.enums.editorType.multiSelect){
           return (<div className="shared-validator">
                        20 characters
                      </div>);
         }
    },


    render: function() {
      var label = '';
      if (this.props.title === undefined){
        label = this.state.metadataField  == null? this.props.field : this.state.metadataField.friendlyName;
      } else{
        label = this.props.field;
      }

        return (

            <div className="metadata-field-editor">
                   <label>{label}</label>
                   <br />
                    {this.renderBody()}
                   <br />
                   {this.renderMenu()}
                    
            </div> 
         );
   }
    

});