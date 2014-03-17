/**
* @jsx React.DOM
*/ 

var QuestionListHeader = React.createClass({

  render: function() {
    return ( 
        <tr>
            <th style={ {width:'5%'}}> <input type="checkbox"/></th>
 
            <QuestinListHeaderCell width='10%' caption="Chapter" order="asc"/>
            <QuestinListHeaderCell width='10%' caption="Bank" order="asc"/>
            <QuestinListHeaderCell width='10%' caption="Seq" order="asc"/>
            <QuestinListHeaderCell width='40%' caption="Title"
                leftIcon="glyphicon glyphicon-chevron-right" 
                customClassName="title-header"/>
            <QuestinListHeaderCell width='10%' caption="Format" order="asc"/>
            <QuestinListHeaderCell width='15%' caption=""/>
 
          </tr>
      );
    }
});