using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Bfw.Common.Caching;
using Bfw.PXAP.Components;
using Bfw.PXAP.Models;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Services;

namespace Bfw.PXAP.Controllers
{
    public class AppFabricCacheController : ApplicationController
    {
        private IMenuService menuService;
        private AppFabricCacheService cacheService;

        public AppFabricCacheController(IApplicationContext context) : base(context)
        {
            menuService = new MenuService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            ModelState.Clear();
            var content = new AppFabricCacheModel();
            content.MenuModel = menuService.GetDLAPMenu();
            content.FindOptions = GetFindOptions(FindType.Key);
            return View(content);
        }

        [ValidateInput(false)]
        public ActionResult Execute(AppFabricCacheModel content)
        {
            ModelState.Clear();
            content.ItemTagsResult = null;
            if (content.Input != null)
            {
                if (content.SubmitType == SubmitType.Fetch)
                {
                    PopulateContent(content);
                }
                else if (content.SubmitType == SubmitType.Clear)
                {
                    var result = "No item found to remove.";
                    if (DeleteFromCache(content))
                        result = "Item(s) deleted from cache";
                    content.Result = result;
                }
            }
            content.MenuModel = menuService.GetDLAPMenu();
            content.FindOptions = GetFindOptions(content.FindType);
            return View("Index", content);
        }

        private void PopulateContent(AppFabricCacheModel content)
        {
            string result = "";
            string tagsResult; ;
            cacheService = new AppFabricCacheService();
            if (content.FindType == FindType.Key)
            {
                result = cacheService.GetFromCache(content.Input, content.Region, out tagsResult);
            }
            else
            {
                result = cacheService.GetByTags(content.Input, content.Region, content.FindType, out tagsResult);
            }
            if (result.Length > 1000000)
            {
                result = result.Substring(0, 1000000);
                result += "...Result too long";
            }
            content.Result = result;
            content.ItemTagsResult = "LIST OF ALL ITEM TAGS\n" + tagsResult ?? "";
        }

        private bool DeleteFromCache(AppFabricCacheModel content)
        {
            cacheService = new AppFabricCacheService();
            if (content.FindType == FindType.Key)
            {
                return cacheService.RemoveByKey(content.Input, content.Region);
            }
            else
            {
                return cacheService.RemoveByTag(content.Input, content.Region);
            }
        }

        private List<SelectListItem> GetFindOptions(FindType defaultOption)
        {
            var options = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Find By Key", Value = "Key" },
                new SelectListItem{ Text = "Find By Tag", Value = "Tag" },
                new SelectListItem{ Text = "Find By Any Tag", Value = "AnyTag" },
                new SelectListItem{ Text = "Find By All Tags", Value = "AllTags" }
            };
            var option = options.FirstOrDefault(o => o.Value == defaultOption.ToString());
            if (option != null)
                option.Selected = true;
            return options;
        }
    }
}
