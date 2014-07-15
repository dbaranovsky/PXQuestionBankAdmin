/**
* @jsx React.DOM
*/ 

var QuestionBulkOperationBarImport = React.createClass({

    deselectsAllHandler: function() {
        this.props.deselectsAllHandler();
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
                               <div className="bulk-operation-item" data-toggle="tooltip" title="  !!!!!!!!!!!!">
                                 !
                               </div>
                            </td>
                            <td className="bulk-operation-cell">
                               <div className="deselect-button" onClick={this.deselectsAllHandler} data-toggle="tooltip" title="Deselect all">
                                 <span > X </span>
                               </div>
                            </td>
                          </tr>
                        </table>
            );
        }
});