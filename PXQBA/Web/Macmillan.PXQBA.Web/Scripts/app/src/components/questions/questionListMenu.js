/**
* @jsx React.DOM
*/ 

var QuestionListMenu = React.createClass({

    editNotesHandler: function(){
      this.props.editNotesHandler();
    },

    copyQuestionHandler: function() {
      this.props.copyQuestionHandler();
    },

    editQuestionHandler: function() {
        this.props.editQuestionHandler();
    },

    render: function() {
        return ( 
                <div>
                  <button type="button" className="btn btn-default btn-sm" onClick={this.editNotesHandler}><span className="glyphicon glyphicon-list-alt"></span></button>	
                  <button type="button" className="btn btn-default btn-sm" onClick={this.editQuestionHandler}><span className="glyphicon glyphicon-pencil"></span></button>
                  <button type="button" className="btn btn-default btn-sm" onClick={this.copyQuestionHandler}><span className="glyphicon glyphicon-copyright-mark"></span></button>
                  <button type="button" className="btn btn-default btn-sm"><span className="glyphicon glyphicon-trash"></span></button>
                </div>
            );
        }
});