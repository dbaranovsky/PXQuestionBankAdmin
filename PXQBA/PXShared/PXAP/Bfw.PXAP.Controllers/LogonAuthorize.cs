using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;


namespace Bfw.PXAP.Controllers
{
    /// <summary>
    /// this filter inherits from AuthorizeAttribute. 
    /// OnAuthorization function is overriden to exculde authorization for account controller.
    /// This attribute will be added to base controller (ApplicationController), and it will make sure
    /// user is required to be logged in, except for the login page. If we apply "Authorize" filter to base controller,
    /// it requies user to be login in while access login page, while is not possible
    /// 
    /// NOTE. this implmentation requires that certain actions of logon class (like change password) should be marked with "Authorize" attribute
    /// </summary>
    public class LogonAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            
            if (!(filterContext.Controller is AccountController))
            {
                base.OnAuthorization(filterContext);
            }
        }
    }
}
