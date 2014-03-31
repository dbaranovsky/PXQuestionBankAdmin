/**
* @jsx React.DOM
*/ 

var QuestionCellWithPreview = React.createClass({
    render: function() {
        return ( 
				<td className={this.props.classNameValue}>
                    <div>
                    	<span className="glyphicon glyphicon-chevron-right title-expander"></span>
                    	 {this.props.value}
                    </div>
                       <QuestionPreview preview={this.props.htmlPreview}/>
                </td>
            );
        }
});