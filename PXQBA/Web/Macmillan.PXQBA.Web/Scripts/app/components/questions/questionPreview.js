/**
* @jsx React.DOM
*/ 

var QuestionPreview = React.createClass({displayName: 'QuestionPreview',

        render: function() {
            return ( 
                  React.DOM.tr(null, 
                    React.DOM.td( {colSpan:this.props.colSpan}, 
                         React.DOM.span( {dangerouslySetInnerHTML:{__html: this.props.preview}} ),
                         React.DOM.hr(null ),
                         React.DOM.div(null, "I love the smell of napalm in the morning. It smells like victory.")
                    )
                  )
            );
        }
});