/**
* @jsx React.DOM
*/

var QuestionTypeList = React.createClass({

      getInitialState: function() {
      return { 
               type: 0, 
             };
    },

    changeHandler: function(type){
        this.setState({type: type});
    },

    renderTypes: function(){
         var self = this;
         var types = this.props.questionTypes.map(function (type) {

          return (<QuestionType questionTypeValue={type.key} questionTypeText={type.value} isSelected={ type.key == self.state.type} changeHandler={self.changeHandler.bind(null, type.key)}/>);
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
             <div className="radio" onClick={this.props.changeHandler}>
           
                            <label>
                                <input type="radio" name="questionTypes" checked={this.props.isSelected} value={this.props.questionTypeValue}/>
                                    {this.props.questionTypeText}
                            </label>
             </div>     
            );
    }
});