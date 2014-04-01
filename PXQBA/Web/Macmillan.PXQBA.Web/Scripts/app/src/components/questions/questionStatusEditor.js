/**
* @jsx React.DOM
*/ 

var QuestionStatusEditor = React.createClass({


    chengeStatusEventHandler: function(event) {
        var value = event.target.getAttribute("data-value");
        if(value != null) {
            asyncManager.startWait();
            questionDataManager.saveQuestionData(this.props.metadata.questionId,
                                                 this.props.metadata.field,
                                                 value);
 
        }
        this.props.afterEditingHandler();
    },

     render: function() {
        return ( 
            <div>
                    <div className="dropdown">
                      <button className="btn dropdown-toggle sr-only" type="button" id="dropdownMenuType" data-toggle="dropdown">
                        Dropdown
                        <span className="caret"></span>
                      </button>
                      <ul className="dropdown-menu menu-show" role="menu" aria-labelledby="dropdownMenuType" onClick={this.chengeStatusEventHandler}>
                        <li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" data-value="Available to instructors">Available to instructors</a></li>
                        <li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" data-value="In progress">In progress</a></li>
                        <li role="presentation"><a className="edit-field-item" role="menuitem" tabIndex="-1" data-value="Deleted">Deleted</a></li>
                      </ul>
                    </div>
            </div>
            );
        }
});