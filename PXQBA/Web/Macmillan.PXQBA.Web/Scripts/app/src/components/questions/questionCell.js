/**
* @jsx React.DOM
*/ 

var QuestionCell = React.createClass({
    render: function() {
        return ( 
                <td>
                    {this.props.value}
                </td>
            );
        }
});