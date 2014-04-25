using System.Collections;
using System.Collections.Generic;

namespace Macmillan.PXQBA.Web.ViewModels.TiteList
{
    public class TitleViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public IEnumerable<ChapterViewModel> Chapters { get; set; }
    }
}
