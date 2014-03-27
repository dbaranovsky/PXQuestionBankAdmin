/**
* @jsx React.DOM
*/

var Loader = React.createClass({displayName: 'Loader',
    render: function() {
       return (
                React.DOM.div(null, 
                    React.DOM.div( {className:"loader-curtain"},  " " ),
                    React.DOM.div( {className:"loader-spinner"},  " " )
                )
            );
    }
});

