/**
* @jsx React.DOM
*/

var Question = React.createClass({
	render: function() {
		return (
			<tr className="question">
				<td> 
					<input type="checkbox"/>
				</td>

				<td className="eBookChapter">
					{this.props.eBookChapter}
				</td>

				<td className="questionBank">
				    {this.props.questionBank}
				</td>

				<td className="questionSeq">
					{this.props.questionSeq}
				</td>

				<td className="title">
					{this.props.title}
				</td>

				<td className="questionType">
					{this.props.questionType}
				</td>
			</tr>
			);
		}
});