/**
* @jsx React.DOM
*/ 

var QuestinListHeaderCell = React.createClass({
 
  render: function() {
             leftIcon = this.renderLeftIcon(this.props.leftIcon);
      return (   
            <th style={ {width: this.props.width}} className={this.props.customClassName}>
                   <span className={this.props.leftIcon}> </span>
                 <span>
                     {this.props.caption}
                 </span>
                <QuestinListHeaderCellOrdering order={this.props.order}/> 
            </th>
      );
    }
});