using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;


namespace Bfw.PXAP.Models
{
    public class LayoutModel
    {
        /// <summary>
        /// list of all environments for the system
        /// </summary>
        public List<PXEnvironment> PxEnvironments { get; set; }

        /// <summary>
        /// options for environment drop down on the layout page
        /// </summary>
        public List<SelectListItem> EnvironmentOptions
        {
            get
            {
                List<SelectListItem> envOptions = new List<SelectListItem>();

                foreach (var env in PxEnvironments)
                {
                    SelectListItem item = new SelectListItem() { Selected = false, Text = env.Title, Value = env.EnvironmentId.ToString() };
                    //item.Value = UrlHelper.GenerateUrl(
                    if (env.Title.Equals(CurrentEnvironment, StringComparison.CurrentCultureIgnoreCase))
                    {
                        item.Selected = true;
                    }
                    envOptions.Add(item);
                }

                // add empty option, and if there is no current env, then select this option.
                if (string.IsNullOrEmpty(this.CurrentEnvironment))
                {
                    envOptions.Insert(0, new SelectListItem() { Text = string.Empty, Value = "", Selected = true });
                }

                return envOptions;
            }
        }
        public string CurrentEnvironment { get; set; }
        public List<ExternalMenuModel> ExternalMenuModel { get; set; }
        public List<MainMenuModel> MainMenuModel { get; set; }

        /// <summary>
        /// returns the PXEnvironment object for the current env
        /// </summary>
        /// <returns></returns>
        public PXEnvironment GetCurrentEnvironment()
        {
            var env = (from e in this.PxEnvironments
                       where e.Title.Equals(this.CurrentEnvironment, StringComparison.CurrentCultureIgnoreCase)
                       select e).FirstOrDefault();

            return env;
        }

    }
}
