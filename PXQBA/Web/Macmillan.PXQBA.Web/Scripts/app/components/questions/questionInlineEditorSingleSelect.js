/**
* @jsx React.DOM
*/ 

var QuestionInlineEditorSingleSelect = React.createClass({displayName: 'QuestionInlineEditorSingleSelect',

    cancelValue: '__cancel',

    changeEventHandler: function(event) {
        var value = event.target.getAttribute("data-value");
        if(value==this.cancelValue) {
            this.props.afterEditingHandler();
            return;
        }

        this.props.saveVelueHandler(value)
    },

    renderMenuItems: function() {
        var items = [];
     
        for(var i=0; i<this.props.values.length; i++){
            items.push(this.renderMenuItem(this.props.values[i].text, this.props.values[i].value));
        }

        return items;
    },

    renderMenuItem: function(label, value) {
        return (React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", 'data-value':value}, label)));
    },

    render: function() {
        return ( 
            React.DOM.div(null, 
                React.DOM.div( {className:"dropdown"}, 
                    React.DOM.ul( {className:"dropdown-menu menu-show", role:"menu", 'aria-labelledby':"dropdownMenuType", onClick:this.changeEventHandler}, 
                       this.renderMenuItems(),
                    React.DOM.li( {role:"presentation", className:"divider"}),
                    React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", 'data-value':this.cancelValue}, "Cancel"))
                     )
                 )
            )
            );
        }
});