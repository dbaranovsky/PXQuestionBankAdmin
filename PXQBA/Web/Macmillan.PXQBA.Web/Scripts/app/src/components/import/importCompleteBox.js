/**
* @jsx React.DOM
*/

var ImportCompleteBox = React.createClass({

	getUrlToList: function() {
		return window.actions.questionList.buildQuestionListIndexUrl(this.props.titleId, null);
	},

    render: function() {
       return (
                <div className="imported-note">
                   {this.props.questionImported==1? "1 question was" : this.props.questionImported+" questions were"} imported successfully.
                    {this.props.questionImported==1? "This question" :  "These questions "} may require metadata editing.
                   <a href={this.getUrlToList()}> Go to target title ></a>
                </div>
            );
    }
});

