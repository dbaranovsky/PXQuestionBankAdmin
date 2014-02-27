using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models.CoreServices
{
    public interface IError
    {
        Error Error { get; set; }
    }
}
