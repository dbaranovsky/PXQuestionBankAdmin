/**
* @jsx React.DOM
*/ 

var QuestionListColumnSelector = React.createClass({

	render: function() {
		return (
           <div className="modal fade" id="addQuestionFieldModal" tabindex="-1" role="dialog" aria-labelledby="addQuestionFieldLabel" aria-hidden="true">
              <div className="modal-dialog">
                <div className="modal-content">
                  <div className="modal-header">
                    <button type="button" className="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 className="modal-title" id="myModalLabel">Add Fields</h4>
                  </div>
                  <div className="modal-body">
                    ...
                  </div>
                  <div className="modal-footer">
                    <button type="button" className="btn btn-primary">Save changes</button>
                    <button type="button" className="btn btn-default" data-dismiss="modal">Close</button>
                  </div>
                </div>
              </div>
            </div>
			);
	}
});                  