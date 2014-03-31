/**
* @jsx React.DOM
*/

var QuestionNoDataStub = React.createClass({
    render: function() {
       return (
                <tr><td colSpan={this.props.colSpan}> <span className="info-message"> There is no data to be displayed. </span></td></tr>
             );
    }
});

