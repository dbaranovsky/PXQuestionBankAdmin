/**
* @jsx React.DOM
*/ 

var QuestionListHeader = React.createClass({displayName: 'QuestionListHeader',

  render: function() {
    return ( 
        React.DOM.tr(null, 
            React.DOM.th( {style: {width:'5%'}},  " ", React.DOM.input( {type:"checkbox"})),
            React.DOM.th( {style: {width:'10%'}},  " Chapter"),
            React.DOM.th( {style: {width:'10%'}},  " Bank"),
            React.DOM.th( {style: {width:'10%'}},  " Seq " ),
            React.DOM.th( {style: {width:'40%'}, className:"title-header"}, 
                   React.DOM.span( {className:"glyphicon glyphicon-chevron-right"}), " Title"
                ),
                React.DOM.th( {style: {width:'10%'}},  " Format " ),
                React.DOM.th( {style: {width:'15%'}},  " " )
          )
      );
    }
});