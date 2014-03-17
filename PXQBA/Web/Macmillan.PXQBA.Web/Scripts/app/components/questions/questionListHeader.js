/**
* @jsx React.DOM
*/ 

var QuestionListHeader = React.createClass({displayName: 'QuestionListHeader',

  render: function() {
    return ( 
        React.DOM.tr(null, 
            React.DOM.th( {style: {width:'5%'}},  " ", React.DOM.input( {type:"checkbox"})),
 
            QuestinListHeaderCell( {width:"10%", caption:"Chapter", order:"asc"}),
            QuestinListHeaderCell( {width:"10%", caption:"Bank", order:"asc"}),
            QuestinListHeaderCell( {width:"10%", caption:"Seq", order:"asc"}),
            QuestinListHeaderCell( {width:"40%", caption:"Title",
                leftIcon:"glyphicon glyphicon-chevron-right", 
                customClassName:"title-header"}),
            QuestinListHeaderCell( {width:"10%", caption:"Format", order:"asc"}),
            QuestinListHeaderCell( {width:"15%", caption:""})
 
          )
      );
    }
});