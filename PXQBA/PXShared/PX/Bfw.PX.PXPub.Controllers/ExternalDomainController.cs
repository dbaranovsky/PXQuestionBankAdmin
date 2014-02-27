using System.Web.Mvc;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class ExternalDomainController : Controller
    {
        /// <summary>
        /// Print configuration settings
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSettings()
        {
            var configuration = ExternalDomainMapper.GetConfiguration();
            return View(configuration);
        }

        /// <summary>
        /// Change configuration settings
        /// </summary>
        /// <param name="configuration"></param>
        public void ChangeSettings(Bfw.PX.PXPub.Models.ExternalDomainMappingConfig configuration)
        {
            ExternalDomainMapper.GetConfiguration().Enable = configuration.Enable;
           
        }

        /// <summary>
        /// Add a mapping to configuration
        /// </summary>
        /// <param name="mapFrom"></param>
        /// <param name="mapTo"></param>
        public void AddMappings(string mapFrom, string mapTo)
        {
            var configuration = ExternalDomainMapper.GetConfiguration();
            var mapping = configuration.Mappings.Find(i => i.MapFrom == mapFrom);
            if (null == mapping)
            {
                mapping = new Models.ExternalDomainMap { MapFrom = mapFrom, MapTo = mapTo };
                configuration.Mappings.Add(mapping);
            }
            else
            {
                mapping.MapTo = mapTo;
            }
            
        }

        /// <summary>
        /// Remove a mapping from configuration
        /// </summary>
        /// <param name="mapFrom"></param>
        public void RemoveMappings(string mapFrom)
        {
            var configuration = ExternalDomainMapper.GetConfiguration();
            configuration.Mappings.RemoveAll(i => i.MapFrom == mapFrom);
         
        }

        /// <summary>
        /// Reset configuration
        /// </summary>
        public void Reset()
        {
            ExternalDomainMapper.SetConfiguration(null);

        }
    }

}
