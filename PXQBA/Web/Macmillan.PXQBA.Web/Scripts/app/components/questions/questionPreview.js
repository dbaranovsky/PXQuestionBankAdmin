/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({displayName: 'QuestionPreview',

       renderGroupLine: function() {
        if(this.props.grouped) {
          return (React.DOM.td( {className:"grouped-cell"}));
        }

        return (React.DOM.td( {className:"grouped-cell-empty"}));
       },

       render: function() {
            return ( 
                  React.DOM.tr( {className:"grid-question-preview",
                      onClick:this.props.onClickQuestionEventHandler}, 
                    this.renderGroupLine(),
                    React.DOM.td( {colSpan:this.props.colSpan}, 
                      QuestionPreviewContent( 
                         {metadata:this.props.metadata, 
                         preview:this.props.preview, 
                        questionCardTemplate:this.props.questionCardTemplate}
                      )
                    )
                  )
            );
        }
});


 