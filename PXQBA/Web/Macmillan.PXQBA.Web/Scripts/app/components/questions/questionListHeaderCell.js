/**
* @jsx React.DOM
*/ 

var QuestinListHeaderCell = React.createClass({displayName: 'QuestinListHeaderCell',

  getInitialState: function() {
        return { showDeleteButton: false };
  },

  getNextOrdering: function(currentOrder) {
    switch(currentOrder) {
        case window.enums.orderType.asc:
            return window.enums.orderType.desc;
        case window.enums.orderType.desc:
            return window.enums.orderType.none;
        default:
            return window.enums.orderType.asc;    
    } 
  },

  changeOrdering: function(event) {
    var title = event.target.getAttribute("data-title");
    routsManager.setOrder(this.getNextOrdering(this.props.order), title);
 },

  mouseOverHandler: function() {
    this.setState({ showDeleteButton: true });
 },

  mouseLeaveHandler: function() {
   this.setState({ showDeleteButton: false });
 },

  dleteButtonEventHandler: function() {
    routsManager.deleteField(this.props.metadataName);
  },

 renderDeleteButton: function() {
    if((this.state.showDeleteButton)&&(!this.props.canNotDelete)) {

      if (this.props.metadataName== "sequence"){
        return React.DOM.div( {className:"delete-button seq", onClick:this.dleteButtonEventHandler, 'data-toggle':"tooltip", title:"Remove column"},  " X " )
      }
      return React.DOM.span( {className:"delete-button", onClick:this.dleteButtonEventHandler, 'data-toggle':"tooltip", title:"Remove column"},  " X " )
    }


    return null;
 },

 renderExpandButton: function() {
     if(this.props.metadataName==window.consts.questionTitleName) {
         return (ExpandButton( {expanded:this.props.expandedAll, onClickHandler:this.props.expandAllQuestionHandler, targetCaption:"all"}));  
      }
      return null;
 },

 renderCaption: function(){
  return ( React.DOM.span( {className:"header-caption", onClick:this.changeOrdering, 'data-title':this.props.metadataName, 'data-toggle':"tooltip", title:"Sort by "+this.props.caption}, 
                     this.props.caption
                 ));
 },

 renderHeader: function(){
    if (this.props.metadataName == "sequence"){
      return(  React.DOM.table(null, 
          React.DOM.tr(null, 
            React.DOM.td( {className:"header-caption seq"}, this.renderCaption(), "  "),
            React.DOM.td(null,  " ", QuestinListHeaderCellOrdering( {order:this.props.order} )),
            React.DOM.td( {className:"delete-button seq"}, this.renderDeleteButton())
          )
        ));

    }


    return (React.DOM.div(null, this.renderExpandButton(),
                  this.renderCaption(),
                QuestinListHeaderCellOrdering( {order:this.props.order} ), 
                this.renderDeleteButton()
          ));

 },
 
  render: function() {
      return (   
            React.DOM.th( {style: {width: this.props.width},
              onMouseOver:this.mouseOverHandler,
              onMouseLeave:this.mouseLeaveHandler}, 
                  this.renderHeader()
            )
      );
    }
});


