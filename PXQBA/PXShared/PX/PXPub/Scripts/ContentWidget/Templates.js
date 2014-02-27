// This object provides methods for handling functionality related to content templating

var PxTemplates = function ($) {
    return {
        TemplateContexts: {
            Content: { value: 'Content' },
            AssignmentCenter: { value: 'Assignments' },
            Eportfolio: { value: 'Eportfolio' },
            FacePlate: { value: 'FacePlate' },
            Xbook: { value: 'Xbook'},
            Default: { value: 'Default' }
        },

        // If we've not populated the client-side cache, get the templates from the server and
        // populate it.  Run the callback after we get the response, if we need to wait for one,
        // otherwise run it immediately.
        GetTemplates: function (context, callback) {
            if (context.data === undefined || context.data === null) {
                $.ajax({
                    url: PxPage.Routes.get_templates,
                    data: { context: context.value },
                    type: "POST",
                    success: function (response) {
                        context.data = response;
                        callback(context.data);
                    }
                });
            }
            else {
                callback(context.data);
            }
        },

        GetRelatedTemplates: function (context, relatedTo, callback) {
            if (!PxTemplates.getRelatedTemplatesRecentlyRun) {
                $.ajax({
                    url: PxPage.Routes.get_related_templates,
                    data: {
                        context: context.value,
                        itemId: relatedTo
                    },
                    type: "POST",
                    success: function (response) {
                        callback(response);
                    }
                });
                // Wait a reasonable amount of time before allowing this call to happen again.  This lets us make
                // multiple calls to this function (e.g., from Init()) without worrying about multiple net requests.
                PxTemplates.getRelatedTemplatesRecentlyRun = true;
                setTimeout(function () { PxTemplates.getRelatedTemplatesRecentlyRun = false; }, 0);
            }
        },

        SetTemplateItem: function (item, filter) {
            //PxAssignmentCenterUtils.AddFromSelectedTemplate(null, filter);
            return false;
        },

        ShowActionLink: function (event) {
            event.preventDefault();

            $('.tocAssign').hide();
            $('.tocUnAssign').hide();
            $('.tocAssignRight').hide();
            $('.padding').show();

            if ($(this).hasClass('templateLineItem')) {
                $(this).find('.padding').hide();
                $(this).find('.tocAssign').show();
            }

            return true;
        }
    };
} (jQuery);