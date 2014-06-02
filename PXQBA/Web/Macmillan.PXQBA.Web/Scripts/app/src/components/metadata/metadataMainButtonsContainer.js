/**
* @jsx React.DOM
*/

var MetadataMainButtonsContainer= React.createClass({
    render: function() {
       return (
               <div> 
               	<button type="button" className="btn btn-default metadata-button"  onClick={this.props.cancelHandler} >Cancel</button>
               	<button type="button" className="btn btn-primary metadata-button"  onClick={this.props.saveHandler} >Save</button>
               </div>
            );
    }
});




