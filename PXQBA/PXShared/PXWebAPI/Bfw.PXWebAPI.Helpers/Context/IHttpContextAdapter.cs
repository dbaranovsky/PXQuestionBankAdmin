using System;
using System.Web;

namespace Bfw.PXWebAPI.Helpers.Context
{
    public interface IHttpContextAdapter
    {
        HttpContextBase Current { get; }
    }
}
