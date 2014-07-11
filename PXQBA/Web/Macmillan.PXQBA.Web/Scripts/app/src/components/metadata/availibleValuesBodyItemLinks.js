/**
* @jsx React.DOM
*/


var AvailibleValuesBodyItemLinks = React.createClass({

    getInitialState: function() {
        return { items: this.props.value };
    },

    editAvailibleValuesHandler: function() {
       this.props.updateHandler(this.props.itemIndex, "valuesOptions",  this.state.items);
    },


    renderItems: function() {
        var itemsHtml = [];

        for(var i=0; i<this.state.items.length; i++) {
            debugger;
            itemsHtml.push(<MetadataItemLinkRow 
                                    index={i}
                                    item={this.state.items[i]}
                                    deleteItemHandler={this.deleteItemHandler}
                                    editItemHandler={this.updateItemHandler}
                                    />);
        }

        return itemsHtml;
    },

    deleteItemHandler: function(index) {
        debugger;
        var items = this.state.items;
        items.splice(index, 1);
        this.setState({items: items});
    },

    updateItemHandler: function(index, updatedItem) {
        var items = this.state.items;
        items[i]=updatedItem;
        this.setState({items: items});
    },

    addItemHandler: function() {
        var newItem = {};
        var items = this.state.items;
        items.push(newItem);
        this.setState({items: items});
    },

    pasteHandler: function(event) {
        var text = event.target.value;
        var items = this.state.items;
        var rows = text.split('\n');
        for(var i=0; i<rows.length; i++) {
            var pair = rows[i].split('\t');

            var item = this.buildItemFormPair(pair);
            if(item!=null) {
               items.push(item);
            }
        }
    
        this.setState({items: items});
    },

    buildItemFormPair: function(pair) {
        if((pair[0]==undefined)||(pair[1]==undefined)) {
            return null;
        }

        return {
            value: pair[0],
            text: pair[1]
        };
    },

    renderTextDescription: function() {
        return (<div className="metadata-item-links-descripton-container">
                List all possible course items for <b>{this.props.fieldNameCaption}</b> below, one
                per row. Values will appear to editors and instructors in the order listed. Tip: You may
                copy values from a spreadsheet with item ID and title columns and paste them into field below.
               </div>);
    },

    renderAddButton: function() {
        var classNameText="btn btn-primary metadata-button";

        if(!this.props.canEdit) {
            classNameText+=" disabled";
        }

        return (<button type="button" className={classNameText}  onClick={this.addItemHandler} >Add row</button>);
    },

    renderPasteHolder: function() {
        return (<textarea placeholder="Paste values form spreadsheet here" 
                 className="availible-values-paste-area"
                 disabled={!this.props.canEdit ? 'disabled' : undefined}
                 onChange={this.pasteHandler} 
                 value=""
                 rows="1">
                 </textarea>);
    },

    render: function() {
        return(
            <div>
                <div>
                    {this.renderTextDescription()}
                </div>
                <div>
                    {this.renderPasteHolder()}
                </div>
                <table className="table table item-liks-table">
                     <thead>
                        <tr>
                            <td style={{width:'40%'}} className="item-liks-table-cell"><b>ItemID</b></td>
                            <td style={{width:'40%'}} className="item-liks-table-cell"><b>ItemTitle</b></td>
                            <td style={{width:'20%'}}></td>
                        </tr>
                     </thead>
                    <tbody>
                       {this.renderItems()}
                    </tbody>
                </table>
                <div>
                 {this.renderAddButton()}
                </div>
                <div className="modal-footer clearfix">
                    <button type="button" className="btn btn-default" data-dismiss="modal">Cancel</button>
                    <button type="button" className="btn btn-primary" data-dismiss="modal" onClick={this.editAvailibleValuesHandler}>Save</button>
                </div>
            </div>
            );
    },

});