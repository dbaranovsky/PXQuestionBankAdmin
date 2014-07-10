/**
* @jsx React.DOM
*/


var MetadataItemLinkRow = React.createClass({displayName: 'MetadataItemLinkRow',

    getInitialState: function() {
        return { 
            item: this.props.item,
            editMode: false,
            draftItem: null
        };
    },


    editEventHandler: function() {
        this.setState({
            editMode: true,
            draftItem: $.extend(true, {}, this.state.item)
            });
    },

    removeEventHandler: function() {
        alert('removed!');
    },

    editAcceptEventHandler: function() {
        this.setState({
            editMode: false,
            item: this.state.draftItem
        });
    },

    editCancelEventHandler: function() {
        this.setState({editMode: false});
    },

    fieldDataChangeHandler: function(fieldName, newValue) {
        var draftItem = this.state.draftItem;
        draftItem[fieldName]=newValue;
        this.setState({draftItem: draftItem});
    },

    renderValueContent: function() {
        if(this.state.editMode) {
          return (TextEditor( {value:this.state.draftItem.value, dataChangeHandler:this.fieldDataChangeHandler.bind(null, 'value')}));
        }

        return (React.DOM.span(null, this.state.item.value));
    },

    renderTextContent: function() {
        if(this.state.editMode) {
           return  (TextEditor( {value:this.state.draftItem.text, dataChangeHandler:this.fieldDataChangeHandler.bind(null, 'text')}));
        }

        return (React.DOM.span(null, this.state.item.text));
    },

    renderMenu: function() {
        if(this.state.editMode) {
            return (React.DOM.span( {className:"input-group-btn"}, 
                        React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.editAcceptEventHandler, 'data-toggle':"tooltip", title:"Apply"}, React.DOM.span( {className:"glyphicon glyphicon-ok"})), 
                        React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.editCancelEventHandler, 'data-toggle':"tooltip", title:"Cancel"}, React.DOM.span( {className:"glyphicon glyphicon-remove"})) 
                    ));
        }

        return (React.DOM.span( {className:"input-group-btn"}, 
                    React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.editEventHandler, 'data-toggle':"tooltip", title:"Apply"}, React.DOM.span( {className:"glyphicon glyphicon glyphicon-pencil"})), 
                    React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.removeEventHandler, 'data-toggle':"tooltip", title:"Cancel"}, React.DOM.span( {className:"glyphicon glyphicon-remove"})) 
                ));
    },

    render: function() {
        return(
                React.DOM.tr(null, 
                    React.DOM.td(null, this.renderValueContent()),
                    React.DOM.td(null, this.renderTextContent()),
                    React.DOM.td(null,  
                         this.renderMenu()
                     )
                )
            );
    },

});