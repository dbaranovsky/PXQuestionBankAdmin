/**
* @jsx React.DOM
*/ 

var Question = React.createClass({displayName: 'Question',
    render: function() {

        return ( 
            React.DOM.tr( {className:"question"}, 
                React.DOM.td(null,  
                    React.DOM.input( {type:"checkbox"})
                ),

                React.DOM.td( {className:"eBookChapter"}, 
                    this.props.metadata.eBookChapter
                ),

                React.DOM.td( {className:"questionBank"}, 
                    this.props.metadata.questionBank
                ),

                React.DOM.td( {className:"questionSeq"}, 
                    this.props.metadata.questionSeq
                ),

                React.DOM.td( {className:"title"}, 
                    React.DOM.div(null, 
                    React.DOM.span( {className:"glyphicon glyphicon-chevron-right"}),
                    this.props.metadata.title
                    ),
                    QuestionPreview( {preview:this.props.metadata.questionHtmlInlinePreview})
                    
                ),

                React.DOM.td( {className:"questionType"}, 
                    this.props.metadata.questionType
                ),

                React.DOM.td( {className:"actions"}  
                
                )  
            ) 
            );
        }
});