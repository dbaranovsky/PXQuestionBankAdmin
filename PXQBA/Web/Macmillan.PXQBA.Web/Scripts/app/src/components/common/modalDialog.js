/**
* @jsx React.DOM
*/

var ModalDialog = React.createClass({

    render: function() {
        return (
            <div className="modal fade" id="addQuestionModal" tabIndex="-1" role="dialog" aria-labelledby="addQuestionModalLabel" aria-hidden="true">
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <button type="button" className="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 className="modal-title" id="myModalLabel">{this.props.renderHeaderText()}</h4>
                        </div>
                        <div className="modal-body">
                            {this.props.renderBody()}
                        </div>
                        <div className="modal-footer">
                           {this.props.renderFooterButtons()}
                        </div>
                    </div>
                </div>
            </div>
            );
    }
});
