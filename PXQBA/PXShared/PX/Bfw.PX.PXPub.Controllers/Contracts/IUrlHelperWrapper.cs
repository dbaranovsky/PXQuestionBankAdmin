using System;
using System.Web.Routing;

namespace Bfw.PX.PXPub.Controllers.Contracts
{
    public interface IUrlHelperWrapper
    {
        //
        // Summary:
        //     Generates a fully qualified URL for the specified route values.
        //
        // Parameters:
        //   routeValues:
        //     An object that contains the parameters for a route. The parameters are retrieved
        //     through reflection by examining the properties of the object. The object
        //     is typically created by using object initializer syntax.
        //
        // Returns:
        //     The fully qualified URL.
        string RouteUrl(object routeValues);

        //
        // Summary:
        //     Generates a fully qualified URL for the specified route values.
        //
        // Parameters:
        //   routeValues:
        //     An object that contains the parameters for a route.
        //
        // Returns:
        //     The fully qualified URL.
        string RouteUrl(RouteValueDictionary routeValues);

        //
        // Summary:
        //     Generates a fully qualified URL for the specified route name.
        //
        // Parameters:
        //   routeName:
        //     The name of the route that is used to generate the URL.
        //
        // Returns:
        //     The fully qualified URL.
        string RouteUrl(string routeName);

        //
        // Summary:
        //     Generates a fully qualified URL for the specified route values by using a
        //     route name.
        //
        // Parameters:
        //   routeName:
        //     The name of the route that is used to generate the URL.
        //
        //   routeValues:
        //     An object that contains the parameters for a route. The parameters are retrieved
        //     through reflection by examining the properties of the object. The object
        //     is typically created by using object initializer syntax.
        //
        // Returns:
        //     The fully qualified URL.
        string RouteUrl(string routeName, object routeValues);

        //
        // Summary:
        //     Generates a fully qualified URL for the specified route values by using a
        //     route name.
        //
        // Parameters:
        //   routeName:
        //     The name of the route that is used to generate the URL.
        //
        //   routeValues:
        //     An object that contains the parameters for a route.
        //
        // Returns:
        //     The fully qualified URL.
        string RouteUrl(string routeName, RouteValueDictionary routeValues);

        //
        // Summary:
        //     Generates a fully qualified URL for the specified route values by using a
        //     route name and the protocol to use.
        //
        // Parameters:
        //   routeName:
        //     The name of the route that is used to generate the URL.
        //
        //   routeValues:
        //     An object that contains the parameters for a route. The parameters are retrieved
        //     through reflection by examining the properties of the object. The object
        //     is typically created by using object initializer syntax.
        //
        //   protocol:
        //     The protocol for the URL, such as "http" or "https".
        //
        // Returns:
        //     The fully qualified URL.
        string RouteUrl(string routeName, object routeValues, string protocol);

        //
        // Summary:
        //     Generates a fully qualified URL for the specified route values by using the
        //     specified route name, protocol to use, and host name.
        //
        // Parameters:
        //   routeName:
        //     The name of the route that is used to generate the URL.
        //
        //   routeValues:
        //     An object that contains the parameters for a route.
        //
        //   protocol:
        //     The protocol for the URL, such as "http" or "https".
        //
        //   hostName:
        //     The host name for the URL.
        //
        // Returns:
        //     The fully qualified URL.
        string RouteUrl(string routeName, RouteValueDictionary routeValues, string protocol, string hostName);

        /// <summary>
        /// Generates a fully qualified URL to an action method by using the specified action name, controller name, and route values.
        /// </summary>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">An object that contains the parameters for a route. The parameters are retrieved through reflection by examining the properties of the object. The object is typically created by using object initializer syntax.</param>
        /// <returns>The fully qualified URL to an action method.</returns>
        string Action(string actionName, string controllerName, object routeValues);

    }
}
