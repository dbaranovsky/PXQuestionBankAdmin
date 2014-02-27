//CreateAssignmentRoutes - handles routing for create assignment widget/workflow in Xbook
var CreateAssignmentRoutes = function ($) {
    return {
        //gradebook to display assigned scores view
        assign: function (args) {
            PxCreateNewAssignment.AssignContentDialog();
        },
        //displays the other scores view
        create: function (args) {
            var parentid = args.parentid || '';
            var folderid = args.folderid || '';
            PxCreateNewAssignment.CreateContentDialog(parentid, folderid);
        },
        //displays the item scores view
        save: function (args) {
            var templateid = args.templateid || '';
            var parentid = args.parentid || '';
            var folderid = args.folderid || '';
            PxCreateNewAssignment.SaveContentDialog(templateid, parentid, folderid);
        }
    };
}(jQuery);