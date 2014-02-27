using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXAP.Components
{
    /// <summary>
    /// Rrepresents set of possible environments, like dev, qa, pristine, production
    /// </summary>
    public enum EnvironmentType
    {
        DEV = 1,
        QA = 2,
        PROD = 3,
        PR = 4,
        LOADTESTING = 5
    }

    public enum FindType
    {
        Key,
        Tag,
        AnyTag,
        AllTags
    }
}
