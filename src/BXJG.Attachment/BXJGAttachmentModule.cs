using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Resources.Embedded;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BXJG.Attachment
{
    public class BXJGAttachmentModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<BXJGAttachmentModuleConfig>();


            Configuration.Localization.Sources.Add(
                    new DictionaryBasedLocalizationSource(
                        BXJGAttachmentConsts.LocalizationSourceName,
                        new XmlEmbeddedFileLocalizationDictionaryProvider(
                            Assembly.GetExecutingAssembly(),
                            "BXJG.Attachment.Localization.Source"
                            )
                        )
                    );

            //Configuration.Settings.Providers.Add<BXJGFileSettingProvider>();
            //Configuration.Features.Providers.Add<BXJGFileFeatureProvider>();
        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
