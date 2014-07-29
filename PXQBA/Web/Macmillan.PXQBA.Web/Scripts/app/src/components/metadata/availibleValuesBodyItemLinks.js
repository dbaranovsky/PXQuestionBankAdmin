/**
* @jsx React.DOM
*/


var AvailibleValuesBodyItemLinks = React.createClass({

    getInitialState: function() {
        var items = [];

        if(this.props.value!=null) {
            items = this.props.value.splice(0);
        }

        return {
             items: items,
             needScrollDown: false
        };
    },

    componentDidUpdate: function() {
        if(this.state.needScrollDown) {
            this.setState({needScrollDown: false})
            this.moveScrollToEndTable();
        }
    },

    editAvailibleValuesHandler: function() {
      var inEditMode = this.isEditModeEnabled();
   
      if(inEditMode) {
        if (!confirm("You have unsaved changes for this items links. Do you want discard your changes?")) {
          return;
        }
      }

      this.props.updateHandler(this.props.itemIndex, "valuesOptions",  this.state.items);
      this.refs.cancelButton.getDOMNode().click();
    },

    isEditModeEnabled: function() {
        var inEditMode = false;
        for (var name in this.refs) {
            if(name=="cancelButton") {
                continue;
            }
            if(this.refs[name].state.editMode) {
                inEditMode = true;
            }
        }

        return inEditMode;
    },


    renderItems: function() {
        var itemsHtml = [];

        for(var i=0; i<this.state.items.length; i++) {
            itemsHtml.push(<MetadataItemLinkRow 
                                    ref={"item-"+i}
                                    index={i}
                                    item={this.state.items[i]}
                                    disabled={!this.props.canEdit}
                                    deleteItemHandler={this.deleteItemHandler}
                                    editItemHandler={this.updateItemHandler}
                                    editModeOff={this.editModeOff}
                                    />);
        }

        return itemsHtml;
    },

    editModeOff: function() {
         for (var name in this.refs) {
            if(name=="cancelButton") {
                continue;
            }
            this.refs[name].setState({editMode:false});
        }
    },

    deleteItemHandler: function(index) {
        var items = this.state.items;
        items.splice(index, 1);
        this.setState({items: items});
    },

    updateItemHandler: function(index, updatedItem) {
        var items = this.state.items;
        items[index]=updatedItem;
        this.setState({items: items});
    },

    addItemHandler: function() {
        var newItem = {
            value: "",
            text: ""
        };
        var items = this.state.items;
        items.push(newItem);
        this.setState({
            items: items,
            needScrollDown: true
        });
 
    },

    moveScrollToEndTable: function() {
        var tableContainer = this.getDOMNode().getElementsByClassName('item-liks-table-container')[0];
        tableContainer.scrollTop = tableContainer.scrollHeight;
    },

    pasteHandler: function(event) {
        var text = event.target.value;
        var items = this.state.items;
        var rows = text.split('\n');
        for(var i=0; i<rows.length; i++) {
            var pair = rows[i].split('\t');
            if((!this.validatePair(pair))&&((i+1)!=rows.length)) {
                notificationManager.showWarning('Please copy&paste only a table with two (ID and Title) rows');
                return;
            }
            var item = this.buildItemFormPair(pair);
            if(item!=null) {
               items.push(item);
            }
        }
    
        this.setState({items: items});
    },

    validatePair: function (pair) {
        if(pair==null) {
            return false;
        }

        if(pair.length!=2) {
            return false;
        }

        return true;
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
                <div className="item-liks-table-container">
                    <table className="table table item-liks-table">
                        <thead>
                              <tr>
                                  <td style={{width:'40%'}} className="item-liks-table-cell"><b>ItemID</b></td>
                                  <td style={{width:'40%'}} className="item-liks-table-cell"><b>ItemTitle</b></td>
                                  <td style={{width:'20%'}}></td>
                             </tr>
                         </thead>
                         <tbody>{this.renderItems()}</tbody>
                    </table>
                </div>
                <div>
                 {this.renderAddButton()}
                </div>
                <div className="modal-footer clearfix">
                    <button ref="cancelButton" type="button" className="btn btn-default" data-dismiss="modal">Cancel</button>
                    <button type="button" className="btn btn-primary" onClick={this.editAvailibleValuesHandler}>Save</button>
                </div>
            </div>
            );
    },

});