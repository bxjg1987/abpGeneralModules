using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.GeneralTree
{
    public class GeneralTreeModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<GeneralTreeModuleConfig>();


            Configuration.Localization.Sources.Add(
                    new DictionaryBasedLocalizationSource(
                        GeneralTreeConsts.LocalizationSourceName,
                        new XmlEmbeddedFileLocalizationDictionaryProvider(
                            Assembly.GetExecutingAssembly(),
                            "BXJG.GeneralTree.Localization.Source"
                            )
                        )
                    );
        }
        public override void Initialize()
        {
            //if (Configuration.Modules.CommonModule().EnableGeneralTreeDynamicWebApi)
            //{
            //    Configuration.Authorization.Providers.Insert(0, typeof(GeneralTreeAuthorizationProvider));
            //    //Configuration.Authorization.Providers.Add<GeneralTreeAuthorizationProvider>();
            //    Configuration.Navigation.Providers.Insert(0,typeof(GeneralTreeNavigationProvider));
            //   // Configuration.Navigation.Providers.Add<GeneralTreeNavigationProvider>();
            //}
              
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
