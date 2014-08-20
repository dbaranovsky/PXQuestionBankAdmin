/**
* @jsx React.DOM
*/ 

var QuestinListHeaderCell = React.createClass({

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
        return <div className="delete-button seq" onClick={this.dleteButtonEventHandler} data-toggle="tooltip" title="Remove column"> X </div>
    }
    return null;
 },

 renderExpandButton: function() {
     if(this.props.metadataName==window.consts.questionTitleName && this.props.canViewPreview) {
         return (<ExpandButton expanded={this.props.expandedAll} onClickHandler={this.props.expandAllQuestionHandler} targetCaption="all"/>);  
      }
      return null;
 },

 renderCaption: function(){
  return ( <span className="header-caption" onClick={this.changeOrdering} data-title={this.props.metadataName} data-toggle="tooltip" title={"Sort by "+this.props.caption}>
                     {this.props.caption}
                 </span>);
 },

 renderHeader: function(){
      return(  <table>
          <tr>
            <td>{this.renderExpandButton()}</td>
            <td className="header-caption seq">{this.renderCaption()}</td>
            <td> <QuestinListHeaderCellOrdering order={this.props.order} /></td>
            <td className="delete-button seq">{this.renderDeleteButton()}</td>
          </tr>
        </table>);

 },
 
  render: function() {
      return (   
            <th style={ {width: this.props.width}}
              onMouseOver={this.mouseOverHandler}
              onMouseLeave={this.mouseLeaveHandler}>
                  {this.renderHeader()}
            </th>
      );
    }
});


