using Abp.Configuration.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    public static class GeneralTreeModuleConfigurationExtensions
    {
        public static GeneralTreeModuleConfig BXJGGeneralTree(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration.Get<GeneralTreeModuleConfig>();
        }
    }
}
