using Abp.Configuration.Startup;
using BXJG.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BXJG.Utils
{
    public static class BXJGUtilsModuleExtensions
    {
        public static BXJGUtilsModuleConfig BXJGUtils(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration.Get<BXJGUtilsModuleConfig>();
        }

        public static BXJGUtilsModuleConfig AddEnum(this BXJGUtilsModuleConfig cfg, string name, Type t, string locationSourceName)
        {
            // if (cfg.Enums == null)
            //  cfg.Enums = new List<EnumConfigItem>();
      //  var sdf = cfg.GetHashCode();
            cfg.Enums.Add(new EnumConfigItem(name, t, locationSourceName));
            return cfg;
        }
    }
}
