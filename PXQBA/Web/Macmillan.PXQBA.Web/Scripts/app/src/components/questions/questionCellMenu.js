/**
* @jsx React.DOM
*/ 

var QuestionCellMenu = React.createClass({

    render: function() {
        return ( 
                <div>
                  <button type="button" className="btn btn-default btn-xs" disabled={this.props.isDisabled} onClick={this.props.onEditClickHandler} data-toggle="tooltip" title="Edit"><span className="glyphicon glyphicon-pencil"></span></button>
                </div>
            );
        }
});