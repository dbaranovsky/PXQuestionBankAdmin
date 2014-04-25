/**
* @jsx React.DOM
*/
var QuestionMetadataEditor = React.createClass({

    getInitialState: function() {
      return { metadata: []};
    },

    
    loadMetadata: function(data)
    {
        this.setState({metadata: data});
    },

    componentDidMount: function(){
       questionDataManager.getMetadataFields().done(this.loadMetadata); 
    },

    loadSourceQuestion: function(event){
      event.preventDefault();
      this.props.getSourceQuestion();
    },

    renderSharingNotification: function(){
      if (this.props.question.isDuplicateOfSharedQuestion && this.props.isDuplicate) {
        return (<div className="shared-note">This question is a duplicate of a &nbsp;
                    <a className="shared-question-link" href="" onClick={this.loadSourceQuestion}>shared question</a>
                    <a href="" onClick={this.loadSourceQuestion}>Delete question</a>
               </div>);
      }

      return null;
    },


    render: function() {
        return (
             <div className="tab-body">
                            {this.renderSharingNotification()}
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"title"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"chapter"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"bank"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"keywords"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"suggestedUse"} title="Suggested Use"/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"learningObjectives"} title="Learning Objective"/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"excerciseNo"} title="Exercise Number"/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"difficulty"} allowDeselect={true} />
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"cognitiveLevel"} title="Cognitive Level"/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"status"} />
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"guidance"}/>
             </div> 
         );
    }
});