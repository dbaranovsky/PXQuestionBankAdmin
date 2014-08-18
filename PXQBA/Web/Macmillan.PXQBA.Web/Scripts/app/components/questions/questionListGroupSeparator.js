/**
* @jsx React.DOM
*/

var QuestionListGroupSeparator = React.createClass({displayName: 'QuestionListGroupSeparator',
    render: function() {
        return (
                 React.DOM.tr(null, React.DOM.td({colSpan: this.props.colSpan}, " "))
             );
}
});

