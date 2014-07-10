/**
* @jsx React.DOM
*/


var AvailibleValuesBodyItemLinks = React.createClass({displayName: 'AvailibleValuesBodyItemLinks',

    getInitialState: function() {
        return { items: this.props.value };
    },


    renderItems: function() {
        var itemsHtml = [];

        for(var i=0; i<this.state.items.length; i++) {
            itemsHtml.push(MetadataItemLinkRow( {item:this.state.items[i]} ));
        }

        return itemsHtml;
    },

    render: function() {
        return(
            React.DOM.div(null, 
                "Item links",
                React.DOM.table(null, 
                     React.DOM.thead(null, 
                        React.DOM.tr(null, 
                            React.DOM.td(null,  " ItemID"),
                            React.DOM.td(null,  " ItemTitle"),
                            React.DOM.td(null,  " " )
                        )
                     ),
                    React.DOM.tbody(null, 
                       this.renderItems()
                    )
                )
            )
            );
    },

});