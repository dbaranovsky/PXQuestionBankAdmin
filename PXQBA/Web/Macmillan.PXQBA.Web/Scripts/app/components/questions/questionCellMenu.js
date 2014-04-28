/**
* @jsx React.DOM
*/ 

var QuestionCellMenu = React.createClass({displayName: 'QuestionCellMenu',

    render: function() {
        return ( 
                React.DOM.div(null, 
                  React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.props.onEditClickHandler, 'data-toggle':"tooltip", title:"Edit"}, React.DOM.span( {className:"glyphicon glyphicon-pencil"}))
                )
            );
        }
});