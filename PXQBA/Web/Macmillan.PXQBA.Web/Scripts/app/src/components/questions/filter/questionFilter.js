/**
* @jsx React.DOM
*/

var QuestionFilter = React.createClass({
    render: function() {
        return (
            <div className="questionFilter">
                 <div> 
                    <span>
                         <strong> Filter: </strong>  <QuestionFilterItemsAppender />
                    </span>
                    <a href="#/filter/query/page/1"> Favourite filter</a>
                    <QuestionFilterItemsContainer />
                </div>
            </div>
            );
        }
});