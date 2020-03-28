using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp;
using BXJG.Utils.Localization;
using BXJG.Utils.Enums;

namespace BXJG.Utils
{
    /*
     * 通用公共功能模块
     */
    //[DependsOn( typeof(Abp.AbpKernelModule))]
    public class BXJGUtilsModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<BXJGUtilsModuleConfig>();
            Configuration.Modules.BXJGUtils().AddEnum("gender", typeof(Gender), BXJGUtilsConsts.LocalizationSourceName);

            BXJGUtilsLocalizationConfigurer.Configure(Configuration.Localization);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BXJGUtilsModule).GetAssembly());
        }

        public override void PostInitialize()
        {
        }
    }
}
