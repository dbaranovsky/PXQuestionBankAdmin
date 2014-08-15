/**
* @jsx React.DOM
*/

var QuestionMetadataList = React.createClass({displayName: 'QuestionMetadataList',

    renderOption: function(questionFieldDescriptor) {
         return ( React.DOM.li(null, " ", React.DOM.a({className: "add-column-item", 'data-field': questionFieldDescriptor.metadataName}, 
                        questionFieldDescriptor.friendlyName)
                  )
                );
    },

    renderStub: function() {
        return (React.DOM.li(null, " ", React.DOM.div({className: "add-columns-message"}, this.props.noValueLabel)));
    },

    onClickEventHandler: function(event) {
        this.props.onClickEventHandler(event);
    },

    renderMenuOption: function() {
        var self = this;
        options = this.props.fields.map(function(questionFieldDescriptor) {
                                    return self.renderOption(questionFieldDescriptor);
                                 });
        
        if(options.length==0) {
            options.push(this.renderStub());
        }
        return options;
    },

    render: function() {
         return (
                 React.DOM.ul({className: "dropdown-menu", onClick: this.onClickEventHandler}, 
                            this.renderMenuOption()
                 )
            );
    }
});
