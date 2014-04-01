/**
* @jsx React.DOM
*/

var QuestionTypeList = React.createClass({

    render: function() {
        var x = this.props.questionTypes;
        var rows = [];
        this.props.questionTypes.forEach(function(questionType) {
            rows.push(<div className="radio">
                            <label>
                                <input type="radio" name="questionTypes" id={questionType.key}/>
                                    {questionType.value }
                            </label>
                        </div>);
        }); 
        return (
          	 <div className="questionTypeList">
                {rows}
             </div>         
            );
    }
});
