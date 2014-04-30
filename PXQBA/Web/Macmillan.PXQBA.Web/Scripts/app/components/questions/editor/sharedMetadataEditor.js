/**
* @jsx React.DOM
*/

var SharedMetadataEditor = React.createClass({displayName: 'SharedMetadataEditor',

    sourceEditHandler: function(sourceQuestion){
        var question = this.props.question;
        question.sourceQuestion = sourceQuestion;
        this.props.editHandler(question);
    },
    render: function() {
        return (
               React.DOM.div( {className:"source"}, 
                               React.DOM.span( {className:"label label-info metadata-info-label"}, "Shared values"),
                               MetadataFieldEditor( {editMode:false, question:this.props.question.sourceQuestion, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:"title"}),
                               MetadataFieldEditor(  {editMode:false, question:this.props.question.sourceQuestion, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:"chapter"}),
                               MetadataFieldEditor(  {editMode:false, question:this.props.question.sourceQuestion, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:"bank"}),
                               MetadataFieldEditor(  {editMode:false, question:this.props.question.sourceQuestion, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:"keywords"}),
                               MetadataFieldEditor(  {editMode:false, question:this.props.question.sourceQuestion, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:"suggestedUse", title:"Suggested Use"}),
                               MetadataFieldEditor(  {editMode:false, question:this.props.question.sourceQuestion, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:"learningObjectives", title:"Learning Objective"}),
                               MetadataFieldEditor(  {editMode:false, question:this.props.question.sourceQuestion, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:"excerciseNo", title:"Exercise Number"}),
                               MetadataFieldEditor(  {editMode:false, question:this.props.question.sourceQuestion, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:"difficulty", allowDeselect:true} ),
                               MetadataFieldEditor(  {editMode:false, question:this.props.question.sourceQuestion, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:"cognitiveLevel", title:"Cognitive Level"}),
                               MetadataFieldEditor(  {editMode:false, question:this.props.question.sourceQuestion, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:"status"} ),
                               MetadataFieldEditor( {editMode:false, question:this.props.question.sourceQuestion, metadata:this.props.metadata, editHandler:this.sourceEditHandler, field:"guidance"})
                           )

         );
    }
});