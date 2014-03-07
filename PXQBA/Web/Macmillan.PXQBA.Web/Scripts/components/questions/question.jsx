/**
* @jsx React.DOM
*/

var Question = React.createClass({
	render: function() {
		return (
			<div className="question">
				<div className="eBookChapter">
					{this.props.eBookChapter}
				</div>

				<div className="questionBank">
				    {this.props.questionBank}
				</div>

				<div className="questionSeq">
					{this.props.questionSeq}
				</div>

				<div className="title">
					{this.props.title}
				</div>

				<div className="questionType">
					{this.props.questionType}
				</div>
			</div>
			);
		}
});