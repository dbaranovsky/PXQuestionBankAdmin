/**
* @jsx React.DOM
*/
var ShareMetadataEditorRow = React.createClass({displayName: 'ShareMetadataEditorRow',

	getInitialState: function() {

    var field = this.props.field; 
    var isDisabled = false;
    var isUnique = false;

    if (!this.props.question.isShared){
      return ({isDisabled: isDisabled, isStatic: this.props.isStatic});
    }

    if (field == 'learningObjectives'){

        var localGuids =[];
        $.each(this.props.question.localValues[field], function(i, objective){
           localGuids.push(objective.guid);
       });

        var sharedGuids = [];

          $.each(this.props.question.defaultValues[field], function(i, objective){
           sharedGuids.push(objective.guid);
       });
      return { isDisabled: this.compareArray(localGuids, sharedGuids)};
    }

    if(this.props.isStatic){
        if ($.isArray(this.props.question.localSection[field])){
            isDisabled = this.compareArray(this.props.question.localSection[field], this.props.question.defaultSection[field]);

        }else {
          isDisabled = this.props.question.localSection[field] === this.props.question.defaultSection[field];
        }  
    }else{
        if ($.isArray(this.props.question.localSection.dynamicValues[field])){
            isDisabled = this.compareArray(this.props.question.localSection.dynamicValues[field], this.props.question.defaultSection.dynamicValues[field]);

        }else {
          isDisabled = this.props.question.localSection.dynamicValues[field] === this.props.question.defaultSection.dynamicValues[field];
        }  
    }

    

      if (this.props.isUnique){
        isDisabled = false;
      }
      
      return { isDisabled: isDisabled, isUnique: this.props.isUnique, isStatic: this.props.isStatic};
    },


   compareArray: function(arr1, arr2) {
    return $(arr1).not(arr2).length == 0 && $(arr2).not(arr1).length == 0;
    },

   renderSharedValue: function(){
        if (this.props.question.isShared ){
             return  (React.DOM.div( {className:this.props.isUnique? "cell shared unique" : "cell shared"}, 
                     MetadataFieldEditor( {question:this.props.isStatic?  this.props.question.defaultSection :  this.props.question.defaultSection.dynamicValues, 
                                           questionId:  this.props.question.id,
                                          editMode:false, 
                                          metadata:this.props.courseMetadata,
                                          editHandler:this.sharedEditHandler, 
                                          applyHandler: this.applyHandler,
                                          field:this.props.field, 
                                          title:this.props.title, 
                                          isUnique:this.state.isUnique})
                 ));
        }
    },

    renderSwitchControl: function(){
       if (this.props.question.isShared && this.state.isUnique != true){
         return  (React.DOM.div( {className:"cell control"}, 
                     React.DOM.div( {className:"override-control"}, 
                          React.DOM.p(null,  " " ),
                         React.DOM.div(null,  " Override"), 
                         React.DOM.div( {className:"switch-wrapper"}, React.DOM.input( {name:"switcher", checked:!this.state.isDisabled, 'data-size':"small", 'data-on-text':"3", 'data-off-text':"2.0.1", type:"checkbox"} ))
                     )
                 ));
       }
    },

    sharedEditHandler: function(sharedMetadata){
        var question = this.props.question;


        if (this.props.isStatic){
          question.defaultSection = sharedMetadata;
        }else{
            question.defaultSection.dynamicValues = sharedMetadata;
        }


        
        this.props.editHandler(question);
    },

    localEditHandler: function(localMetadata){
        var question = this.props.question;

        if (this.props.isStatic){
            question.localSection = localMetadata;
        }else{
            question.localSection.dynamicValues = localMetadata;

        }


        if (this.props.editHandler !== undefined){
          this.props.editHandler(question);  
        }
        
    },

    renderLocalValue: function(){
      return  (React.DOM.div( {className:"cell"}, 
                 MetadataFieldEditor( {question:this.props.isStatic?  this.props.question.localSection :  this.props.question.localSection.dynamicValues, 
                                    isDisabled:this.state.isDisabled, 
                                    metadata:this.props.metadata, 
                                    editHandler:this.localEditHandler, 
                                    field:this.props.field, 
                                    title:this.props.title} )
                 ));

    },

    componentDidMount: function(){
    	 this.updateSwitcher();
    },

    toggleState: function(){
      this.setState( {isDisabled: !this.state.isDisabled})
    },

    override: function(){
      this.toggleState();
    },

    restoreField: function(){
        this.setState({isDisabled: true});
        this.restoreLocalQuestion();   
    },

    restoreLocalQuestion: function(){
        var question = this.props.question;

        var localDescriptor = this.getMetaField(this.props.metadata);
        var defaultDescriptor = this.getMetaField(this.props.courseMetadata);

        if (localDescriptor == null || 
            defaultDescriptor == null || 
            (localDescriptor.editorDescriptor.editorType != window.enums.editorType.singleSelect && localDescriptor.editorDescriptor.editorType != window.enums.editorType.singleSelect)){
                                  if (this.props.isStatic){
                                      question.localSection[this.props.field] = question.defaultSection[this.props.field];
                                  }else{
                                      question.localSection.dynamicValues[this.props.field] =  question.defaultSection.dynamicValues[this.props.field];
                                  }
                                  this.props.editHandler(question);
                                  return;
        }

         var localAvailibleChoices = $.map(localDescriptor.editorDescriptor.availableChoice, function(n){ return n.text;});
         var defaultAvailibleChoices = $.map(defaultDescriptor.editorDescriptor.availableChoice, function(n){ return n.text;});
         var self = this;
         if(this.props.isStatic){

            if($.isArray(question.defaultSection[self.props.field])){
              question.localSection[this.props.field] = $.grep(question.defaultSection[self.props.field], function(el, i){  return $.inArray(el, localAvailibleChoices);});
            }
            else{
              var result = $.grep(localAvailibleChoices, function(el, i){ return el == question.defaultSection[self.props.field]; });
              question.localSection[this.props.field] = result.length == 0? "" : question.defaultSection[this.props.field];
           }


         }else{

            question.localSection.dynamicValues[this.props.field] = $.grep(question.defaultSection.dynamicValues[this.props.field], function(el, i){  return $.inArray(el, localAvailibleChoices);});

         }


      this.props.editHandler(question);
    },


     getMetaField: function(metadata){
       var field = this.props.field;
       var metadataField = $.grep(metadata, function(e){ return $.inArray(e.metadataName, [field, "dlap_q_"+field, "dlap_"+field, field.toLowerCase()])!=-1;  });
       
       return metadataField.length>0 ? metadataField[0]: null;
     },
    applyHandler: function(){

        this.restoreLocalQuestion();
        this.setState({isDisabled: true});
        $(this.getDOMNode()).find("[name='switcher']").switchButton({checked: false});
    },

    updateSwitcher: function(){
        $(this.getDOMNode()).find("[name='switcher']").switchButton({labels_placement: 'both', height: 10, on_callback: this.override, off_callback: this.restoreField});
    },

    render: function() {
    		return(   React.DOM.div( {className:"row"}, 
                        this.renderSharedValue(),
                        this.renderSwitchControl(),
                        this.renderLocalValue()
                      )
                   );

   }
});