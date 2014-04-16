/**
* @jsx React.DOM
*/ 

var SingleSelectButton = React.createClass({displayName: 'SingleSelectButton',

    cancelValue: '__cancel',

    selectValueEventHandler: function(event) {
        var value = event.target.getAttribute("data-value");
        if(value==null) {
            this.props.cancelHandler();
            return;
        }

        if(value==this.cancelValue) {
            this.props.cancelHandler();
            return;
        }

        this.props.selectHandler(value);
    },

    renderMenuItems: function() {
        var items = [];
        for (var propertyName in this.props.values) {
            items.push(this.renderMenuItem(this.props.values[propertyName], propertyName));
        }
        return items;
    },

    renderMenuItem: function(label, value) {
        return (React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", 'data-value':value}, label)));
    },

    render: function() {
        return ( 
            React.DOM.div(null, 
                 React.DOM.div( {className:"btn-group"}, 
                  React.DOM.button( {type:"button", className:"btn btn-default btn-sm dropdown-toggle", 'data-toggle':"dropdown"}, 
                    this.props.caption, " ", React.DOM.span( {className:"caret"})
                  ),
                  React.DOM.ul( {className:"dropdown-menu", role:"menu", onClick:this.selectValueEventHandler}, 
                     this.renderMenuItems(),
                    React.DOM.li( {className:"divider"}),
                    React.DOM.li( {role:"presentation"}, React.DOM.a( {className:"edit-field-item", role:"menuitem", tabIndex:"-1", 'data-value':this.cancelValue}, "Cancel"))
                  )
                )
            )
            );
        }
});