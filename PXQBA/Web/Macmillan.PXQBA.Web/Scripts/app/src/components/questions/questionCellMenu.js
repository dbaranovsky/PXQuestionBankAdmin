/**
* @jsx React.DOM
*/ 

var QuestionCellMenu = React.createClass({

    render: function() {
        return ( 
                <div>
                  <button type="button" className="btn btn-default btn-xs" onClick={this.props.onEditClickHandler}><span className="glyphicon glyphicon-pencil"></span></button>
                </div>
            );
        }
});