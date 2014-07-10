/**
* @jsx React.DOM
*/


var AvailibleValuesBodyItemLinks = React.createClass({

    getInitialState: function() {
        return { items: this.props.value };
    },


    renderItems: function() {
        var itemsHtml = [];

        for(var i=0; i<this.state.items.length; i++) {
            itemsHtml.push(<MetadataItemLinkRow item={this.state.items[i]} />);
        }

        return itemsHtml;
    },

    render: function() {
        return(
            <div>
                Item links
                <table>
                     <thead>
                        <tr>
                            <td> ItemID</td>
                            <td> ItemTitle</td>
                            <td> </td>
                        </tr>
                     </thead>
                    <tbody>
                       {this.renderItems()}
                    </tbody>
                </table>
            </div>
            );
    },

});