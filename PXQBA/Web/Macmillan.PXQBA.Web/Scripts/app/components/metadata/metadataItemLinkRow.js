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

    componentWillReceiveProps: function(nextProps) {
        this.setState({
            item: nextProps.item,
            editMode: false});
    },

    editEventHandler: function() {
        this.props.editModeOff();
        this.setState({
            editMode: true,
            draftItem: $.extend(true, {}, this.state.item)
            });
    },

    removeEventHandler: function() {
        this.props.deleteItemHandler(this.props.index);
    },

    editAcceptEventHandler: function() {
        this.setState({
            editMode: false,
            item: this.state.draftItem
        });
        this.props.editItemHandler(this.props.index, this.state.draftItem);
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
        if(this.props.disabled) {
            return null;
        }

        if(this.state.editMode) {
            return (React.DOM.span( {className:"input-group-btn"}, 
                        React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.editAcceptEventHandler, 'data-toggle':"tooltip", title:"Apply"}, React.DOM.span( {className:"glyphicon glyphicon-ok"})), 
                        React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.editCancelEventHandler, 'data-toggle':"tooltip", title:"Cancel"}, React.DOM.span( {className:"glyphicon glyphicon-remove"})) 
                    ));
        }

        return (React.DOM.span( {className:"input-group-btn"}, 
                    React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.editEventHandler, 'data-toggle':"tooltip", title:"Apply"}, React.DOM.span( {className:"icon-pencil-1"})), 
                    React.DOM.button( {type:"button", className:"btn btn-default btn-xs", onClick:this.removeEventHandler, 'data-toggle':"tooltip", title:"Cancel"}, React.DOM.span( {className:"glyphicon glyphicon-remove"})) 
                ));
    },

    render: function() {
        return(
                React.DOM.tr(null, 
                    React.DOM.td( {className:"item-liks-table-cell"}, this.renderValueContent()),
                    React.DOM.td( {className:"item-liks-table-cell"}, this.renderTextContent()),
                    React.DOM.td( {className:"item-liks-table-cell-menu"},  
                         this.renderMenu()
                     )
                )
            );
    },

});