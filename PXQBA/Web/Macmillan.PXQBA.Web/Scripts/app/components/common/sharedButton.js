/**
* @jsx React.DOM
*/

var SharedButton = React.createClass({displayName: 'SharedButton',

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
      return(React.DOM.span( {className:"shared-counter"}, this.getTitleCount()));
    },


    isShared: function(){
            var titleCount = this.getTitleCount();
            return titleCount > 0;
    },

    render: function() {
       return (
                 React.DOM.button( {type:"button", className:"btn btn-default btn-sm shared-btn shared-to",
			     rel:"popover",  
			     'data-toggle':"popover", 
			      'data-title':this.isShared()? "Shared with:" : "",  
			      'data-content':this.isShared()? this.props.sharedWith : "<b>Not Shared</b>"} , 
                 React.DOM.span( {className:"icon-share"} ),this.renderCourseCountBadge() 
               )
            );
    }
});

