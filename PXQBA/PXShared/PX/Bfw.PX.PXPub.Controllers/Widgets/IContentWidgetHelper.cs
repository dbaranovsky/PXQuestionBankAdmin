using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.PXPub.Models;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    public interface IContentWidgetHelper
    {
        List<TocCategory> GetCategoryList(ContentWidget model, string selectedCategory);
        //List<TocItem> GetEbookTocItemList(ContentWidget model, string selectedCategor);
    }
}
