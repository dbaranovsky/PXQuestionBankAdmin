/**
* @jsx React.DOM
*/

var TitleListSelector = React.createClass({displayName: 'TitleListSelector',

    renderTitles: function() {
        titlesHtml = []
        for(var i=0; i<this.props.data.length; i++) {
            titlesHtml.push(this.renderTitle(this.props.data[i]));
        }
            
        return titlesHtml;
    },

    renderTitle: function(titleModel) {
        return (TitleSelector( {data:titleModel, selectTitleHandler:this.props.selectTitleHandler}));
    },



    render: function() {
       return (
                React.DOM.div(null, 
                   React.DOM.div( {className:"title-list-selector shared-note"}, 
                        "Select title to import to", 
                 
                        React.DOM.div( {className:"selector-menu"}, 
                        this.props.renderSelectorMenu == undefined? "" : this.props.renderSelectorMenu()
                      )

                   ),

                     this.renderTitles()
                )
            );
    }
});

