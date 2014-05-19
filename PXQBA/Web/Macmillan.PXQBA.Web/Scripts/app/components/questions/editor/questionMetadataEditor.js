/**
* @jsx React.DOM
*/
var QuestionMetadataEditor = React.createClass({displayName: 'QuestionMetadataEditor',
   
    render: function() {
        var style = this.props.question.sharedMetadata != null? {} : {display: "none !important"};
        return ( React.DOM.div( {className:this.props.question.sharedMetadata == null ? "local" : "local wide"}, 
                      React.DOM.div( {className:"row header", style:style}, 
                        React.DOM.div( {className:"cell"},  " ", React.DOM.span( {className:"label label-default metadata-info-label"}, "Shared values")),
                        React.DOM.div( {className:"cell control"}),
                        React.DOM.div( {className:"cell"},  " ", React.DOM.span( {className:"label label-default metadata-info-label"}, "Local values"))
                      ),
                      React.DOM.div( {className:"body-container"}, 

                         
                        ShareMetadataEditorRow( {question:this.props.question, metadata:this.props.metadata, editHandler:this.props.editHandler, field:"title"} )
                 
                     )          
                 )

           
         );
    }
});