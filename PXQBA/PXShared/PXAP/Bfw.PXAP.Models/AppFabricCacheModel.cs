using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bfw.PXAP.Components;

namespace Bfw.PXAP.Models
{
    public class AppFabricCacheModel
    {
        public string Region { get; set; }

        [Required]
        public string Input { get; set; }

        public SubmitType SubmitType { get; set; }

        public FindType FindType { get; set; }

        public List<MainMenuModel> MenuModel { get; set; }

        public string Result { get; set; }

        public string ItemTagsResult { get; set; }

        public List<SelectListItem> FindOptions { get; set; }
    }

    public enum SubmitType
    {
        Fetch,
        Clear
    }
}
