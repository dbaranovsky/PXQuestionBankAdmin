var HtmlQuizHelper = function ($) {
    return {        
        GetAuthToken: function () {
            var retval = 'no token'
            $.ajax({
                type: 'POST',
                url: 'http://dev.dlap.bfwpub.com/dlap.ashx',
                data: {
                    cmd: 'login',
                    username: 'root/pxmigration',
                    password: 'Px-Migration-123',
                    submit: 'login'
                },
                async: false,
                success: function(response) {
                    retval = $(response).find('user').attr('token');
                }
            });
            return retval;
        }
    }
}(jQuery)