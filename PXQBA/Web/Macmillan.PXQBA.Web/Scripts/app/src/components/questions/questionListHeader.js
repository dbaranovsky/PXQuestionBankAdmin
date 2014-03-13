/**
* @jsx React.DOM
*/ 

var QuestionListHeader = React.createClass({

	render: function() {
		return ( 
		    <tr>
        		<th style={ {width:'5%'}}> <input type="checkbox"/></th>
         		<th style={ {width:'10%'}}> Chapter</th>
        		<th style={ {width:'10%'}}> Bank</th>
        		<th style={ {width:'10%'}}> Seq </th>
        		<th style={ {width:'40%'}} className="title-header">
            	     <span className="glyphicon glyphicon-chevron-right"></span> Title
              	</th>
              	<th style={ {width:'10%'}}> Format </th>
              	<th style={ {width:'15%'}}> </th>
        	</tr>
			);
		}
});