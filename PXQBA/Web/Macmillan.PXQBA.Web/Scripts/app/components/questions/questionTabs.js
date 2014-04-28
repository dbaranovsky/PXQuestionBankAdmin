/**
* @jsx React.DOM
*/ 

var QuestionTabs = React.createClass({displayName: 'QuestionTabs',

      render: function() {
        return ( 
            			React.DOM.div(null,  
            			React.DOM.div( {className:"product-title"},  " ", this.props.response.productTitle),
                         QuestionGrid( {response:this.props.response, handlers:this.props.handlers})
              			)
            );
        }

});