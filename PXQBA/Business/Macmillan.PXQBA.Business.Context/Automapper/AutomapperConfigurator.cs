using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Macmillan.PXQBA.Business.Automapper
{
    public class AutomapperConfigurator
    {
        private readonly Profile profile;

        public AutomapperConfigurator(Profile profile)
        {
            this.profile = profile;
        }

        public void Configure()
        {
            Mapper.Initialize(configuration => configuration.AddProfile(profile));
        }
    }
}
