/**
* @jsx React.DOM
*/

var QuestionTypeList = React.createClass({displayName: 'QuestionTypeList',

    renderTypes: function(){
         var self = this;
         var types = this.props.questionTypes.map(function (type) {
           // return (<QuestionType questionTypeValue={type.key} questionTypeText={type.value} changeHandler={self.props.changeHandler.bind(self, type.key)}/>);
            return (QuestionType( {questionTypeValue:type.key, questionTypeText:type.value} ));
          });
         return types;
    },

    render: function() {
       
        return (
          	 React.DOM.div( {className:"questionTypeList"}, 
                this.renderTypes()
             )         
            );
    }
});

var QuestionType = React.createClass({displayName: 'QuestionType',

    render: function() {
        return (
             //<div className="radio" onClick={this.props.changeHandler}>
             React.DOM.div( {className:"radio"}, 
                            React.DOM.label(null, 
                                React.DOM.input( {type:"radio", name:"questionTypes", value:this.props.questionTypeValue}),
                                    this.props.questionTypeText
                            )
             )     
            );
    }
});