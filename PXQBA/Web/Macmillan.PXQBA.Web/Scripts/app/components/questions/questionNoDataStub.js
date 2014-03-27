/**
* @jsx React.DOM
*/

var QuestionNoDataStub = React.createClass({displayName: 'QuestionNoDataStub',
    render: function() {
       return (
                React.DOM.tr(null, React.DOM.td( {colSpan:this.props.colSpan},  " ", React.DOM.span( {className:"info-message"},  " There is no data to be displayed. " )))
             );
    }
});

