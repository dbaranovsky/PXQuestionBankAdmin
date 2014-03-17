/**
* @jsx React.DOM
*/ 

var QuestionListMenu = React.createClass({

    render: function() {
        return ( 
                <div>
                  <button type="button" className="btn btn-default btn-sm"><span className="glyphicon glyphicon-pencil"></span></button>
                  <button type="button" className="btn btn-default btn-sm"><span className="glyphicon glyphicon-copyright-mark"></span></button>
                  <button type="button" className="btn btn-default btn-sm"><span className="glyphicon glyphicon-trash"></span></button>
                </div>
            );
        }
});