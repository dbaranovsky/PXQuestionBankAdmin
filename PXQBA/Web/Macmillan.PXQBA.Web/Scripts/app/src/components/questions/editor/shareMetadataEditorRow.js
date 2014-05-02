/**
* @jsx React.DOM
*/
var ShareMetadataEditorRow = React.createClass({

	getInitialState: function() {

    var field = this.props.field; 
      return { isDisabled: this.props.question.sourceQuestion != null && this.props.question[field] === this.props.question.sourceQuestion[field]};
    },

   renderSharedValue: function(){
        if (this.props.question.sourceQuestion != null){
             return  (<div className="cell">
                     <MetadataFieldEditor question={this.props.question.sourceQuestion} editMode={false} metadata={this.props.metadata} editHandler={this.sourceEditHandler} field={this.props.field} title={this.props.title} />
                 </div>);
        }
    },

    renderSwitchControl: function(){
       if (this.props.question.sourceQuestion != null){
         return  (<div className="cell control">
                      Override <input name="switcher" checked="checked" data-size="small" data-on-text="3" data-off-text="2.0.1" type="checkbox" />
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
    	$(this.getDOMNode()).find("[name='switcher']").bootstrapSwitch();
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