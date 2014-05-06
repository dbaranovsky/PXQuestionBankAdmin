/**
* @jsx React.DOM
*/
var QuestionMetadataEditor = React.createClass({displayName: 'QuestionMetadataEditor',
   
    render: function() {
        var style = this.props.question.sourceQuestion != null? {} : {display: "none !important"};
        return ( React.DOM.div( {className:this.props.question.sourceQuestion == null ? "local" : "local wide"}, 
                      React.DOM.div( {className:"row", style:style}, 
                        React.DOM.div( {className:"cell"},  " ", React.DOM.span( {className:"label label-info metadata-info-label"}, "Shared values")),
                        React.DOM.div( {className:"cell control"}),
                        React.DOM.div( {className:"cell"},  " ", React.DOM.span( {className:"label label-info metadata-info-label"}, "Local values"))
                      ),

                         
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"title"} ),
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"chapter"} ),
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"bank"} ),
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"keywords"} ),
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"suggestedUse", title:"Suggested Use"}),
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"learningObjectives",  title:"Learning Objective"}),
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"excerciseNo", title:"Exercise Number"}),
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"difficulty"} ),
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"cognitiveLevel", title:"Cognitive Level"}),
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"status"} ),
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"guidance"} )            
                 )

           
         );
    }
});