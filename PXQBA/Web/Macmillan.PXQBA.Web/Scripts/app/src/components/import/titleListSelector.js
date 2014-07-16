/**
* @jsx React.DOM
*/

var TitleListSelector = React.createClass({

    renderTitles: function() {
        titlesHtml = []
        for(var i=0; i<this.props.data.length; i++) {
            titlesHtml.push(this.renderTitle(this.props.data[i]));
        }
            
        return titlesHtml;
    },

    renderTitle: function(titleModel) {
        return (<TitleSelector data={titleModel} selectTitleHandler={this.props.selectTitleHandler}/>);
    },



    render: function() {
       return (
                <div>
                   <div className="title-list-selector shared-note">
                        <div className="selector-text">
                          <span>
                            {this.props.caption}
                          </span>
                         </div>
                        <div className="selector-menu">
                        {this.props.renderSelectorMenu == undefined? "" : this.props.renderSelectorMenu()}
                        </div>

                   </div>

                     {this.renderTitles()}
                </div>
            );
    }
});

