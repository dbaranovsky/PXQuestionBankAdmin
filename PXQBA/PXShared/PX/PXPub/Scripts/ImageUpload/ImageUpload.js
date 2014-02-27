var PxImageUpload = function ($) {
    return {
        OnImageGridBindCompleted: function (gridId, parentId) {
            $("#loadingMessage").hide();
            var showOptions = false;
            // are there any records?
            $(".jqGrid").each(function (index) {
                if ($(this).getGridParam('records') > 0) {
                    var ids = $(this).getDataIDs();
                    for (var i = 0; i < ids.length; i++) {
                        var cl = ids[i];
                        var url = $(this).getRowData(cl).Url;
                        var res = $(this).getRowData(cl).Title;
                    }
                    $(this).parents('.gridWrapper').show();
                    showOptions = true;
                }
                else {
                    $(this).parents('.gridWrapper').hide();
                }

               
                PxImageUpload.FixGridWidth($(gridId), $('#assignmentViewContent'));
                
            });
            if ((showOptions == false) && ($('.ui-jqgrid').length <= 1)) {
                $("#emptyMessage").show();
            }
            $('a[rel*=jmenu]').contextMenu(
                {
                    menu: 'myMenu'
                },
			    function (action, el, pos) {
				    var queryString = $.jqURL.get("fl");
				    if (action == "paste") {
					    var path = "images/";
					    var queryString = $.jqURL.get("fl");
					    if (queryString != undefined || queryString != null) {
					        path += queryString + "/";
					    }
					    javascript: netBrowserDialog.insert($(el).attr('id'), $(el).attr('id'));
				} 
			}); // contextMenu end
        },

        ReloadImageGrid: function () {
            $("#documentsGrid").jqGrid("GridUnload");
            PxImageUpload.BindImageGrid();
            $.unblockUI();
        },
        
        BindImageGrid: function () {
            var qString = "";
            if (PxPage && PxPage.Context.CurrentUserId) qString = "&uId=" + PxPage.Context.CurrentUserId;

            PxImageUpload.ShowGrid(
            "#documentsGrid",
            PxPage.Context.BaseUrl + '/ImageUpload/ImageGridData/?page=1' + qString,
            '<b>Click on an image to access its options menu:</b>',
            false,
            function () {
                PxImageUpload.OnImageGridBindCompleted("#documentsGrid");
            });
        },
        ShowGrid: function (elemId, dataUrl, gridCaption, isReadOnly, onGridComplete) {
            $.ajax({
                url: dataUrl,
                success: function (gridData) {
                    //PxPage.log(gridData);
                    $(elemId).jqGrid({
                        gridComplete: onGridComplete,
                        datatype: 'jsonstring',
                        datastr: gridData.Data,
                        mtype: 'GET',
                        colNames: gridData.ColNames,
                        colModel: gridData.ColModel,
                        multiselect: isReadOnly,
                        height: 'auto',
                        emptyrecords: 'No records to view',
                        sortname: 'Id',
                        sortorder: "desc",
                        viewrecords: true,
                        hoverrows: false,
                        scrollOffset: 0,
                        imgpath: '/scripts/themes/coffee/images',
                        caption: gridCaption,
                        jsonReader: {
                            root: 'Rows',
                            page: 'Page',
                            total: 'Total',
                            records: 'Records',
                            repeatitems: false,
                            userdata: 'UserData',
                            id: 'Id'
                        }
                    });
                }
            });
        },
        ShowDialog: function (dialogBox, title) {
            $('#fne-window').block({
                message: dialogBox,
                css: {
                    padding: 0,
                    margin: 0,
                    top: '20%',
                    left: '50%'
                }
            });

        },

        FixGridWidth: function (grid, parent) {
            var gviewScrollWidth = grid[0].parentNode.parentNode.parentNode.scrollWidth;
            var mainWidth = jQuery(parent).width();
            var gridScrollWidth = grid[0].scrollWidth;
            var htable = jQuery('table.ui-jqgrid-htable', grid[0].parentNode.parentNode.parentNode);
            var scrollWidth = gridScrollWidth;
            if (htable.length > 0) {
                var hdivScrollWidth = htable[0].scrollWidth;
                if ((gridScrollWidth < hdivScrollWidth))
                    scrollWidth = hdivScrollWidth;
            }
            if (gviewScrollWidth != scrollWidth || scrollWidth > mainWidth) {
                var newGridWidth = (scrollWidth <= mainWidth) ? scrollWidth : mainWidth;
                // if the grid has no data, gridScrollWidth can be less then hdiv[0].scrollWidth
                if (newGridWidth != gviewScrollWidth)
                    grid.jqGrid("setGridWidth", newGridWidth);
            }
        }


    }
}(jQuery);



