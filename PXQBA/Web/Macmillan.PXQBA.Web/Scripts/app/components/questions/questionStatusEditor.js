/**
* @jsx React.DOM
*/ 

var QuestionStatusEditor = React.createClass({displayName: 'QuestionStatusEditor',

    chengeStatusEventHandler: function(event) {
        var value = event.target.getAttribute("data-value");
        if(value != null) {
            questionDataManager.saveQuestionData(this.props.metadata.questionId,
                                                 this.props.metadata.field,
                                                 value);
 
        }
        this.props.afterEditingHandler();
    },

    renderMenuItems: function() {
        var items = [];
        for (var propertyName in this.props.statusEnum) {
            items.push(this.renderMenuItem(this.props.statusEnum[propertyName]));
        }
        return items;
    },

    renderMenuItem: function(value) {
        return (React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", 'data-value':value}, value)));
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
                       this.renderMenuItems()
                     )
                 )
            )
            );
        }
});