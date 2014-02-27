using System.Configuration;
using Bfw.PX.PXPub.Controllers.Configurations;
using Bfw.PX.PXPub.Models;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    /// <summary>
    /// This mapper handles arga item
    /// </summary>
    public static class ExternalDomainMapper
    {
        private static ExternalDomainMappingConfig _configuration;


        /// <summary>
        /// Get arga configuration
        /// </summary>
        /// <returns></returns>
        public static ExternalDomainMappingConfig GetConfiguration()
        {
            if (null == _configuration)
                _configuration = (ConfigurationManager.GetSection("externalDomainMapping") as ExternalDomainMapping).ToExternalDomainConfiguration();

            return _configuration;
        }

        /// <summary>
        /// Set arga confuration
        /// </summary>
        /// <param name="conf"></param>
        public static void SetConfiguration(ExternalDomainMappingConfig conf)
        {
            _configuration = conf;
        }

        public static bool IsEnable()
        {

            return GetConfiguration().Enable;
        }

        /// <summary>
        /// Convert a arga domain url to px domain url
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static void MapUrlToPxUrl(ExternalContent item)
        {
            if (null == item || string.IsNullOrEmpty(item.Url))
                return;
            var argaManger = GetConfiguration();

            if (null == argaManger || null == argaManger.Mappings || !argaManger.Enable)
                return;
            foreach (ExternalDomainMap mapping in argaManger.Mappings)
            {
                if (item.Url.Contains(mapping.MapFrom))
                {
                    item.Url = item.Url.Replace(mapping.MapFrom, mapping.MapTo);
                    item.IsTransformedArgaItem = true;
                }
            }

        }

        /// <summary>
        /// Convert a arga domain url to px domain url
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static void MapUrlToPxUrl(Activity activity)
        {
            if (null == activity || string.IsNullOrEmpty(activity.Href))
                return;
            var argaManger = GetConfiguration();

            if (null == argaManger || null == argaManger.Mappings || !argaManger.Enable)
                return;
            foreach (ExternalDomainMap mapping in argaManger.Mappings)
            {
                if (activity.Href.Contains(mapping.MapFrom))
                {
                    activity.Href = activity.Href.Replace(mapping.MapFrom, mapping.MapTo);
                    activity.IsTransformedArgaItem = true;
                }
            }

        }

        /// <summary>
        /// Convert Arga section configuration to model configuration
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        public static ExternalDomainMappingConfig ToExternalDomainConfiguration(this ExternalDomainMapping conf)
        {
            var c = new ExternalDomainMappingConfig();

            if (null == conf)
                return c;
            c.Enable = conf.Enable;

            foreach (ExternalDomainMapping.ExternalDomainMap mapping in conf.Mappings)
            {
                c.Mappings.Add(new ExternalDomainMap()
                {
                    MapFrom = mapping.MapFrom,
                    MapTo = mapping.MapTo
                });
            }
            return c;
        }

    }
}
