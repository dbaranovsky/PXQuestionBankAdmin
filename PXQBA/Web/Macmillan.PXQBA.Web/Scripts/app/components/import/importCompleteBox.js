/**
* @jsx React.DOM
*/

var ImportCompleteBox = React.createClass({displayName: 'ImportCompleteBox',

	getUrlToList: function() {
		return window.actions.questionList.buildQuestionListIndexUrl(this.props.titleId, null);
	},

    render: function() {
       return (
                React.DOM.div({className: "imported-note"}, 
                   this.props.questionImported==1? "1 question was" : this.props.questionImported+" questions were", " imported successfully.", 
                    this.props.questionImported==1? "This question" :  "These questions ", " may require metadata editing.", 
                   React.DOM.a({href: this.getUrlToList()}, " Go to target title >")
                )
            );
    }
});

