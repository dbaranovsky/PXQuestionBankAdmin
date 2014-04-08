/**
* @jsx React.DOM
*/

var QuestionTypeList = React.createClass({

    renderTypes: function(){
         var self = this;
         var types = this.props.questionTypes.map(function (type) {
           // return (<QuestionType questionTypeValue={type.key} questionTypeText={type.value} changeHandler={self.props.changeHandler.bind(self, type.key)}/>);
            return (<QuestionType questionTypeValue={type.key} questionTypeText={type.value} />);
          });
         return types;
    },

    render: function() {
       
        return (
          	 <div className="questionTypeList">
                {this.renderTypes()}
             </div>         
            );
    }
});

var QuestionType = React.createClass({

    render: function() {
        return (
             //<div className="radio" onClick={this.props.changeHandler}>
             <div className="radio">
                            <label>
                                <input type="radio" name="questionTypes" value={this.props.questionTypeValue}/>
                                    {this.props.questionTypeText}
                            </label>
             </div>     
            );
    }
});