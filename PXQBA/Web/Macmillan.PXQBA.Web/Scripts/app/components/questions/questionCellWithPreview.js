/**
* @jsx React.DOM
*/ 

var QuestionCellWithPreview = React.createClass({displayName: 'QuestionCellWithPreview',
    render: function() {
        return ( 
				React.DOM.td( {className:this.props.classNameValue}, 
                    React.DOM.div(null, 
                    	React.DOM.span( {className:"glyphicon glyphicon-chevron-right title-expander"}),
                    	 this.props.value
                    ),
                       QuestionPreview( {preview:this.props.htmlPreview})
                )
            );
        }
});