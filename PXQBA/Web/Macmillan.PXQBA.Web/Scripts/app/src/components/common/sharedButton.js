/**
* @jsx React.DOM
*/

var SharedButton = React.createClass({

    componentDidUpdate: function(){
      this.initializePopovers();
    },

    componentDidMount: function(){
      this.initializePopovers();
    },

    initializePopovers: function() {
    	$(this.getDOMNode()).popover({
                            // selector: '[rel="popover"]',
                            // trigger: 'click', 
                             trigger: this.props.trigger, 
                             placement:'bottom',           
                             html: true,
                             container: 'body'});  
    	 

    },

    componentWillUnmount: function () {
    	$(this.getDOMNode()).popover('destroy');
    },


    getTitleCount: function() {
        var isShared = true;
        var shareWith = this.props.sharedWith;
        var titleCount=0;
        if(shareWith==null) {
            isShared = false;
        }
        else {
            isShared =  shareWith != "";
        }
        if(isShared) {
            titleCount = shareWith.split("<br>").length;
        }

        return titleCount;
    },

    renderCourseCountBadge: function(){
      if (!this.isShared()){
        return "";
      }
      return(<span className="badge shared-to-incompare">{this.getTitleCount()}</span>);
    },


    isShared: function(){
            var titleCount = this.getTitleCount();
            return titleCount > 0;
    },

    render: function() {
       return (
                 <button type="button" className="btn btn-default btn-sm custom-btn shared-to"
			     rel="popover"  
			     data-toggle="popover" 
			      data-title={this.isShared()? "Shared with:" : ""}  
			      data-content={this.isShared()? this.props.sharedWith : "<b>Not Shared</b>"} >
                 <span className="glyphicon icon-shared-to" ></span>{this.renderCourseCountBadge()} 
               </button>
            );
    }
});

