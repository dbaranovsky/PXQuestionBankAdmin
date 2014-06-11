/**
* @jsx React.DOM
*/ 

var QuestionPaginator = React.createClass({

    displayPages: 5,
    firstMarker: "_first",
    prevMarker: "_prev",
    nextMarker: "_next",
    lastMarker: "_last",

    getAvailiblePageNumbers: function() {
        availibleNumbers = [];
        var currentPage = this.props.metadata.currentPage;

        if(this.displayPages%2==0) {
            this.displayPages= this.displayPages+1;
        }

        var halfNumbers = this.displayPages/2>>0;
        var leftNumbers = halfNumbers;
        var rightNumbers = halfNumbers;

        if(!(currentPage-leftNumbers>0)) {
            var diff = currentPage-leftNumbers-1;
            leftNumbers+=diff;
            rightNumbers-=diff
        }

        if(currentPage+rightNumbers>this.props.metadata.totalPages) {
            var diff = (currentPage+rightNumbers)-this.props.metadata.totalPages;
            rightNumbers-=diff
        }

        while(leftNumbers>0) {
            availibleNumbers.push(currentPage-leftNumbers);
            leftNumbers--;
        }

        availibleNumbers.push(currentPage);

        var nextPage = currentPage;

        while(rightNumbers>0) {
            nextPage++;
            availibleNumbers.push(nextPage);
            rightNumbers--;
        }

        return availibleNumbers;
    },

    getRealPageNumber: function(page) {
        if(page==this.firstMarker) {
            return 1;
        }
        if(page==this.prevMarker) {
            return this.props.metadata.currentPage-1;
        }
        if(page==this.nextMarker) {
            return this.props.metadata.currentPage+1;
        }
        if(page==this.lastMarker) {
            return this.props.metadata.totalPages;
        }
        return page;
    },

    onClickHandler: function(event) {
       var page = event.target.getAttribute("data-page");
       if(page==null) {
          return;
       }

       page = this.getRealPageNumber(page);

       if(page==this.props.metadata.currentPage) {
          return;
       }

       routsManager.setPage(page);
    },

    renderPageButton: function(pageNumper, isActive) {
        var pageClass = React.addons.classSet({
                'page': true,
                'active': isActive,
            });

        return (<li className={pageClass}><a href="javascript:void(0);" data-page={pageNumper}>{pageNumper}</a></li>)
    },

    renderFirstPageButtons: function() {
        buttons = [];
        var enabled = this.props.metadata.currentPage > 1;

        if(!enabled) {
            buttons.push(<li className="first disabled"><a href="javascript:void(0);">First</a></li>);
            buttons.push(<li className="prev disabled"><a href="javascript:void(0);">Previous</a></li>);
        }
        else {
            buttons.push(<li className="first"><a href="javascript:void(0);" data-page={this.firstMarker}>First</a></li>);
            buttons.push(<li className="prev"><a href="javascript:void(0);" data-page={this.prevMarker}>Previous</a></li>);
        }

        return buttons;
    },

    renderEndPageButtons: function() {
        buttons = [];
        var enabled = this.props.metadata.currentPage < this.props.metadata.totalPages;

        if(!enabled) {
            buttons.push(<li className="next disabled"><a href="javascript:void(0);">Next</a></li>);
            buttons.push(<li className="last disabled"><a href="javascript:void(0);">Last</a></li>);
        }
        else {
            buttons.push(<li className="next"><a href="javascript:void(0);" data-page={this.nextMarker}>Next</a></li>);
            buttons.push(<li className="last"><a href="javascript:void(0);" data-page={this.lastMarker}>Last</a></li>);
        }

        return buttons;
    },

    renderPageButtons: function() {
        var pagebuttons = [];
        var numbers = this.getAvailiblePageNumbers();

        for(var i=0; i<numbers.length; i++) {
            pagebuttons.push(this.renderPageButton(numbers[i], numbers[i]==this.props.metadata.currentPage));
        }

        return pagebuttons;
    },

    render: function() {
        return ( 
                <ul id="pagination-demo" className="pagination-sm pagination" onClick={this.onClickHandler}>
                     {this.renderFirstPageButtons()}
                     {this.renderPageButtons()}
                     {this.renderEndPageButtons()}
               </ul>
              
            );
    }
});