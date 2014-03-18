﻿using System.Web;
using System.Web.Mvc;
using Macmillan.PXQBA.Web.Filters;

namespace Macmillan.PXQBA.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticationAttribute());
        }
    }
}