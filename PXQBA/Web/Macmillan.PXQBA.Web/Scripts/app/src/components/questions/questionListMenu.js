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

    componentDidUpdate: function(){
      this.initializePopovers();
    },

     componentDidMount: function(){
       this.initializePopovers();
    },

    initializePopovers: function(){
        $(this.getDOMNode()).find('.shared-to').popover({title: 'Shared to:', trigger: 'hover', placement:'bottom', container:'body', html: true});
        $(this.getDOMNode()).find('.shared-from').popover({title: 'Unsubscribe from:', trigger: 'hover', placement:'bottom', container:'body', html: true});
    },


    render: function() {
      if (this.props.showAll){
        return ( 
                <div>

                   {  
                    this.props.data["sharedTo"] != "" ?
                     <button type="button" className="btn btn-default btn-sm custom-btn shared-to" data-content={this.props.data["sharedTo"]}><span className="glyphicon icon-shared-to" ></span></button> :
                    ""
                 }

                  {  
                    this.props.data["sharedFrom"] != "" ?
                     <button type="button" className="btn btn-default btn-sm custom-btn shared-from" data-content={this.props.data["sharedFrom"]}><span className="glyphicon icon-shared-from"></span></button> :
                    ""
                 }

                
                
                  <button type="button" className="btn btn-default btn-sm" onClick={this.editNotesHandler} data-toggle="tooltip" title="Edit Notes"><span className="glyphicon glyphicon-list-alt"></span></button>	
                  <button type="button" className="btn btn-default btn-sm" onClick={this.editQuestionHandler}  data-toggle="tooltip" title="Edit Question"><span className="glyphicon glyphicon-pencil"></span></button>
                  <button type="button" className="btn btn-default btn-sm" onClick={this.copyQuestionHandler}  data-toggle="tooltip" title="Duplicate Question"><span className="glyphicon glyphicon-copyright-mark"></span></button>
                  <button type="button" className="btn btn-default btn-sm" data-toggle="tooltip" title="Delete Question"><span className="glyphicon glyphicon-trash"></span></button>
                </div>
            );
      }

      return (<div>
                 {  
                    this.props.data["sharedTo"] != "" ?
                    <button type="button" className="btn btn-default btn-sm custom-btn shared-to" data-content={this.props.data["sharedTo"]}><span className="glyphicon icon-shared-to"></span></button> :
                    ""
                 }

                  {  
                    this.props.data["sharedFrom"] != "" ?
                    <button type="button" className="btn btn-default btn-sm custom-btn shared-from" data-content={this.props.data["sharedFrom"]}><span className="glyphicon icon-shared-from"></span></button> :
                    ""
                 }
              

             </div>);
  }
});