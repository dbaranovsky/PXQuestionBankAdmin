/**
* @jsx React.DOM
*/ 

var QuestionListMenu = React.createClass({displayName: 'QuestionListMenu',

    renderNotesHandler: function(){
      this.props.renderNotes();
    },

    copyQuestionHandler: function() {
      this.props.copyQuestionHandler();
    },

    editQuestionHandler: function() {
        this.props.editQuestionHandler();
    },

    render: function() {
        return ( 
                React.DOM.div(null, 
                  React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.renderNotesHandler}, React.DOM.span( {className:"glyphicon glyphicon-list-alt"})),	
                  React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.editQuestionHandler}, React.DOM.span( {className:"glyphicon glyphicon-pencil"})),
                  React.DOM.button( {type:"button", className:"btn btn-default btn-sm", onClick:this.copyQuestionHandler}, React.DOM.span( {className:"glyphicon glyphicon-copyright-mark"})),
                  React.DOM.button( {type:"button", className:"btn btn-default btn-sm"}, React.DOM.span( {className:"glyphicon glyphicon-trash"}))
                )
            );
        }
});