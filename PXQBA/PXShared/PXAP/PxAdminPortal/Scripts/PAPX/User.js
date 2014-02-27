var PxApUser = function () {
    return {
        CreateUser: function () {
            showLoading();
            $.get("/dev/User/Create", "", function (data) {
                PxApUser.OpenModal(data)
                hideLoading();
            });
        },

        EditUser: function (userName) {
            showLoading();
            $.get("/dev/User/Edit?username=" + userName, "", function (data) {
                PxApUser.OpenModal(data)
                hideLoading();
            });
        },

        DeleteUser: function (userName) {
            showLoading();
            $.get("/dev/User/Delete?username=" + userName, "", function (data) {
                PxApUser.OpenModal(data)
                hideLoading();
            });
        },

        UpdateUser: function (data) {
            showLoading();
            if (data == "True") {
                PxApUser.ClearAndResetModal();
                $("#list").trigger("reloadGrid");
                hideLoading();
            }
            else {
                $('#userInfo').html('');
                $('#userInfo').append(data);
                hideLoading();
            }
        },

        ClearAndResetModal: function () {
            $('#userInfo').html('');
            $.unblockUI();
        },

        OpenModal: function (data) {
            $('#userInfo').html('');
            $('#userInfo').append(data);
            $.blockUI({ message: $('#userInfo'), css: { width: '400px', cursor: 'default', backgroundColor: 'transparent', border: 'none'} });
            $(".blockOverlay").css('cursor', 'default');
        },

        BindGrid: function () {
            $('#list').jqGrid('GridUnload');

            $("#list").jqGrid({
                url: '/User/_UsersPartial/',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['UserName', 'Name', 'Email', 'Actions'],
                colModel: [
                          { name: 'UserName', index: 'UserName', width: 150, align: 'left' },
                          { name: 'Name', index: 'Name', width: 250, align: 'left' },
                          { name: 'Email', index: 'Email', width: 350, align: 'left' },
                          { name: 'Actions', width: 150, align: 'left', sortable: false },
                          ],
                pager: jQuery('#pager'),
                rowNum: 20,
                rowList: [20, 40, 50],
                height: 250,
                sortname: 'UserName',
                sortorder: "desc",
                viewrecords: true,
                caption: 'Users',
                thousandsSeparator: ',',
                jsonReader: {
                    root: 'Rows',
                    page: 'Page',
                    total: 'Total',
                    records: 'Records',
                    repeatitems: false,
                    userdata: 'UserData',
                    id: 'UserName'
                }
            });
        }
    };
} ();