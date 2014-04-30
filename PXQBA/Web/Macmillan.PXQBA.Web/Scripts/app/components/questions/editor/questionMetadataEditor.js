/**
* @jsx React.DOM
*/
var QuestionMetadataEditor = React.createClass({displayName: 'QuestionMetadataEditor',

   
    render: function() {
        var style = this.props.question.sourceQuestion != null? {} : {display: "none !important"};
        return (
            

                            React.DOM.div( {className:this.props.question.sourceQuestion == null ? "local" : "local wide"}, 
                                React.DOM.span( {className:"label label-info metadata-info-label", style:style}, "Local values"),
                               MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"title"}),
                               MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"chapter"}),
                               MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"bank"}),
                               MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"keywords"}),
                               MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"suggestedUse", title:"Suggested Use"}),
                               MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"learningObjectives", title:"Learning Objective"}),
                               MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"excerciseNo", title:"Exercise Number"}),
                               MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"difficulty", allowDeselect:true} ),
                               MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"cognitiveLevel", title:"Cognitive Level"}),
                               MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"status"} ),
                               MetadataFieldEditor( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"guidance"})
                           )

           
         );
    }
});