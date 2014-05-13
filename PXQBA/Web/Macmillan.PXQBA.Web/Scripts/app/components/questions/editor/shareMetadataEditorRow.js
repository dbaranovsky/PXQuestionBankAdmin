/**
* @jsx React.DOM
*/
var ShareMetadataEditorRow = React.createClass({displayName: 'ShareMetadataEditorRow',

	getInitialState: function() {

    var field = this.props.field; 
    var isDisabled = false;
    var isUnique = false;
    if (this.props.question.sharedMetadata == null){
      return ({isDisabled: isDisabled});
    }

    if (field == 'learningObjectives'){

        var localGuids =[];
        $.each(this.props.question.localMetadata[field], function(i, objective){
           localGuids.push(objective.guid);
       });

        var sharedGuids = [];

          $.each(this.props.question.sharedMetadata[field], function(i, objective){
           sharedGuids.push(objective.guid);
       });
      return { isDisabled: this.compareArray(localGuids, sharedGuids)};
    }

    if ($.isArray(this.props.question.localMetadata[field])){
       isDisabled = this.compareArray(this.props.question.localMetadata[field], this.props.question.sharedMetadata[field]);

    }else {
      isDisabled = this.props.question.localMetadata[field] === this.props.question.sharedMetadata[field];
    }

      if (this.props.isUnique){
        isDisabled = false;
      }
      
      return { isDisabled: isDisabled, isUnique: this.props.isUnique};
    },


   compareArray: function(arr1, arr2) {
    return $(arr1).not(arr2).length == 0 && $(arr2).not(arr1).length == 0;
    },

   renderSharedValue: function(){
        if (this.props.question.sharedMetadata != null){
             return  (React.DOM.div( {className:this.props.isUnique? "cell shared unique" : "cell shared"}, 
                     MetadataFieldEditor( {question:this.props.question.sharedMetadata, 
                                          editMode:false, 
                                          metadata:this.props.metadata,
                                          editHandler:this.sharedEditHandler, 
                                          applyHandler: this.applyHandler,
                                          field:this.props.field, 
                                          title:this.props.title, 
                                          isUnique:this.state.isUnique})
                 ));
        }
    },

    renderSwitchControl: function(){
       if (this.props.question.sharedMetadata != null && this.state.isUnique != true){
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
        question.sharedMetadata = sharedMetadata;
        this.props.editHandler(question);
    },

    localEditHandler: function(localMetadata){
        var question = this.props.question;
        question.localMetadata = localMetadata;
        if (this.props.editHandler !== undefined){
          this.props.editHandler(question);  
        }
        
    },

    renderLocalValue: function(){
      return  (React.DOM.div( {className:"cell"}, 
                 MetadataFieldEditor( {question:this.props.question.localMetadata, 
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
        question.localMetadata[this.props.field] = question.sharedMetadata[this.props.field];
        this.props.editHandler(question);
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