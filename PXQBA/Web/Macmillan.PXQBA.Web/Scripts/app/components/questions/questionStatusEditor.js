/**
* @jsx React.DOM
*/ 

var QuestionStatusEditor = React.createClass({displayName: 'QuestionStatusEditor',


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
            React.DOM.div(null, 
                    React.DOM.div( {className:"dropdown"}, 
                      React.DOM.button( {className:"btn dropdown-toggle sr-only", type:"button", id:"dropdownMenuType", 'data-toggle':"dropdown"}, 
                        "Dropdown",
                        React.DOM.span( {className:"caret"})
                      ),
                      React.DOM.ul( {className:"dropdown-menu menu-show", role:"menu", 'aria-labelledby':"dropdownMenuType", onClick:this.chengeStatusEventHandler}, 
                        React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", 'data-value':"Available to instructors"}, "Available to instructors")),
                        React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", 'data-value':"In progress"}, "In progress")),
                        React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", 'data-value':"Deleted"}, "Deleted"))
                      )
                    )
            )
            );
        }
});