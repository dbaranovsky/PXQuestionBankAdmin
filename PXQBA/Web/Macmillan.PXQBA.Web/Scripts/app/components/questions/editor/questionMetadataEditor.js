/**
* @jsx React.DOM
*/
var QuestionMetadataEditor = React.createClass({displayName: 'QuestionMetadataEditor',

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

    },

    renderSharingNotification: function(){
      if (true) {
        return (React.DOM.div( {className:"shared-note"}, "This question is a duplicate of a  ",
                    React.DOM.a( {className:"shared-question-link", href:"", onClick:this.loadSourceQuestion}, "shared question"),
                    React.DOM.a( {href:"", onClick:this.loadSourceQuestion}, "Delete question")
               ));
      }

      return null;
    },


    render: function() {
        return (
             React.DOM.div( {className:"tab-body"}, 
                            this.renderSharingNotification(),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"title"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"chapter"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"bank"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"keywords"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"suggestedUse", title:"Suggested Use"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"learningObjectives", title:"Learning Objective"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"excerciseNo", title:"Excercise Number"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"difficulty", allowDeselect:true} ),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"cognitiveLevel", title:"Cognitive Level"}),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"status"} ),
                           MetadataFieldEditor( {question:this.props.question, metadata:this.state.metadata, editHandler:this.props.editHandler, field:"guidance"})
             ) 
         );
    }
});