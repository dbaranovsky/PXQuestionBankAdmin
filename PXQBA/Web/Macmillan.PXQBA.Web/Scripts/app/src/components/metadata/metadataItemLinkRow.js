/**
* @jsx React.DOM
*/


var MetadataItemLinkRow = React.createClass({

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
          return (<TextEditor value={this.state.draftItem.value} dataChangeHandler={this.fieldDataChangeHandler.bind(null, 'value')}/>);
        }

        return (<span>{this.state.item.value}</span>);
    },

    renderTextContent: function() {
        if(this.state.editMode) {
           return  (<TextEditor value={this.state.draftItem.text} dataChangeHandler={this.fieldDataChangeHandler.bind(null, 'text')}/>);
        }

        return (<span>{this.state.item.text}</span>);
    },

    renderMenu: function() {
        if(this.state.editMode) {
            return (<span className="input-group-btn">
                        <button type="button" className="btn btn-default btn-xs" onClick={this.editAcceptEventHandler} data-toggle="tooltip" title="Apply"><span className="glyphicon glyphicon-ok"></span></button> 
                        <button type="button" className="btn btn-default btn-xs" onClick={this.editCancelEventHandler} data-toggle="tooltip" title="Cancel"><span className="glyphicon glyphicon-remove"></span></button> 
                    </span>);
        }

        return (<span className="input-group-btn">
                    <button type="button" className="btn btn-default btn-xs" onClick={this.editEventHandler} data-toggle="tooltip" title="Apply"><span className="glyphicon glyphicon glyphicon-pencil"></span></button> 
                    <button type="button" className="btn btn-default btn-xs" onClick={this.removeEventHandler} data-toggle="tooltip" title="Cancel"><span className="glyphicon glyphicon-remove"></span></button> 
                </span>);
    },

    render: function() {
        return(
                <tr>
                    <td>{this.renderValueContent()}</td>
                    <td>{this.renderTextContent()}</td>
                    <td> 
                         {this.renderMenu()}
                     </td>
                </tr>
            );
    },

});