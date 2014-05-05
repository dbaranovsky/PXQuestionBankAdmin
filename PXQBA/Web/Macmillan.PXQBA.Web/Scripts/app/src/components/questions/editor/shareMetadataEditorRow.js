/**
* @jsx React.DOM
*/
var ShareMetadataEditorRow = React.createClass({

	getInitialState: function() {

    var field = this.props.field; 
    var isDisabled = false;
    if (this.props.question.sourceQuestion == null){
      return ({isDisabled: isDisabled});
    }

    if (field == 'learningObjectives'){

        var localGuids =[];
        $.each(this.props.question[field], function(i, objective){
           localGuids.push(objective.guid);
       });

        var sharedGuids = [];

          $.each(this.props.question.sourceQuestion[field], function(i, objective){
           sharedGuids.push(objective.guid);
       });
      return { isDisabled: this.compareArray(localGuids, sharedGuids)};
    }

    if ($.isArray(this.props.question[field])){
       isDisabled = this.compareArray(this.props.question[field], this.props.question.sourceQuestion[field]);

    }else {
      isDisabled = this.props.question[field] === this.props.question.sourceQuestion[field];
    }

      return { isDisabled: isDisabled};
    },


   compareArray: function(arr1, arr2) {
    return $(arr1).not(arr2).length == 0 && $(arr2).not(arr1).length == 0;
    },

   renderSharedValue: function(){
        if (this.props.question.sourceQuestion != null){
             return  (<div className="cell shared">
                     <MetadataFieldEditor question={this.props.question.sourceQuestion} editMode={false} metadata={this.props.metadata} editHandler={this.sourceEditHandler} field={this.props.field} title={this.props.title} />
                 </div>);
        }
    },

    renderSwitchControl: function(){
       if (this.props.question.sourceQuestion != null){
         return  (<div className="cell control">
                     <div className="override-control">
                          <p> </p>
                         <div> Override</div> 
                         <div className="switch-wrapper"><input name="switcher" checked={!this.state.isDisabled} data-size="small" data-on-text="3" data-off-text="2.0.1" type="checkbox" /></div>
                     </div>
                 </div>);
       }
    },

    sourceEditHandler: function(sourceQuestion){
        var question = this.props.question;
        question.sourceQuestion = sourceQuestion;
        this.props.editHandler(question);
    },

    renderLocalValue: function(){
       
 //    alert(this.state.isDisabled); 
      return  (<div className="cell">
                 <MetadataFieldEditor question={this.props.question} isDisabled={this.state.isDisabled} metadata={this.props.metadata} editHandler={this.props.editHandler} field={this.props.field} title={this.props.title} />
                 </div>);

    },

    componentDidMount: function(){
    	$(this.getDOMNode()).find("[name='switcher']").switchButton({labels_placement: 'both', height: 10, on_callback: this.override, off_callback: this.restore});
    },

    toggleState: function(){
      this.setState( {isDisabled: !this.state.isDisabled})
    },

    override: function(){
      this.toggleState();
    },

    restore: function(){
        this.toggleState();
        var question = this.props.question;
        question[this.props.field] = question.sourceQuestion[this.props.field];
        this.props.editHandler(question);
    },


    render: function() {
    		return(   <div className="row">
                        {this.renderSharedValue()}
                        {this.renderSwitchControl()}
                        {this.renderLocalValue()}
                      </div>
                   );

   }
});