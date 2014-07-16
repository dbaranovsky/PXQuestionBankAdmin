/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationBarImport = React.createClass({

    backHandler: function() {
      alert('back');
    },

    importHandler: function() {
      alert('import');
    },


    render: function() {
        return ( 
                 <table className="bulk-operation-bar-table">
                          <tr>
                            <td className="bulk-operation-cell">
                              <div className="bulk-operation-item">
                                 <span> {this.props.message}  </span>
                               </div>
                            </td>
                            <td className="bulk-operation-cell">
                              <div className="bulk-operation-item">
                                <button type="button" className="btn btn-primary"  onClick={this.importHandler}>
                                    Import questions to...
                                </button>
                              </div>
                            </td>
                            <td className="bulk-operation-cell">
                              <div className="bulk-operation-item">
                                 <button type="button" className="btn btn-default" onClick={this.backHandler}>
                                     Back
                                  </button>
                              </div>
                            </td>
                          </tr>
                        </table>
            );
        }
});