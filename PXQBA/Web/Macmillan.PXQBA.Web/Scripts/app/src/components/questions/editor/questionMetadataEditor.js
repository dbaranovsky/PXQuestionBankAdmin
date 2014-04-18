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
    render: function() {
        return (
             <div className="tab-body">
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"title"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"chapter"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"bank"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"keywords"}/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"excerciseNo"} title="Excercise Number"/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"difficulty"} allowDeselect={true} />
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"cognitiveLevel"} title="Cognitive Level"/>
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"status"} />
                           <MetadataFieldEditor question={this.props.question} metadata={this.state.metadata} editHandler={this.props.editHandler} field={"guidance"}/>
             </div> 
         );
    }
});