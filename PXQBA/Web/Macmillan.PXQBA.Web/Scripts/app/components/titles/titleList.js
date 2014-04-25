/**
* @jsx React.DOM
*/

var TitleList = React.createClass({displayName: 'TitleList',

	renderTitles: function() {
		titlesHtml = []
		for(var i=0; i<this.props.data.length; i++) {
			titlesHtml.push(this.renderTitle(this.props.data[i]));
		}
			
		return titlesHtml;
	},

	renderTitle: function(titleModel) {
		return (Title( {data:titleModel} ));
	},

    render: function() {
       return (
                React.DOM.div(null, 
                     this.renderTitles()
                )
            );
    }
});

