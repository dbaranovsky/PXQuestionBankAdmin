/**
* @jsx React.DOM
*/

var QuestionTypeList = React.createClass({displayName: 'QuestionTypeList',

    render: function() {
        this.props.questionTypes = [];
        this.props.questionTypes.push({key: "dsfs", value: "sdfsd"})
        var x = this.props.questionTypes;
        var rows = [];
        this.props.questionTypes.forEach(function(questionType) {
            rows.push(React.DOM.div( {className:"radio"}, 
                            React.DOM.label(null, 
                                React.DOM.input( {type:"radio", name:"questionTypes", id:questionType.key}),
                                    questionType.value 
                            )
                        ));
        }); 
        return (
          	 React.DOM.div( {className:"questionTypeList"}, 
                rows
             )         
            );
    }
});
