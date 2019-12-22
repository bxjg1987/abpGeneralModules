using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Resources.Embedded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BXJG.File
{
    public class BXJGFileModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<BXJGFileModuleConfig>();

            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/jqWebUploader/",
                    Assembly.GetExecutingAssembly(),
                    "BXJG.File.jqWebUploader"
                )
            );
            //Configuration.Localization.Sources.Add(
            //        new DictionaryBasedLocalizationSource(
            //            BXJGConsts.LocalizationSourceName,
            //            new XmlEmbeddedFileLocalizationDictionaryProvider(
            //                Assembly.GetExecutingAssembly(),
            //                "BXJG.File.Localization.Source"
            //                )
            //            )
            //        );

            Configuration.Settings.Providers.Add<BXJGFileSettingProvider>();
            Configuration.Features.Providers.Add<BXJGFileFeatureProvider>();
        }
        public override void Initialize()
        {
           
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
