using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace BXJG.Utils.Localization
{
    public static class BXJGUtilsLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(BXJGUtilsConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(BXJGUtilsLocalizationConfigurer).GetAssembly(),
                        "BXJG.Utils.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
