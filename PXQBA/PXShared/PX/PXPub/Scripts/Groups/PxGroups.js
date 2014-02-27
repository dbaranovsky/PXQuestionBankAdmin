var PxGroups = function ($) {

    $(document).ready(function () {
      //  PxGroups.Init();
    });

    return {
        Init: function () {
            PxPage.FneInitHooks['edit-groups'] = PxGroups.FneInit;
            PxPage.FneResizeHooks['edit-groups'] = PxGroups.FneResize;

            $('.group-set .actions a').click(function (event) { PxGroups.ClickWhenFneClosed = $(event.target).closest('li').find('a.group-link'); });
            $('.group-set .actions a.delete').unbind().click(function (event) { PxGroups.ClickWhenFneClosed = 'a.all-students'; });
            $('a.create-group-set').click(function () { PxGroups.ClickWhenFneClosed = '.group-sets-list .group-set:last a.group-link'; });

            $('.group-link').click(function (event) {
                PxPage.log('clicked');
                $('.group-link').removeClass('selected');
                $(this).addClass('selected');
            });
        },

        ClickWhenFneClosed: '',

        FneInit: function () {
            PxGroups.FneResize();
            $('#fne-unblock-action').click(PxGroups.FneClosed);
        },

        FneClosed: function () {
            PxGroups.UpdateGroupSets(function () {
                PxGroups.ShowRequestedGroupSet();
            });
        },

        ShowRequestedGroupSet: function () {
            PxGroups.Init();
            $(PxGroups.ClickWhenFneClosed).click();
        },

        UpdateGroupSets: function (callback) {
            $('.group-sets-list').load(PxPage.Routes['group_list'], callback);
        },

        DeleteComplete: function () {
            PxGroups.UpdateGroupSets(PxGroups.ShowRequestedGroupSet);
        },

        FneResize: function () {
            var totalHeight = $('#fne-window').outerHeight();
            var titleHeight = $('#fne-title').outerHeight();
            $('#fne-content, #fne-content iframe.component').height(totalHeight - titleHeight);
        }
    };

} (jQuery);