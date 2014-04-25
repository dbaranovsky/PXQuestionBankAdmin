/**
* @jsx React.DOM
*/

var TitlePage = React.createClass({displayName: 'TitlePage',

    getInitialState: function() {
      return { loading: false };
    },

    render: function() {
       return (
                React.DOM.div(null, 
                     "Page"
                )
            );
    }
});

