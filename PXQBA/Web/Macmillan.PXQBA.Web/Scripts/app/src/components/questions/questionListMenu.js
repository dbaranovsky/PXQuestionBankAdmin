/**
* @jsx React.DOM
*/ 

var QuestionListMenu = React.createClass({

    editNotesHandler: function(){
      this.props.editNotesHandler();
    },

    copyQuestionHandler: function() {
      this.props.copyQuestionHandler();
    },

    editQuestionHandler: function() {
        this.props.editQuestionHandler();
    },

    componentDidMount: function(){
      $(this.getDOMNode()).find('.shared-to').popover({title: 'Shared to:', trigger: 'hover', placement:'bottom', container:'body', html: true});
      $(this.getDOMNode()).find('.shared-from').popover({title: 'Unsubscribe from:', trigger: 'hover', placement:'bottom', container:'body', html: true});
      

    },

    render: function() {
      if (this.props.showAll){
        return ( 
                <div>
                  <button type="button" className="btn btn-default btn-sm custom-btn shared-to" data-toggle="popover" data-content="Vivamus sagittis lacus vel &#013; augue laoreet rutrum faucibus."><span className="glyphicon icon-shared-to" ></span></button>
                  <button type="button" className="btn btn-default btn-sm custom-btn shared-from"><span className="glyphicon icon-shared-from"></span></button>
                  <button type="button" className="btn btn-default btn-sm" onClick={this.editNotesHandler}><span className="glyphicon glyphicon-list-alt"></span></button>	
                  <button type="button" className="btn btn-default btn-sm" onClick={this.editQuestionHandler}><span className="glyphicon glyphicon-pencil"></span></button>
                  <button type="button" className="btn btn-default btn-sm" onClick={this.copyQuestionHandler}><span className="glyphicon glyphicon-copyright-mark"></span></button>
                  <button type="button" className="btn btn-default btn-sm"><span className="glyphicon glyphicon-trash"></span></button>
                </div>
            );
      }

      return (<div>

               <button type="button" className="btn btn-default btn-sm custom-btn shared-to"><span className="glyphicon icon-shared-to"></span></button>
               <button type="button" className="btn btn-default btn-sm custom-btn shared-from"><span className="glyphicon icon-shared-from"></span></button>

             </div>);
  }
});